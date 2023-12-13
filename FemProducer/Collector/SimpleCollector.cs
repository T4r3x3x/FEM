using FemProducer.MatrixBuilding;

using Grid.Models;

using MathModels.Models;

namespace FemProducer.Collector
{
	internal class SimpleCollector : AbstractCollector
	{
		private readonly GridModel _grid;
		private readonly MatrixFactory _matrixFactory;
		private readonly ProblemService _problemParametrs;
		private Matrix _matrix;
		private Vector _vector;

		public SimpleCollector(ICollectorBase collector, GridModel grid, MatrixFactory matrixFactory, ProblemService problemParametrs) : base(collector)
		{
			_grid = grid;
			_matrixFactory = matrixFactory;
			_problemParametrs = problemParametrs;

			_matrix = _matrixFactory.CreateMatrix(grid);
			_vector = new Vector(_matrix.Size);
			//Slae slae = new Slae(matrix, vector);
		}

		public override Slae Collect(int timeLayer)
		{
			ResetSlae();
			var result = _collectorBase.Collect();
			var matrixes = result.Item1;
			_vector = result.Item2;
			var slae = GetSlae(matrixes.GetValueOrDefault("M"), matrixes.GetValueOrDefault("G"));
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
			Console.WriteLine("Done with boundary " + DateTime.UtcNow);
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
			Parallel.ForEach(_grid.FirstBoundaryNodes, boundaryNodeIndex =>
				{
					AccountFirstCondition(_grid.Nodes[boundaryNodeIndex], boundaryNodeIndex);
				});

		}

		private void AccountFirstCondition(Node node, int nodeIndex)
		{
			_matrix.ZeroingRow(nodeIndex);
			_matrix.Di[nodeIndex] = 1;
			_vector[nodeIndex] = _problemParametrs.Function(node, nodeIndex);
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




		#endregion

		enum NormalVectorDirection
		{
			NonCoDirectional = -1,
			CoDirectional = 1,
		}
	}
}
