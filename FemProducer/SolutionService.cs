using Grid.Models;

using MathModels.Models;

namespace FemProducer
{
	public class SolutionService
	{
		public IList<Vector> NumericalSolves { get; private set; } = new List<Vector>();
		public IList<Vector> AnalyticsSolves { get; private set; } = new List<Vector>();

		private readonly GridModel _grid;

		public SolutionService(ProblemService problemService, GridModel grid)
		{
			_grid = grid;
			CalculateAnalyticsSolves(problemService);
		}

		private void CalculateAnalyticsSolves(ProblemService problemService)
		{
			for (int t = 0; t < _grid.TimeLayersCount; t++)
			{
				var exactSolution = new Vector(_grid.NodesCount);
				for (int i = 0; i < _grid.Nodes.Count; i++)
				{
					var area = _grid.GetSubDomain(_grid.Nodes[i]);
					exactSolution[i] = problemService.Function(_grid.Nodes[i], area);
				}

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