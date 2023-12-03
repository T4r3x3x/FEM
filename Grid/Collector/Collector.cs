using FemProducer;

using ResearchPaper;

using Tensus;

namespace ReaserchPaper.Assemblier
{
	public class Collector : ICollector
	{
		private readonly Grid.Grid _grid;
		private readonly MatrixFactory _matrixFactory;

		public Collector(Grid.Grid grid, MatrixFactory matrixFactory)
		{
			_grid = grid;
			_matrixFactory = matrixFactory;
		}

		public IList<Matrix> Collect()
		{
			return GetMatrixesMG();
		}


		private void GetBoundaryConditions()
		{
			//нижняя граница
			if (Master.boundaryConditions[0] == 1)//первое краевое
				for (int i = 0; i < Grid.N; i++)
				{
					area = Grid.GetAreaNumber(i, 0);
					ZeroingRow(i);
					Master.Slau.A.di[i] = 1;
					Master.Slau.b.Elements[i] = Master.Func1(Grid.X[i], Grid.y[0], area);
				}
			//else //второе краевое            
			//    for (int i = 0; i < Grid.N - 1; i++)
			//    {
			//        area = Grid.GetAreaNumber(i, 0);
			//        Master.Slau.b.Elements[i] -= Master.Lamda * Grid.hx[i] / 6 * (2 * Master.DivFuncY1(Grid.X[i], Grid.y[0], area) + Master.DivFuncY1(Grid.X[i + 1], Grid.y[0], area));
			//        Master.Slau.b.Elements[i + 1] -= Master.Lamda * Grid.hx[i] / 6 * (2 * Master.DivFuncY1(Grid.X[i], Grid.y[0], area) + Master.DivFuncY1(Grid.X[i + 1], Grid.y[0], area));
			//    }

			if (Master.boundaryConditions[2] == 1)//верхняя граница
				for (int i = Master.Slau.A.Size - 1; i > Master.Slau.A.Size - Grid.N - 1; i--)
				{
					area = Grid.GetAreaNumber(i % Grid.N, Grid.M - 1);
					ZeroingRow(i);
					Master.Slau.A.di[i] = 1;
					Master.Slau.b.Elements[i] = Master.Func1(Grid.X[i % Grid.N], Grid.y[Grid.M - 1], area);
				}
			// else
			//for (int i = 0; i < Grid.N - 1; i++)
			//{
			//    area = Grid.GetAreaNumber(i, Grid.M - 1);
			//    Master.Slau.b.Elements[Grid.N * (Grid.M - 1) + i] += Master.Lamda * Grid.hx[i] / 6 * (2 * Master.DivFuncY1(Grid.X[i], Grid.y[Grid.M - 1], area) + Master.DivFuncY1(Grid.X[i + 1], Grid.y[Grid.M - 1], area));
			//    Master.Slau.b.Elements[Grid.N * (Grid.M - 1) + i + 1] += Master.Lamda * Grid.hx[i] / 6 * (Master.DivFuncY1(Grid.X[i], Grid.y[Grid.M - 1], area) + 2 * Master.DivFuncY1(Grid.X[i + 1], Grid.y[Grid.M - 1], area));
			//}

			if (Master.boundaryConditions[3] == 1)//левая гравнь
				for (int i = Grid.N; i < Master.Slau.A.Size - Grid.N - 1; i += Grid.N)
				{
					area = Grid.GetAreaNumber(0, i / Grid.N);
					ZeroingRow(i);
					Master.Slau.A.di[i] = 1;
					Master.Slau.b.Elements[i] = Master.Func1(Grid.X[0], Grid.y[i / Grid.N], area);
				}
			//  else
			//for (int i = 0; i < Grid.N - 1; i++)
			//{
			//    area = Grid.GetAreaNumber(0, i);
			//    Master.Slau.b.Elements[Grid.N * i] -= Grid.hy[i] * Master.Lamda / 6 * (2 * Master.DivFuncX1(Grid.X[0], Grid.y[i], area) + Master.DivFuncX1(Grid.X[0], Grid.y[i + 1], area));
			//    Master.Slau.b.Elements[Grid.N * (i + 1)] -= Grid.hy[i] * Master.Lamda / 6 * (Master.DivFuncX1(Grid.X[0], Grid.y[i], area) + 2 * Master.DivFuncX1(Grid.X[0], Grid.y[i + 1], area));
			//}

			if (Master.boundaryConditions[1] == 1)//правая граница
				for (int i = 2 * Grid.N - 1; i < Master.Slau.A.Size - 1; i += Grid.N)
				{
					area = Grid.GetAreaNumber(Grid.N - 1, i / Grid.N);
					ZeroingRow(i);
					Master.Slau.A.di[i] = 1;
					Master.Slau.b.Elements[i] = Master.Func1(Grid.X[Grid.N - 1], Grid.y[i / Grid.N], area);
				}
			//   else
			//for (int i = 0; i < Grid.N - 1; i++)
			//{
			//    area = Grid.GetAreaNumber(Grid.N - 1, i);
			//    Master.Slau.b.Elements[Grid.N * (i + 1) - 1] += Grid.hy[i] * Master.Lamda / 6 * (2 * Master.DivFuncX1(Grid.X[Grid.N - 1], Grid.y[i], area) + Master.DivFuncX1(Grid.X[0], Grid.y[i + 1], area));
			//    Master.Slau.b.Elements[Grid.N * (i + 2) - 1] += Grid.hy[i] * Master.Lamda / 6 * (Master.DivFuncX1(Grid.X[Grid.N - 1], Grid.y[i], area) + 2 * Master.DivFuncX1(Grid.X[0], Grid.y[i + 1], area));
			//}
			for (int i = 0; i < Grid.boreholes.Length; i++)
			{
				ZeroingRow(Grid.boreholes[i][1] * Grid.N + Grid.boreholes[i][0]);
				ZeroingRow(Grid.boreholes[i][1] * Grid.N + Grid.boreholes[i][0] + 1);
				ZeroingRow((Grid.boreholes[i][1] + 1) * Grid.N + Grid.boreholes[i][0]);
				ZeroingRow((Grid.boreholes[i][1] + 1) * Grid.N + Grid.boreholes[i][0] + 1);

				Master.Slau.A.di[Grid.boreholes[i][1] * Grid.N + Grid.boreholes[i][0]] = 1;
				Master.Slau.A.di[Grid.boreholes[i][1] * Grid.N + Grid.boreholes[i][0] + 1] = 1;
				Master.Slau.A.di[(Grid.boreholes[i][1] + 1) * Grid.N + Grid.boreholes[i][0]] = 1;
				Master.Slau.A.di[(Grid.boreholes[i][1] + 1) * Grid.N + Grid.boreholes[i][0] + 1] = 1;

				Master.Slau.b.Elements[Grid.boreholes[i][1] * Grid.N + Grid.boreholes[i][0]] = Master.Func1(Grid.X[Grid.boreholes[i][0]], Grid.y[Grid.boreholes[i][1]], 0);
				Master.Slau.b.Elements[Grid.boreholes[i][1] * Grid.N + Grid.boreholes[i][0] + 1] = Master.Func1(Grid.X[Grid.boreholes[i][0] + 1], Grid.y[Grid.boreholes[i][1]], 0);
				Master.Slau.b.Elements[(Grid.boreholes[i][1] + 1) * Grid.N + Grid.boreholes[i][0]] = Master.Func1(Grid.X[Grid.boreholes[i][0]], Grid.y[Grid.boreholes[i][1] + 1], 0);
				Master.Slau.b.Elements[(Grid.boreholes[i][1] + 1) * Grid.N + Grid.boreholes[i][0] + 1] = Master.Func1(Grid.X[Grid.boreholes[i][0] + 1], Grid.y[Grid.boreholes[i][1] + 1], 0);
			}
		}
		private void GetBoundaryConditions(int timeLayer)
		{
			//нижняя граница
			if (Master.boundaryConditions[0] == 1)//первое краевое
				for (int i = 0; i < Grid.N; i++)
				{
					area = Grid.GetAreaNumber(i, 0);
					ZeroingRow(i);
					Master.Slau.A.di[i] = 1;
					Master.Slau.b.Elements[i] = Master.Func2(Grid.X[i], Grid.y[0], Grid.T[timeLayer], area);
					//  Master.Slau.A.di[i] = C;
					//   Master.Slau.b.Elements[i] = C * Master.Func2(Grid.X[i], Grid.y[0], Grid.T[timeLayer]);
				}
			//  else //второе краевое            
			//for (int i = 0; i < Grid.N - 1; i++)
			//{
			//    area = Grid.GetAreaNumber(i, 0);
			//    Master.Slau.b.Elements[i] -= Master.Lamda * Grid.hx[i] / 6 * (2 * Master.DivFuncY2(Grid.X[i], Grid.y[0], Grid.T[timeLayer],area  ) + Master.DivFuncY2(Grid.X[i + 1], Grid.y[0], Grid.T[timeLayer], area));
			//    Master.Slau.b.Elements[i + 1] -= Master.Lamda * Grid.hx[i] / 6 * (Master.DivFuncY2(Grid.X[i], Grid.y[0], Grid.T[timeLayer], area) + 2 * Master.DivFuncY2(Grid.X[i + 1], Grid.y[0], Grid.T[timeLayer], area));
			//}

			if (Master.boundaryConditions[2] == 1)//верхняя граница
				for (int i = Master.Slau.A.Size - 1; i > Master.Slau.A.Size - Grid.N - 1; i--)
				{
					area = Grid.GetAreaNumber(i % Grid.N, Grid.M - 1);
					ZeroingRow(i);
					Master.Slau.A.di[i] = 1;
					Master.Slau.b.Elements[i] = Master.Func2(Grid.X[i % Grid.N], Grid.y[Grid.M - 1], Grid.T[timeLayer], area);
				}
			//   else
			//for (int i = 0; i < Grid.N - 1; i++)
			//{
			//    area = Grid.GetAreaNumber(i, Grid.M - 1);
			//    Master.Slau.b.Elements[Grid.N * (Grid.M - 1) + i] += Master.Lamda * Grid.hx[i] / 6 * (2 * Master.DivFuncY2(Grid.X[i], Grid.y[Grid.M - 1], Grid.T[timeLayer] , area) + Master.DivFuncY2(Grid.X[i + 1], Grid.y[Grid.M - 1], Grid.T[timeLayer], area));
			//    Master.Slau.b.Elements[Grid.N * (Grid.M - 1) + i + 1] += Master.Lamda * Grid.hx[i] / 6 * (Master.DivFuncY2(Grid.X[i], Grid.y[Grid.M - 1], Grid.T[timeLayer], area) + 2 * Master.DivFuncY2(Grid.X[i + 1], Grid.y[Grid.M - 1], Grid.T[timeLayer], area));
			//}

			if (Master.boundaryConditions[3] == 1)//левая гравнь
				for (int i = Grid.N; i < Master.Slau.A.Size - Grid.N - 1; i += Grid.N)
				{
					area = Grid.GetAreaNumber(0, i / Grid.N);
					ZeroingRow(i);
					Master.Slau.A.di[i] = 1;
					Master.Slau.b.Elements[i] = Master.Func2(Grid.X[0], Grid.y[i / Grid.N], Grid.T[timeLayer], area);
				}
			//   else
			//for (int i = 0; i < Grid.N - 1; i++)
			//{
			//    area = Grid.GetAreaNumber(0, i);
			//    Master.Slau.b.Elements[Grid.N * i] -= Grid.hy[i] * Master.Lamda / 6 * (2 * Master.DivFuncX2(Grid.X[0], Grid.y[i], Grid.T[timeLayer], area) + Master.DivFuncX2(Grid.X[0], Grid.y[i + 1], Grid.T[timeLayer], area));
			//    Master.Slau.b.Elements[Grid.N * (i + 1)] -= Grid.hy[i] * Master.Lamda / 6 * (Master.DivFuncX2(Grid.X[0], Grid.y[i], Grid.T[timeLayer]   , area) + 2 * Master.DivFuncX2(Grid.X[0], Grid.y[i + 1], Grid.T[timeLayer], area));
			//}

			if (Master.boundaryConditions[1] == 1)//правая граница
				for (int i = 2 * Grid.N - 1; i < Master.Slau.A.Size - 1; i += Grid.N)
				{
					area = Grid.GetAreaNumber(Grid.N - 1, i / Grid.N);
					ZeroingRow(i);
					Master.Slau.A.di[i] = 1;
					Master.Slau.b.Elements[i] = Master.Func2(Grid.X[Grid.N - 1], Grid.y[i / Grid.N], Grid.T[timeLayer], area);
				}
			//    else
			//for (int i = 0; i < Grid.N - 1; i++)
			//{
			//    area = Grid.GetAreaNumber(Grid.N - 1, i);
			//    Master.Slau.b.Elements[Grid.N * (i + 1) - 1] += Grid.hy[i] * Master.Lamda / 6 * (2 * Master.DivFuncX2(Grid.X[Grid.N - 1], Grid.y[i], Grid.T[timeLayer], area) + Master.DivFuncX2(Grid.X[0], Grid.y[i + 1], Grid.T[timeLayer], area));
			//    Master.Slau.b.Elements[Grid.N * (i + 2) - 1] += Grid.hy[i] * Master.Lamda / 6 * (Master.DivFuncX2(Grid.X[Grid.N - 1], Grid.y[i], Grid.T[timeLayer], area) + 2 * Master.DivFuncX2(Grid.X[0], Grid.y[i + 1], Grid.T[timeLayer], area));
			//}
			AccountingBoreholes(timeLayer);
		}
		private IList<Matrix> GetMatrixesMG()
		{
			double[][] localMatrix;
			Matrix M = _matrixFactory.CreateMatrix();
			Matrix G = _matrixFactory.CreateMatrix();
			for (int j = 0; j < _grid.M - 1; j++) //y
				for (int i = 0; i < _grid.N - 1; i++) //X | проходим по КЭ 
					if (!_grid.IsBorehole(i, j))
					{
						int area = _grid.GetAreaNumber(i, j);

						localMatrix = FEM.GetMassMatrix(_grid.Hx[i], _grid.Hy[j]);
						AddLocalMatrix(M, localMatrix, i, j);

						localMatrix = FEM.GetStiffnessMatrix(_grid.Hx[i], _grid.Hy[j]);
						AddLocalMatrix(G, localMatrix, i, j);
					}
			return new Matrix[] { M, G };
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

		private void AddLocalMatrix(Matrix matrix, double[][] localMatrix, int i, int j)
		{
			for (int p = 0; p < 4; p++)
			{
				matrix.Di[_grid.LocalNumToGlobal(i, j, p)] += localMatrix[p][p];

				int ibeg = matrix.Ia[_grid.LocalNumToGlobal(i, j, p)];
				int iend = matrix.Ia[_grid.LocalNumToGlobal(i, j, p) + 1];
				for (int k = 0; k < p; k++)
				{
					int index = Tools.BinarySearch(matrix.Ja, _grid.LocalNumToGlobal(i, j, k), ibeg, iend - 1);

					matrix.Au[index] += localMatrix[k][p];
					matrix.Al[index] += localMatrix[p][k];
					ibeg++;
				}
			}
		}
		private void AddLocalB(int i, int j)
		{
			area = Grid.GetAreaNumber(i, j);
			Master.Slau.b.Elements[i + j * Grid.N] += Grid.hx[i] * Grid.hy[j] / 36 * (4 * Master.F1(Grid.X[i], Grid.y[j], area) + 2 * Master.F1(Grid.X[i + 1], Grid.y[j], area) + 2 * Master.F1(Grid.X[i], Grid.y[j + 1], area) + Master.F1(Grid.X[i + 1], Grid.y[j + 1], area));
			Master.Slau.b.Elements[i + j * Grid.N + 1] += Grid.hx[i] * Grid.hy[j] / 36 * (2 * Master.F1(Grid.X[i], Grid.y[j], area) + 4 * Master.F1(Grid.X[i + 1], Grid.y[j], area) + Master.F1(Grid.X[i], Grid.y[j + 1], area) + 2 * Master.F1(Grid.X[i + 1], Grid.y[j + 1], area));
			Master.Slau.b.Elements[i + (j + 1) * Grid.N] += Grid.hx[i] * Grid.hy[j] / 36 * (2 * Master.F1(Grid.X[i], Grid.y[j], area) + Master.F1(Grid.X[i + 1], Grid.y[j], area) + 4 * Master.F1(Grid.X[i], Grid.y[j + 1], area) + 2 * Master.F1(Grid.X[i + 1], Grid.y[j + 1], area));
			Master.Slau.b.Elements[i + (j + 1) * Grid.N + 1] += Grid.hx[i] * Grid.hy[j] / 36 * (Master.F1(Grid.X[i], Grid.y[j], area) + 2 * Master.F1(Grid.X[i + 1], Grid.y[j], area) + 2 * Master.F1(Grid.X[i], Grid.y[j + 1], area) + 4 * Master.F1(Grid.X[i + 1], Grid.y[j + 1], area));
		}
		private void AddLocalB(int i, int j, int timeLayer)
		{
			area = Grid.GetAreaNumber(i, j);
			Master.Slau.b.Elements[i + j * Grid.N] += Grid.hx[i] * Grid.hy[j] / 36 * (4 * Master.F2(Grid.X[i], Grid.y[j], Grid.T[timeLayer], area) + 2 * Master.F2(Grid.X[i + 1], Grid.y[j], Grid.T[timeLayer], area) + 2 * Master.F2(Grid.X[i], Grid.y[j + 1], Grid.T[timeLayer], area) + Master.F2(Grid.X[i + 1], Grid.y[j + 1], Grid.T[timeLayer], area));
			Master.Slau.b.Elements[i + j * Grid.N + 1] += Grid.hx[i] * Grid.hy[j] / 36 * (2 * Master.F2(Grid.X[i], Grid.y[j], Grid.T[timeLayer], area) + 4 * Master.F2(Grid.X[i + 1], Grid.y[j], Grid.T[timeLayer], area) + Master.F2(Grid.X[i], Grid.y[j + 1], Grid.T[timeLayer], area) + 2 * Master.F2(Grid.X[i + 1], Grid.y[j + 1], Grid.T[timeLayer], area));
			Master.Slau.b.Elements[i + (j + 1) * Grid.N] += Grid.hx[i] * Grid.hy[j] / 36 * (2 * Master.F2(Grid.X[i], Grid.y[j], Grid.T[timeLayer], area) + Master.F2(Grid.X[i + 1], Grid.y[j], Grid.T[timeLayer], area) + 4 * Master.F2(Grid.X[i], Grid.y[j + 1], Grid.T[timeLayer], area) + 2 * Master.F2(Grid.X[i + 1], Grid.y[j + 1], Grid.T[timeLayer], area));
			Master.Slau.b.Elements[i + (j + 1) * Grid.N + 1] += Grid.hx[i] * Grid.hy[j] / 36 * (Master.F2(Grid.X[i], Grid.y[j], Grid.T[timeLayer], area) + 2 * Master.F2(Grid.X[i + 1], Grid.y[j], Grid.T[timeLayer], area) + 2 * Master.F2(Grid.X[i], Grid.y[j + 1], Grid.T[timeLayer], area) + 4 * Master.F2(Grid.X[i + 1], Grid.y[j + 1], Grid.T[timeLayer], area));
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