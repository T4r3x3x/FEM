using FemProducer.Basises;
using FemProducer.MatrixBuilding;

using Grid.Models;

using MathModels.Models;

using Tools;

namespace FemProducer.Collector
{
	public class CollectorBase : ICollectorBase
	{
		private readonly GridModel _grid;
		private readonly MatrixFactory _matrixFactory;
		private readonly ProblemService _problemService;
		private object _lock = new object();
		private readonly AbstractBasis _basis;

		public CollectorBase(GridModel grid, MatrixFactory matrixFactory, ProblemService problemService, AbstractBasis basis)
		{
			_grid = grid;
			_matrixFactory = matrixFactory;
			_problemService = problemService;
			_basis = basis;
		}

		public (Dictionary<string, Matrix>, Vector) Collect()
		{
			return GetMatrixesMG();
		}

		private int GetAreaNumber(IList<Node> nodes)
		{
			Node center = new Node((nodes[0].X + nodes[1].X) / 2, (nodes[0].Y + nodes[2].Y) / 2);
			return _grid.GetAreaNumber(center);
		}

		private (Dictionary<string, Matrix>, Vector) GetMatrixesMG()
		{
			Matrix M = _matrixFactory.CreateMatrix(_grid);
			Matrix G = _matrixFactory.CreateMatrix(_grid);
			Matrix H = _matrixFactory.CreateMatrix(_grid);
			Vector vector = new Vector(M.Size);

			//	Parallel.ForEach(_grid.Elements, element =>
			foreach (var element in _grid.Elements)
			{
				int formulaNumber = element.formulaNumber;
				var nodes = _grid.ElementToNode(element);


				var localMatrix = _basis.GetMassMatrix(nodes);
				localMatrix.MultiplyLocalMatrix(_problemService.Gamma(formulaNumber));
				AddLocalMatrix(M, localMatrix, element);


				localMatrix = _basis.GetStiffnessMatrix(nodes);
				localMatrix.MultiplyLocalMatrix(_problemService.Lambda(formulaNumber));
				AddLocalMatrix(G, localMatrix, element);

				var areaNumber = GetAreaNumber(nodes);
				localMatrix = ((LinearRectangularCylindricalBasis)_basis).GetGradTMatrix(nodes, areaNumber);
				localMatrix.MultiplyLocalMatrix(_problemService.Gamma(formulaNumber));
				AddLocalMatrix(H, localMatrix, element);

				var localVector = _basis.GetLocalVector(nodes, _problemService.F, formulaNumber);
				AddLocalVector(vector, localVector, element);
			}//);

			Dictionary<string, Matrix> matrixes = new() { { "M", M }, { "G", G }, { "H", H } };
			return (matrixes, vector);
		}

		private void AddLocalVector(Vector vector, IList<double> localVector, FiniteElement element)
		{
			lock (_lock)
				for (int i = 0; i < localVector.Count; i++)
					vector[element.NodesIndexes[i]] += localVector[i];
		}

		private void AddLocalMatrix(Matrix matrix, IList<IList<double>> localMatrix, FiniteElement element)
		{
			lock (_lock)
			{
				for (int p = 0; p < element.NodesIndexes.Length; p++)
				{
					matrix.Di[element.NodesIndexes[p]] += localMatrix[p][p];

					int ibeg = matrix.Ia[element.NodesIndexes[p]];
					int iend = matrix.Ia[element.NodesIndexes[p] + 1];
					for (int k = 0; k < p; k++)
					{
						int index = SearchingAlghoritms.BinarySearch(matrix.Ja, element.NodesIndexes[k], ibeg, iend - 1);

						matrix.Au[index] += localMatrix[k][p];
						matrix.Al[index] += localMatrix[p][k];
						ibeg++;
					}
				}
			}
		}

		public void GetBoundaryConditions(Slae slae)
		{
			ConsiderSecondBoundaryConditions(slae);
			ConsiderThirdBoundaryConditions(slae);
			ConsiderFirstBoundaryConditions(slae);
		}

		private void ConsiderFirstBoundaryConditions(Slae slae)
		{
			Parallel.ForEach(_grid.FirstBoundaryNodes, boundaryNodeIndex =>
			{
				var area = _grid.GetSubDomain(_grid.Nodes[boundaryNodeIndex]);
				_basis.ConsiderFirstBoundaryCondition(slae, _grid.Nodes[boundaryNodeIndex], boundaryNodeIndex, area);
			});
		}
		private void ConsiderSecondBoundaryConditions(Slae slae)
		{
			//Parallel.ForEach(_grid.SecondBoundaryNodes, nodesIndexes =>
			//{
			//	List<Node> nodes = new();
			//	for (int i = 0; i < nodesIndexes.Count; i++)
			//		nodes.Add(_grid.Nodes[nodesIndexes[i]]);

			//	_basis.ConsiderSecondBoundaryCondition(slae, nodes, nodesIndexes);
			//});
		}

		private void ConsiderThirdBoundaryConditions(Slae slae)
		{
			//Parallel.ForEach(_grid.ThirdBoundaryNodes, nodesIndexes =>
			foreach (var nodesIndexes in _grid.ThirdBoundaryNodes)
			{
				List<Node> nodes = new();
				for (int i = 0; i < nodesIndexes.Item1.Count; i++)
					nodes.Add(_grid.Nodes[nodesIndexes.Item1[i]]);

				int formulaNumber = nodesIndexes.Item2;
				var res = _basis.ConsiderThirdBoundaryCondition(slae, nodes, nodesIndexes.Item1, _problemService.FBetta, formulaNumber);
				AddLocalMatrix(slae.Matrix, res.Item1, new FiniteElement([nodesIndexes.Item1[0], nodesIndexes.Item1[1]], formulaNumber));
				AddLocalVector(slae.Vector, res.Item2, new FiniteElement([nodesIndexes.Item1[0], nodesIndexes.Item1[1]], formulaNumber));
			}
		}
	}
}