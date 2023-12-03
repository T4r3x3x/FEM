using FemProducer;

namespace ReaserchPaper
{
	internal class ResultProducer
	{
		public IList<Vector> NumericalSolves { get; private set; }
		public IList<Vector> AnalyticsSolves { get; private set; }

		private readonly Grid.Grid _grid;

		public ResultProducer(ProblemParametrs problemParametrs, Grid.Grid grid)
		{
			_grid = grid;
			CalculateAnalysticsSolves(problemParametrs);
		}

		private void CalculateAnalysticsSolves(ProblemParametrs problemParametrs)
		{
			for (int t = 0; t < _grid.TimeLayersCount; t++)
			{
				var exactSolution = new Vector(_grid.NodesCount);
				for (int i = 0; i < _grid.M; i++)
					for (int j = 0; j < _grid.N; j++)
						exactSolution[i * _grid.N + j] = problemParametrs.Func2(_grid.X[j], _grid.Y[i], _grid.T[t], _grid.GetAreaNumber(j, i));
				AnalyticsSolves.Add(exactSolution);
			}
		}

		public double GetSolveDifference(int layer)
		{
			int size = NumericalSolves[0].Length;
			Vector temp = new Vector(size);

			for (int i = 0; i < size; i++)
				temp[i] = NumericalSolves[layer][i] - AnalyticsSolves[layer][i];

			return temp.GetNorm() / NumericalSolves[layer].GetNorm();
		}
	}
}
