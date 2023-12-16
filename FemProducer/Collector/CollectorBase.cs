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
		private readonly ProblemService _problemParametrs;
		private object _lock = new object();
		private readonly IBasis _basis;

		public CollectorBase(GridModel grid, MatrixFactory matrixFactory, ProblemService problemParametrs, IBasis basis)
		{
			_grid = grid;
			_matrixFactory = matrixFactory;
			_problemParametrs = problemParametrs;
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

			//Parallel.ForEach(_grid.Elements, element =>
			foreach (var element in _grid.Elements)
			{
				int formulaNumber = element.formulaNumber;

				var nodes = _grid.ElementToNode(element);

				var localMatrix = _basis.GetMassMatrix(nodes);
				localMatrix.MultiplyLocalMatrix(_problemParametrs.Gamma(formulaNumber));
				AddLocalMatrix(M, localMatrix, element);

				localMatrix = _basis.GetStiffnessMatrix(nodes);
				localMatrix.MultiplyLocalMatrix(_problemParametrs.Lamda(formulaNumber));
				AddLocalMatrix(G, localMatrix, element);

				var localVector = _basis.GetLocalVector(nodes, _problemParametrs.F1, formulaNumber);
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
			Parallel.ForEach(_grid.FirstBoundaryNodes, boundaryNodeIndex =>
			{
				AccountFirstCondition(slae, _grid.Nodes[boundaryNodeIndex], boundaryNodeIndex);
			});

		}

		private void AccountFirstCondition(Slae slae, Node node, int nodeIndex)
		{
			slae.Matrix.ZeroingRow(nodeIndex);
			slae.Matrix.Di[nodeIndex] = 1;
			slae.Vector[nodeIndex] = _problemParametrs.Function(node);
		}

		private void AccountSecondConditionHorizontal(int i, int j, int area, NormalVectorDirection normal)
		{
			//_vector[i] += (int)normal * _problemParametrs.Lamda(area) * _grid.Hx[i] / 6 * (2 * _problemParametrs.DivFuncY1(_grid.X[i], _grid.Y[j], area) + _problemParametrs.DivFuncY1(_grid.X[i + 1], _grid.Y[j], area));
			//_vector[i + 1] += (int)normal * _problemParametrs.Lamda(area) * _grid.Hx[i] / 6 * (_problemParametrs.DivFuncY1(_grid.X[i], _grid.Y[j], area) + 2 * _problemParametrs.DivFuncY1(_grid.X[i + 1], _grid.Y[j], area));
		}
		private void AccountSecondConditionVertical(int i, int j, int area, NormalVectorDirection normal)
		{
			//	_vector[i] += (int)normal * _problemParametrs.Lamda(area) * _grid.Hy[i] / 6 * (2 * _problemParametrs.DivFuncX1(_grid.X[i], _grid.Y[j], area) + _problemParametrs.DivFuncX1(_grid.X[i], _grid.Y[j + 1], area));
			//	_vector[i + 1] += (int)normal * _problemParametrs.Lamda(area) * _grid.Hy[i] / 6 * (_problemParametrs.DivFuncX1(_grid.X[i], _grid.Y[j], area) + 2 * _problemParametrs.DivFuncX1(_grid.X[i], _grid.Y[j + 1], area));
		}

		enum NormalVectorDirection
		{
			NonCoDirectional = -1,
			CoDirectional = 1,
		}
	}
}