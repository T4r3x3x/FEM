using FemProducer.MatrixBuilding;

using Grid.Models;

using MathModels.Models;

namespace FemProducer.Collector
{
	public class TimeCollector : AbstractCollector
	{
		private readonly SolutionService _solutionService;

		public TimeCollector(SolutionService solutionService, ICollectorBase collectorBase, GridModel grid, MatrixFactory matrixFactory) : base(collectorBase, grid, matrixFactory)
		{
			_solutionService = solutionService;
		}

		public Slae CollectSlae(int timeLayer)
		{
			Vector vector = new Vector(_grid.NodesCount);
			Matrix matrix = _matrixFactory.CreateMatrix(_grid);
			Matrix M, G;

			double deltaT = _grid.T[timeLayer] - _grid.T[timeLayer - 1];
			//	double deltaT1 = _grid.T[timeLayer - 1] - _grid.T[timeLayer - 2];
			//	double deltaT0 = _grid.T[timeLayer] - _grid.T[timeLayer - 1];

			var results = _collectorBase.Collect();

			results.Item1.TryGetValue("M", out M);
			results.Item1.TryGetValue("G", out G);

			Vector vector1 = M * _solutionService.NumericalSolves[timeLayer - 1];
			//	Vector vector2 = M * _solutionService.NumericalSolves[timeLayer - 1];

			//	var timeCoef = (deltaT + deltaT0) / (deltaT * deltaT0);

			vector = results.Item2 + 1 / deltaT * vector1;
			matrix = 1 / deltaT * M + G;

			return new Slae(matrix, vector);
		}

		public override Slae Collect(int timeLayer)
		{
			var slae = CollectSlae(timeLayer);
			_collectorBase.GetBoundaryConditions(slae);
			return slae;
		}
	}
}