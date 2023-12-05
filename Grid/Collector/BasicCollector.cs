using ReaserchPaper;
using ReaserchPaper.Assemblier;
using ReaserchPaper.Grid;

using ResearchPaper;

using Tensus;

namespace FemProducer.Assemblier
{
	internal class BasicCollector : CollectorBase
	{
		private readonly ICollector _collector;
		private readonly Grid _grid;
		private readonly MatrixFactory _matrixFactory;
		private readonly Slae _slae;
		private readonly ProblemParametrs _problemParametrs;
		private Matrix _matrix;
		private Vector _vector;

		public BasicCollector(ICollector collector, Grid grid, MatrixFactory matrixFactory, ProblemParametrs problemParametrs) : base(grid, matrixFactory, problemParametrs)
		{
			_collector = collector;
			_grid = grid;
			_matrixFactory = matrixFactory;
			_problemParametrs = problemParametrs;

			_matrix = _matrixFactory.CreateMatrix();
			_vector = new Vector(_matrix.Size);
			//Slae slae = new Slae(matrix, vector);
		}

		public Slae Collect(int timeLayer)
		{
			ResetSlae();
			var result = _collector.Collect();
			var matrixes = result.Item1;
			_vector = result.Item2;
			var slae = GetSlae(matrixes[0], matrixes[1]);
			return slae;
		}

		#region to refactory
		//private void SubstractM()
		//{
		//	Master.Slau.A -= _M * timeCoef;
		//}

		//private void ResetSlauOptimized()
		//{
		//	SubstractM();
		//	Master.Slau.b.Reset();
		//}

		private void ResetSlae()
		{
			_matrix.Reset();
			_vector.Reset();
		}

		//public void SwitchTask(Solver solver)
		//{
		//	ResetSlau();
		//	_G.Reset();
		//	_M.Reset();

		//	double[][] localMatrix;
		//	for (int j = 0; j < Grid.M - 1; j++) //y
		//		for (int i = 0; i < Grid.N - 1; i++) //X | проходим по КЭ 
		//		{
		//			if (!IsBorehole(i, j))
		//			{
		//				int area = Grid.GetAreaNumber(i, j);

		//				localMatrix = FEM.GetMassMatrix(Grid.hx[i], Grid.hy[j]);

		//				for (int p = 0; p < 4; p++)
		//					for (int k = 0; k < 4; k++)
		//						localMatrix[p][k] *= Master.Sigma(area);

		//				AddLocalMatrix(_M, localMatrix, i, j);


		//				localMatrix = FEM.GetStiffnessMatrix(Grid.hx[i], Grid.hy[j]);

		//				for (int p = 0; p < 4; p++)
		//					for (int k = 0; k < 4; k++)
		//						localMatrix[p][k] *= Master.Lamda(area);

		//				AddLocalMatrix(_G, localMatrix, i, j);
		//			}
		//		}
		//	SolveSecondTimeLayer(solver);
		//	ResetSlau();
		//	Master.Slau.A += _G + _H;
		//}

		//private void SolveSecondTimeLayer(Solver solver)
		//{
		//	//   double deltaT = Grid.ht[0];//Grid.T[1] - Grid.T[0];

		//	Master.Slau.A = _M * 1 / Grid.ht[0] + _G * Master.Lamda(0) + _H;
		//	for (int j = 0; j < Grid.M - 1; j++)
		//		for (int i = 0; i < Grid.N - 1; i++) // проходим по КЭ 
		//			AddLocalB(i, j, 1);

		//	Master.Slau.b += _M * (1 / Grid.ht[0]) * Master.Slau.q[0];
		//	GetBoundaryConditions(1);

		//	Master.Slau.q[1] = solver.Solve(Master.Slau.A, Master.Slau.b);
		//}

		private Slae GetSlae(Matrix M, Matrix G)
		{
			_matrix = M + G;

			GetBoundaryConditions();

			return new Slae(_matrix, _vector);
		}

		//private void MakeSLau(int timeLayer)
		//{
		//	double deltaT = _grid.Ht[timeLayer - 1] + _grid.Ht[timeLayer - 2];//_grid.T[timeLayer] - _grid.T[timeLayer - 2];
		//	double deltaT1 = _grid.Ht[timeLayer - 2];//_grid.T[timeLayer - 1] - _grid.T[timeLayer - 2];
		//	double deltaT0 = _grid.Ht[timeLayer - 1];//_grid.T[timeLayer] - _grid.T[timeLayer - 1];

		//	Vector vector1 = _M * Master.Slau.q[timeLayer - 2];
		//	Vector vector2 = _M * Master.Slau.q[timeLayer - 1];

		//	var timeCoef = (deltaT + deltaT0) / (deltaT * deltaT0);

		//	vector += -(deltaT0 / (deltaT * deltaT1)) * vector1 + deltaT / (deltaT1 * deltaT0) * vector2;
		//	matrix += _M * timeCoef;

		//	for (int j = 0; j < _grid.M - 1; j++)
		//		for (int i = 0; i < _grid.N - 1; i++) // проходим по КЭ 
		//			AddLocalB(i, j, timeLayer);
		//}

		//private void AddLocalB(Vector vector, int i, int j, int timeLayer)
		//{
		//	var area = _grid.GetAreaNumber(i, j);
		//	vector[i + j * _grid.N] += _grid.Hx[i] * _grid.Hy[j] / 36 * (4 * _problemParametrs.F2(_grid.X[i], _grid.Y[j], _grid.T[timeLayer], area) + 2 * _problemParametrs.F2(_grid.X[i + 1], _grid.Y[j], _grid.T[timeLayer], area) + 2 * _problemParametrs.F2(_grid.X[i], _grid.Y[j + 1], _grid.T[timeLayer], area) + _problemParametrs.F2(_grid.X[i + 1], _grid.Y[j + 1], _grid.T[timeLayer], area));
		//	vector[i + j * _grid.N + 1] += _grid.Hx[i] * _grid.Hy[j] / 36 * (2 * _problemParametrs.F2(_grid.X[i], _grid.Y[j], _grid.T[timeLayer], area) + 4 * _problemParametrs.F2(_grid.X[i + 1], _grid.Y[j], _grid.T[timeLayer], area) + _problemParametrs.F2(_grid.X[i], _grid.Y[j + 1], _grid.T[timeLayer], area) + 2 * _problemParametrs.F2(_grid.X[i + 1], _grid.Y[j + 1], _grid.T[timeLayer], area));
		//	vector[i + (j + 1) * _grid.N] += _grid.Hx[i] * _grid.Hy[j] / 36 * (2 * _problemParametrs.F2(_grid.X[i], _grid.Y[j], _grid.T[timeLayer], area) + _problemParametrs.F2(_grid.X[i + 1], _grid.Y[j], _grid.T[timeLayer], area) + 4 * _problemParametrs.F2(_grid.X[i], _grid.Y[j + 1], _grid.T[timeLayer], area) + 2 * _problemParametrs.F2(_grid.X[i + 1], _grid.Y[j + 1], _grid.T[timeLayer], area));
		//	vector[i + (j + 1) * _grid.N + 1] += _grid.Hx[i] * _grid.Hy[j] / 36 * (_problemParametrs.F2(_grid.X[i], _grid.Y[j], _grid.T[timeLayer], area) + 2 * _problemParametrs.F2(_grid.X[i + 1], _grid.Y[j], _grid.T[timeLayer], area) + 2 * _problemParametrs.F2(_grid.X[i], _grid.Y[j + 1], _grid.T[timeLayer], area) + 4 * _problemParametrs.F2(_grid.X[i + 1], _grid.Y[j + 1], _grid.T[timeLayer], area));
		//}
		//private void GetTimeConditions()
		//{

		//	for (int p = 0; p < 2; p++) //записываем в [1] и в [2] так как потом в цикле вызовется SwapSolves и значение перезапишутся в [0] и [1] соотвественно.     
		//		for (int j = 0; j < _grid.M - 1; j++)
		//			for (int i = 0; i < _grid.N - 1; i++) // проходим по КЭ 
		//			{
		//				var area = _grid.GetAreaNumber(i, j);
		//				Master.Slau.q[p].Elements[i + j * _grid.N] = Master.Func2(_grid.X[i], _grid.Y[j], _grid.T[p], area);
		//				Master.Slau.q[p].Elements[i + j * _grid.N + 1] = Master.Func2(_grid.X[i + 1], _grid.Y[j], _grid.T[p], area);
		//				Master.Slau.q[p].Elements[i + (j + 1) * _grid.N] = Master.Func2(_grid.X[i], _grid.Y[j + 1], _grid.T[p], area);
		//				Master.Slau.q[p].Elements[i + (j + 1) * _grid.N + 1] = Master.Func2(_grid.X[i + 1], _grid.Y[j + 1], _grid.T[p], area);
		//			}
		//}



		private void GetBoundaryConditions()
		{
			int area;
			//нижняя граница
			if (_problemParametrs.boundaryConditions[0] == 1)//первое краевое
				for (int i = 0; i < _grid.N; i++)
				{
					area = _grid.GetAreaNumber(i, 0);
					AccountFirstCondition(i, i, 0, area);
				}
			else //второе краевое            
				for (int i = 0; i < _grid.N - 1; i++)
				{
					area = _grid.GetAreaNumber(i, 0);
					AccountSecondConditionVertical(i, 0, area, NormalVectorDirection.NonCoDirectional);
				}

			if (_problemParametrs.boundaryConditions[2] == 1)//верхняя граница
				for (int i = _matrix.Size - 1; i > _matrix.Size - _grid.N - 1; i--)
				{
					area = _grid.GetAreaNumber(i % _grid.N, _grid.M - 1);
					AccountFirstCondition(i, i % _grid.N, _grid.M - 1, area);
				}
			else
				for (int i = 0; i < _grid.N - 1; i++)
				{
					area = _grid.GetAreaNumber(i, _grid.M - 1);
					AccountSecondConditionVertical(i, _grid.M - 1, area, NormalVectorDirection.CoDirectional);
				}

			if (_problemParametrs.boundaryConditions[3] == 1)//левая гравнь
				for (int i = _grid.N; i < _matrix.Size - _grid.N - 1; i += _grid.N)
				{
					area = _grid.GetAreaNumber(0, i / _grid.N);
					AccountFirstCondition(i, 0, i / _grid.N, area);
				}
			else
				for (int i = 0; i < _grid.N - 1; i++)
				{
					area = _grid.GetAreaNumber(0, i / _grid.N);
					AccountSecondConditionHorizontal(0, i / _grid.N, area, NormalVectorDirection.NonCoDirectional);
				}
			if (_problemParametrs.boundaryConditions[1] == 1)//правая граница
				for (int i = 2 * _grid.N - 1; i < _matrix.Size - 1; i += _grid.N)
				{
					area = _grid.GetAreaNumber(_grid.N - 1, i / _grid.N);
					AccountFirstCondition(i, _grid.N - 1, i / _grid.N, area);
				}
			else
				for (int i = 0; i < _grid.N - 1; i++)
				{
					area = _grid.GetAreaNumber(_grid.N - 1, i);
					AccountSecondConditionHorizontal(_grid.N - 1, i, area, NormalVectorDirection.CoDirectional);
				}

		}
		//public void Collect()
		//{
		//	ResetSlau();
		//	MakeSLau();
		//	GetBoundaryConditions();
		//}


		//public void Collect(int timeLayer)
		//{
		//	ResetSlauOptimized();
		//	MakeSLau(timeLayer);
		//	GetBoundaryConditions(timeLayer);
		//}
		private void AccountFirstCondition(int k, int i, int j, int area)
		{
			_matrix.ZeroingRow(k);
			_matrix.Di[k] = 1;
			_vector[k] = _problemParametrs.Func1(_grid.X[i], _grid.Y[j], area);
		}

		private void AccountSecondConditionHorizontal(int i, int j, int area, NormalVectorDirection normal)
		{
			_vector[i] += (int)normal * _problemParametrs.Lamda(area) * _grid.Hx[i] / 6 * (2 * _problemParametrs.DivFuncY1(_grid.X[i], _grid.Y[j], area) + _problemParametrs.DivFuncY1(_grid.X[i + 1], _grid.Y[j], area));
			_vector[i + 1] += (int)normal * _problemParametrs.Lamda(area) * _grid.Hx[i] / 6 * (_problemParametrs.DivFuncY1(_grid.X[i], _grid.Y[j], area) + 2 * _problemParametrs.DivFuncY1(_grid.X[i + 1], _grid.Y[j], area));
		}
		private void AccountSecondConditionVertical(int i, int j, int area, NormalVectorDirection normal)
		{
			_vector[i] += (int)normal * _problemParametrs.Lamda(area) * _grid.Hy[i] / 6 * (2 * _problemParametrs.DivFuncX1(_grid.X[i], _grid.Y[j], area) + _problemParametrs.DivFuncX1(_grid.X[i], _grid.Y[j + 1], area));
			_vector[i + 1] += (int)normal * _problemParametrs.Lamda(area) * _grid.Hy[i] / 6 * (_problemParametrs.DivFuncX1(_grid.X[i], _grid.Y[j], area) + 2 * _problemParametrs.DivFuncX1(_grid.X[i], _grid.Y[j + 1], area));
		}

		//private void GetBoundaryConditions(int timeLayer)
		//{
		//	int area;
		//	//нижняя граница
		//	if (_problemParametrs.boundaryConditions[0] == 1)//первое краевое
		//		for (int i = 0; i < _grid.N; i++)
		//		{
		//			area = _grid.GetAreaNumber(i, 0);
		//			AccountFirstCondition(i, 0, area, timeLayer);
		//		}
		//	else //второе краевое            
		//		for (int i = 0; i < _grid.N - 1; i++)
		//		{
		//			area = _grid.GetAreaNumber(i, 0);
		//			AccountSecondCondition(i, area, timeLayer, NormalVectorDirection.NonCoDirectional);
		//		}

		//	if (_problemParametrs.boundaryConditions[2] == 1)//верхняя граница
		//		for (int i = _matrix.Size - 1; i > _matrix.Size - _grid.N - 1; i--)
		//		{
		//			area = _grid.GetAreaNumber(i % _grid.N, _grid.M - 1);
		//			AccountFirstCondition(i % _grid.N, _grid.M - 1, area, timeLayer);
		//		}
		//	else
		//		for (int i = 0; i < _grid.N - 1; i++)
		//		{
		//			area = _grid.GetAreaNumber(i, _grid.M - 1);
		//			AccountSecondCondition(i, _grid.M - 1, area, timeLayer, NormalVectorDirection.CoDirectional);
		//		}

		//	if (_problemParametrs.boundaryConditions[3] == 1)//левая гравнь
		//		for (int i = _grid.N; i < Master.Slau.A.Size - _grid.N - 1; i += _grid.N)
		//		{
		//			area = _grid.GetAreaNumber(0, i / _grid.N);
		//			AccountFirstCondition(i, area, timeLayer);
		//		}
		//	else
		//		for (int i = 0; i < _grid.N - 1; i++)
		//		{
		//			area = _grid.GetAreaNumber(0, i);
		//			area = _grid.GetAreaNumber(i, _grid.M - 1);
		//			AccountSecondCondition(i, area, timeLayer, NormalVectorDirection.NonCoDirectional);
		//			Master.Slau.b.Elements[_grid.N * i] -= _grid.hy[i] * Master.Lamda / 6 * (2 * Master.DivFuncX2(_grid.X[0], _grid.Y[i], _grid.T[timeLayer], area) + Master.DivFuncX2(_grid.X[0], _grid.Y[i + 1], _grid.T[timeLayer], area));
		//			Master.Slau.b.Elements[_grid.N * (i + 1)] -= _grid.hy[i] * Master.Lamda / 6 * (Master.DivFuncX2(_grid.X[0], _grid.Y[i], _grid.T[timeLayer], area) + 2 * Master.DivFuncX2(_grid.X[0], _grid.Y[i + 1], _grid.T[timeLayer], area));
		//		}

		//	if (Master.boundaryConditions[1] == 1)//правая граница
		//		for (int i = 2 * _grid.N - 1; i < Master.Slau.A.Size - 1; i += _grid.N)
		//		{
		//			area = _grid.GetAreaNumber(_grid.N - 1, i / _grid.N);
		//			ZeroingRow(i);
		//			Master.Slau.A.di[i] = 1;
		//			Master.Slau.b.Elements[i] = Master.Func2(_grid.X[_grid.N - 1], _grid.Y[i / _grid.N], _grid.T[timeLayer], area);
		//		}
		//    else
		//for (int i = 0; i < _grid.N - 1; i++)
		//{
		//    area = _grid.GetAreaNumber(_grid.N - 1, i);
		//    Master.Slau.b.Elements[_grid.N * (i + 1) - 1] += _grid.hy[i] * Master.Lamda / 6 * (2 * Master.DivFuncX2(_grid.X[_grid.N - 1], _grid.Y[i], _grid.T[timeLayer], area) + Master.DivFuncX2(_grid.X[0], _grid.Y[i + 1], _grid.T[timeLayer], area));
		//    Master.Slau.b.Elements[_grid.N * (i + 2) - 1] += _grid.hy[i] * Master.Lamda / 6 * (Master.DivFuncX2(_grid.X[_grid.N - 1], _grid.Y[i], _grid.T[timeLayer], area) + 2 * Master.DivFuncX2(_grid.X[0], _grid.Y[i + 1], _grid.T[timeLayer], area));
		//}
		//	AccountingBoreholes(timeLayer);
		//}

		//private void AccountingBoreholes(int timeLayer)
		//{
		//	for (int i = 0; i < _grid.boreholes.Length; i++)
		//	{
		//		ZeroingRow(_grid.boreholes[i][1] * _grid.N + _grid.boreholes[i][0]);
		//		ZeroingRow(_grid.boreholes[i][1] * _grid.N + _grid.boreholes[i][0] + 1);
		//		ZeroingRow((_grid.boreholes[i][1] + 1) * _grid.N + _grid.boreholes[i][0]);
		//		ZeroingRow((_grid.boreholes[i][1] + 1) * _grid.N + _grid.boreholes[i][0] + 1);

		//		Master.Slau.A.di[_grid.boreholes[i][1] * _grid.N + _grid.boreholes[i][0]] = 1;
		//		Master.Slau.A.di[_grid.boreholes[i][1] * _grid.N + _grid.boreholes[i][0] + 1] = 1;
		//		Master.Slau.A.di[(_grid.boreholes[i][1] + 1) * _grid.N + _grid.boreholes[i][0]] = 1;
		//		Master.Slau.A.di[(_grid.boreholes[i][1] + 1) * _grid.N + _grid.boreholes[i][0] + 1] = 1;

		//		Master.Slau.b.Elements[_grid.boreholes[i][1] * _grid.N + _grid.boreholes[i][0]] = Master.Func2(_grid.X[_grid.boreholes[i][0]], _grid.Y[_grid.boreholes[i][1]], timeLayer, 0);
		//		Master.Slau.b.Elements[_grid.boreholes[i][1] * _grid.N + _grid.boreholes[i][0] + 1] = Master.Func2(_grid.X[_grid.boreholes[i][0] + 1], _grid.Y[_grid.boreholes[i][1]], timeLayer, 0);
		//		Master.Slau.b.Elements[(_grid.boreholes[i][1] + 1) * _grid.N + _grid.boreholes[i][0]] = Master.Func2(_grid.X[_grid.boreholes[i][0]], _grid.Y[_grid.boreholes[i][1] + 1], timeLayer, 0);
		//		Master.Slau.b.Elements[(_grid.boreholes[i][1] + 1) * _grid.N + _grid.boreholes[i][0] + 1] = Master.Func2(_grid.X[_grid.boreholes[i][0] + 1], _grid.Y[_grid.boreholes[i][1] + 1], timeLayer, 0);

		//	}
		//}
		#endregion

		enum NormalVectorDirection
		{
			NonCoDirectional = -1,
			CoDirectional = 1,
		}
	}
}
