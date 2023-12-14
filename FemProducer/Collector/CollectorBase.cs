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
		private readonly LinearQuadrangularCartesianBasis _basis;

		public CollectorBase(GridModel grid, MatrixFactory matrixFactory, ProblemService problemParametrs, LinearQuadrangularCartesianBasis basis)
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
				var i = element.NodesIndexes[0];
				var j = element.NodesIndexes[1];

				var hx = _grid.Nodes[j].X - _grid.Nodes[i].X;
				var hy = _grid.Nodes[element.NodesIndexes[2]].Y - _grid.Nodes[i].Y;

				int area = 0;// _grid.GetAreaNumber(i, j);

				var nodes = _grid.ElementToNode(element);

				var localMatrix = _basis.GetMassMatrix(nodes);
				localMatrix.MultiplyLocalMatrix(_problemParametrs.Gamma(area));
				AddLocalMatrix(M, localMatrix, element);

				localMatrix = _basis.GetStiffnessMatrix(nodes);
				localMatrix.MultiplyLocalMatrix(_problemParametrs.Lamda(area));
				AddLocalMatrix(G, localMatrix, element);

				var localVector = _basis.GetLocalVector(nodes, _problemParametrs.F1);
				AddLocalVector(vector, localVector, element);
				//		AddLocalVector(vector, hx, hy, 0, element);
			}

			Dictionary<string, Matrix> matrixes = new() { { "M", M }, { "G", G } };
			return (matrixes, vector);

		}

		//public void GetMatrixH()
		//{
		//	double[][] localMatrix;
		//	for (int j = 0; j < _grid.M - 1; j++)
		//		for (int i = 0; i < _grid.N - 1; i++) // проходим по КЭ 
		//			if (!_grid.IsBorehole(i, j))
		//			{
		//				localMatrix = GetGradTMatrix(_grid.LocalNumToGlobal(i, j, 0), _grid.Hx[i], _grid.Hy[j], _grid.X[i], _grid.Y[j]);
		//				AddLocalMatrix(_H, localMatrix, i, j);
		//			}
		//}

		private void AddLocalVector(Vector vector, double hx, double hy, int area, FiniteElement element)
		{
			lock (_lock)
			{
				//for (int i = 0; i < localVector.Count; i++)
				//{
				//	vector[element.NodesIndexes[i]] += localVector[i];
				//}

				var x1 = _grid.Nodes[element.NodesIndexes[0]].X;
				var x2 = _grid.Nodes[element.NodesIndexes[1]].X;
				var y1 = _grid.Nodes[element.NodesIndexes[0]].Y;
				var y2 = _grid.Nodes[element.NodesIndexes[2]].Y;

				var a = hx * hy / 36 * (4 * _problemParametrs.F1(x1, y1, area) + 2 * _problemParametrs.F1(x2, y1, area) + 2 * _problemParametrs.F1(x1, y2, area) + _problemParametrs.F1(x2, y2, area));
				vector[element.NodesIndexes[0]] += hx * hy / 36 * (4 * _problemParametrs.F1(x1, y1, area) + 2 * _problemParametrs.F1(x2, y1, area) + 2 * _problemParametrs.F1(x1, y2, area) + _problemParametrs.F1(x2, y2, area));
				vector[element.NodesIndexes[1]] += hx * hy / 36 * (2 * _problemParametrs.F1(x1, y1, area) + 4 * _problemParametrs.F1(x2, y1, area) + _problemParametrs.F1(x1, y2, area) + 2 * _problemParametrs.F1(x2, y2, area));
				vector[element.NodesIndexes[2]] += hx * hy / 36 * (2 * _problemParametrs.F1(x1, y1, area) + _problemParametrs.F1(x2, y1, area) + 4 * _problemParametrs.F1(x1, y2, area) + 2 * _problemParametrs.F1(x2, y2, area));
				vector[element.NodesIndexes[3]] += hx * hy / 36 * (_problemParametrs.F1(x1, y1, area) + 2 * _problemParametrs.F1(x2, y1, area) + 2 * _problemParametrs.F1(x1, y2, area) + 4 * _problemParametrs.F1(x2, y2, area));
			}
		}

		private void AddLocalVector(Vector vector, IList<double> localVector, FiniteElement element)
		{
			lock (_lock)
			{
				for (int i = 0; i < localVector.Count; i++)
				{
					vector[element.NodesIndexes[i]] += localVector[i];
				}

				//var x1 = _grid.Nodes[element.NodesIndexes[0]].X;
				//var x2 = _grid.Nodes[element.NodesIndexes[1]].X;
				//var y1 = _grid.Nodes[element.NodesIndexes[0]].Y;
				//var y2 = _grid.Nodes[element.NodesIndexes[2]].Y;

				//vector[element.NodesIndexes[0]] += hx * hy / 36 * (4 * _problemParametrs.F1(x1, y1, area) + 2 * _problemParametrs.F1(x2, y1, area) + 2 * _problemParametrs.F1(x1, y2, area) + _problemParametrs.F1(x2, y2, area));
				//vector[element.NodesIndexes[1]] += hx * hy / 36 * (2 * _problemParametrs.F1(x1, y1, area) + 4 * _problemParametrs.F1(x2, y1, area) + _problemParametrs.F1(x1, y2, area) + 2 * _problemParametrs.F1(x2, y2, area));
				//vector[element.NodesIndexes[2]] += hx * hy / 36 * (2 * _problemParametrs.F1(x1, y1, area) + _problemParametrs.F1(x2, y1, area) + 4 * _problemParametrs.F1(x1, y2, area) + 2 * _problemParametrs.F1(x2, y2, area));
				//vector[element.NodesIndexes[3]] += hx * hy / 36 * (_problemParametrs.F1(x1, y1, area) + 2 * _problemParametrs.F1(x2, y1, area) + 2 * _problemParametrs.F1(x1, y2, area) + 4 * _problemParametrs.F1(x2, y2, area));
			}
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


		//private double[][] GetGradTMatrix(int elemNumber, double hx, double hy, double xLeft, double yLower)
		//{
		//	double[][] matrix = new double[4][];
		//	Point xBoundaries = new Point(xLeft, xLeft + hx);
		//	Point yBoundaries = new Point(yLower, yLower + hy);

		//	for (int i = 0; i < 4; i++)
		//	{
		//		matrix[i] = new double[4];
		//		for (int j = 0; j < 4; j++)
		//			matrix[i][j] = NumericalMethods.GaussIntegration(elemNumber, i, j, xBoundaries, yBoundaries, hx, hy);
		//	}

		//	return matrix;
		//}
	}
}