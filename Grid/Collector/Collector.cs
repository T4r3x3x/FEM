using ResearchPaper;

namespace ReaserchPaper.Assemblier
{
	public class Collector : ICollector
	{
		private Matrix _M, _G, _H;
		private double _timeCoef;
		private int _area = 0;

		public Collector(int nodesCount)
		{
			_M = new Matrix(nodesCount);
			_G = new Matrix(nodesCount);
			_H = new Matrix(nodesCount);
			GetMatrixesMG();
			GetTimeConditions();
		}

		/// <summary>
		///  Сборка слау для первой задачи
		/// </summary>
		public void Collect()
		{
			ResetSlau();
			MakeSLau();
			GetBoundaryConditions();
		}


		public void Collect(int timeLayer)
		{
			ResetSlauOptimized();
			MakeSLau(timeLayer);
			GetBoundaryConditions(timeLayer);
		}

		private void SubstractM()
		{
			Master.Slau.A -= _M * timeCoef;
		}

		private void ResetSlauOptimized()
		{
			SubstractM();
			Master.Slau.b.Reset();
		}

		private void ResetSlau()
		{
			Master.Slau.A.Reset();
			Master.Slau.b.Reset();
		}

		public void SwitchTask(Solver solver)
		{
			ResetSlau();
			_G.Reset();
			_M.Reset();

			double[][] localMatrix;
			for (int j = 0; j < Grid.M - 1; j++) //y
				for (int i = 0; i < Grid.N - 1; i++) //X | проходим по КЭ 
				{
					if (!IsBorehole(i, j))
					{
						int area = Grid.GetAreaNumber(i, j);

						localMatrix = FEM.GetMassMatrix(Grid.hx[i], Grid.hy[j]);

						for (int p = 0; p < 4; p++)
							for (int k = 0; k < 4; k++)
								localMatrix[p][k] *= Master.Sigma(area);

						AddLocalMatrix(_M, localMatrix, i, j);


						localMatrix = FEM.GetStiffnessMatrix(Grid.hx[i], Grid.hy[j]);

						for (int p = 0; p < 4; p++)
							for (int k = 0; k < 4; k++)
								localMatrix[p][k] *= Master.Lamda(area);

						AddLocalMatrix(_G, localMatrix, i, j);
					}
				}
			SolveSecondTimeLayer(solver);
			ResetSlau();
			Master.Slau.A += _G + _H;
		}

		private void SolveSecondTimeLayer(Solver solver)
		{
			//   double deltaT = Grid.ht[0];//Grid.T[1] - Grid.T[0];

			Master.Slau.A = _M * 1 / Grid.ht[0] + _G * Master.Lamda(0) + _H;
			for (int j = 0; j < Grid.M - 1; j++)
				for (int i = 0; i < Grid.N - 1; i++) // проходим по КЭ 
					AddLocalB(i, j, 1);

			Master.Slau.b += _M * (1 / Grid.ht[0]) * Master.Slau.q[0];
			GetBoundaryConditions(1);

			Master.Slau.q[1] = solver.Solve(Master.Slau.A, Master.Slau.b);
		}

		private void GetMatrixesMG()
		{
			double[][] localMatrix;
			for (int j = 0; j < Grid.M - 1; j++) //y
				for (int i = 0; i < Grid.N - 1; i++) //X | проходим по КЭ 
				{
					if (!IsBorehole(i, j))
					{
						int area = Grid.GetAreaNumber(i, j);

						localMatrix = FEM.GetMassMatrix(Grid.hx[i], Grid.hy[j]);

						for (int p = 0; p < 4; p++)
							for (int k = 0; k < 4; k++)
								localMatrix[p][k] *= Master.Gamma(area);

						AddLocalMatrix(_M, localMatrix, i, j);



						localMatrix = FEM.GetStiffnessMatrix(Grid.hx[i], Grid.hy[j]);

						for (int p = 0; p < 4; p++)
							for (int k = 0; k < 4; k++)
								localMatrix[p][k] *= Master.Lamda(area);

						AddLocalMatrix(_G, localMatrix, i, j);
					}
				}
		}
		public void GetMatrixH()
		{
			double[][] localMatrix;
			for (int j = 0; j < Grid.M - 1; j++)
				for (int i = 0; i < Grid.N - 1; i++) // проходим по КЭ 
				{
					if (!IsBorehole(i, j))
					{
						localMatrix = GetGradTMatrix(LocalNumToGlobal(i, j, 0), Grid.hx[i], Grid.hy[j], Grid.X[i], Grid.y[j]);
						AddLocalMatrix(_H, localMatrix, i, j);
					}
				}
		}

		public int LocalNumToGlobal(int i, int j, int k)
		{
			return i + j * Grid.N + k / 2 * Grid.N + k % 2;
		}
		private void AddLocalMatrix(Matrix matrix, double[][] localMatrix, int i, int j)
		{
			for (int p = 0; p < 4; p++)
			{
				matrix.di[LocalNumToGlobal(i, j, p)] += localMatrix[p][p];

				int ibeg = matrix.ia[LocalNumToGlobal(i, j, p)];
				int iend = matrix.ia[LocalNumToGlobal(i, j, p) + 1];
				for (int k = 0; k < p; k++)
				{
					int index = BinarySearch(matrix.ja, LocalNumToGlobal(i, j, k), ibeg, iend - 1);

					matrix.au[index] += localMatrix[k][p];
					matrix.al[index] += localMatrix[p][k];
					ibeg++;
				}
			}
		}
		private int BinarySearch(List<int> list, int value, int l, int r)
		{
			while (l != r)
			{
				int mid = (l + r) / 2 + 1;

				if (list[mid] > value)
					r = mid - 1;
				else
					l = mid;
			}

			return list[l] == value ? l : -1;
		}
		private void GetTimeConditions()
		{

			for (int p = 0; p < 2; p++) //записываем в [1] и в [2] так как потом в цикле вызовется SwapSolves и значение перезапишутся в [0] и [1] соотвественно.     
				for (int j = 0; j < Grid.M - 1; j++)
					for (int i = 0; i < Grid.N - 1; i++) // проходим по КЭ 
					{
						area = Grid.GetAreaNumber(i, j);
						Master.Slau.q[p].Elements[i + j * Grid.N] = Master.Func2(Grid.X[i], Grid.y[j], Grid.T[p], area);
						Master.Slau.q[p].Elements[i + j * Grid.N + 1] = Master.Func2(Grid.X[i + 1], Grid.y[j], Grid.T[p], area);
						Master.Slau.q[p].Elements[i + (j + 1) * Grid.N] = Master.Func2(Grid.X[i], Grid.y[j + 1], Grid.T[p], area);
						Master.Slau.q[p].Elements[i + (j + 1) * Grid.N + 1] = Master.Func2(Grid.X[i + 1], Grid.y[j + 1], Grid.T[p], area);
					}
		}


		private void MakeSLau()
		{
			Master.Slau.A += _M + _G;

			for (int j = 0; j < Grid.M - 1; j++)
				for (int i = 0; i < Grid.N - 1; i++) // проходим по КЭ 
					AddLocalB(i, j);
		}

		private void MakeSLau(int timeLayer)
		{
			double deltaT = Grid.ht[timeLayer - 1] + Grid.ht[timeLayer - 2];//Grid.T[timeLayer] - Grid.T[timeLayer - 2];
			double deltaT1 = Grid.ht[timeLayer - 2];//Grid.T[timeLayer - 1] - Grid.T[timeLayer - 2];
			double deltaT0 = Grid.ht[timeLayer - 1];//Grid.T[timeLayer] - Grid.T[timeLayer - 1];

			Vector vector1 = _M * Master.Slau.q[timeLayer - 2];
			Vector vector2 = _M * Master.Slau.q[timeLayer - 1];

			timeCoef = (deltaT + deltaT0) / (deltaT * deltaT0);

			Master.Slau.b += -(deltaT0 / (deltaT * deltaT1)) * vector1 + deltaT / (deltaT1 * deltaT0) * vector2;
			Master.Slau.A += _M * timeCoef;

			for (int j = 0; j < Grid.M - 1; j++)
				for (int i = 0; i < Grid.N - 1; i++) // проходим по КЭ 
					AddLocalB(i, j, timeLayer);
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
		private void ZeroingRow(int row)
		{
			for (int i = Master.Slau.A.ia[row]; i < Master.Slau.A.ia[row + 1]; i++)
				Master.Slau.A.al[i] = 0;

			for (int j = row + 1; j < Master.Slau.A.Size; j++)
			{
				int jbeg = Master.Slau.A.ia[j];
				int jend = Master.Slau.A.ia[j + 1];
				int index = BinarySearch(Master.Slau.A.ja, row, jbeg, jend - 1);
				if (index != -1)
					Master.Slau.A.au[index] = 0;
			}
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

		private void AccountingBoreholes(int timeLayer)
		{
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

				Master.Slau.b.Elements[Grid.boreholes[i][1] * Grid.N + Grid.boreholes[i][0]] = Master.Func2(Grid.X[Grid.boreholes[i][0]], Grid.y[Grid.boreholes[i][1]], timeLayer, 0);
				Master.Slau.b.Elements[Grid.boreholes[i][1] * Grid.N + Grid.boreholes[i][0] + 1] = Master.Func2(Grid.X[Grid.boreholes[i][0] + 1], Grid.y[Grid.boreholes[i][1]], timeLayer, 0);
				Master.Slau.b.Elements[(Grid.boreholes[i][1] + 1) * Grid.N + Grid.boreholes[i][0]] = Master.Func2(Grid.X[Grid.boreholes[i][0]], Grid.y[Grid.boreholes[i][1] + 1], timeLayer, 0);
				Master.Slau.b.Elements[(Grid.boreholes[i][1] + 1) * Grid.N + Grid.boreholes[i][0] + 1] = Master.Func2(Grid.X[Grid.boreholes[i][0] + 1], Grid.y[Grid.boreholes[i][1] + 1], timeLayer, 0);

			}
		}
		private double[][] GetGradTMatrix(int elemNumber, double hx, double hy, double xLeft, double yLower)
		{
			double[][] matrix = new double[4][];
			Point xBoundaries = new Point(xLeft, xLeft + hx);
			Point yBoundaries = new Point(yLower, yLower + hy);

			for (int i = 0; i < 4; i++)
			{
				matrix[i] = new double[4];
				for (int j = 0; j < 4; j++)
					matrix[i][j] = NumericalMethods.GaussIntegration(elemNumber, i, j, xBoundaries, yBoundaries, hx, hy);
			}

			return matrix;
		}

		public IList<Matrix> Collect(Grid.Grid grid) => throw new NotImplementedException();
	}
}