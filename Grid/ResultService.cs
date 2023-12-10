using FemProducer.Grid;

using Tensus;

namespace FemProducer
{
	internal class ResultService
	{
		public IList<Vector> NumericalSolves { get; private set; } = new List<Vector>();
		public IList<Vector> AnalyticsSolves { get; private set; } = new List<Vector>();

		private readonly GridModel _grid;

		public ResultService(ProblemService problemParametrs, GridModel grid)
		{
			_grid = grid;
			CalculateAnalysticsSolves(problemParametrs);
		}

		private void CalculateAnalysticsSolves(ProblemService problemService)
		{
			//for (int t = 0; t < _grid.TimeLayersCount; t++)
			//{
			var exactSolution = new Vector(_grid.NodesCount);
			for (int i = 0; i < _grid.M; i++)
				for (int j = 0; j < _grid.N; j++)
					exactSolution[i * _grid.N + j] = problemService.Function(_grid.X[j], _grid.Y[i], _grid.GetAreaNumber(j, i));
			AnalyticsSolves.Add(exactSolution);
			//}
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
