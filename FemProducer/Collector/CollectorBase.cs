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

		private (Dictionary<string, Matrix>, Vector) GetMatrixesMG()
		{
			Matrix M = _matrixFactory.CreateMatrix(_grid);
			Matrix G = _matrixFactory.CreateMatrix(_grid);
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


				var localVector = _basis.GetLocalVector(nodes, _problemService.F, formulaNumber);
				AddLocalVector(vector, localVector, element);
			}//);

			Dictionary<string, Matrix> matrixes = new() { { "M", M }, { "G", G } };
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
				for (int p = 0; p < 4; p++)
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
			ConsiderFirstBoundaryConditions(slae);
			//	ConsiderSecondBoundaryConditions(slae);
			//	ConsiderThirdBoundaryConditions(slae);
		}

		private void ConsiderFirstBoundaryConditions(Slae slae)
		{
			Parallel.ForEach(_grid.FirstBoundaryNodes, boundaryNodeIndex =>
			{
				var area = _grid.GetSubDomain(_grid.Nodes[boundaryNodeIndex]);
				_basis.ConsiderFirstBoundaryCondition(slae, _grid.Nodes[boundaryNodeIndex], boundaryNodeIndex, area);
			});
		}

		//private void ConsiderSecondBoundaryConditions(Slae slae)
		//{
		//	Parallel.ForEach(_grid.FirstBoundaryNodes, boundaryNodeIndex =>
		//	{
		//		_basis.ConsiderFirstBoundaryCondition(slae, _grid.Nodes[boundaryNodeIndex], boundaryNodeIndex);
		//	});
		//}

		//private void ConsiderThirdBoundaryConditions(Slae slae)
		//{
		//	Parallel.ForEach(_grid.FirstBoundaryNodes, boundaryNodeIndex =>
		//	{
		//		_basis.ConsiderFirstBoundaryCondition(slae, _grid.Nodes[boundaryNodeIndex], boundaryNodeIndex);
		//	});
		//}
	}
}