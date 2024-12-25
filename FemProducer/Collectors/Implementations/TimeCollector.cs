using FemProducer.Collectors.Abstractions;
using FemProducer.Collectors.CollectorBases.Interfaces;
using FemProducer.Services;

using Grid.Models;

using MathModels.Models;

namespace FemProducer.Collectors.Implementations
{
    public class TimeCollector : AbstractCollector
    {
        private readonly SolutionService _solutionService;

        public TimeCollector(SolutionService solutionService, ICollectorBase collectorBase, GridModel grid, MatrixFactory.MatrixFactory matrixFactory) : base(collectorBase, grid, matrixFactory)
        {
            _solutionService = solutionService;
        }
        public override Slae Collect(int timeLayer)
        {
            var slae = CollectSlae(timeLayer);
            _collectorBase.GetBoundaryConditions(slae);
            return slae;
        }

        private Slae CollectSlae(int timeLayer)
        {
            Matrix M, G, H;
            double deltaT = _grid.T[timeLayer] - _grid.T[timeLayer - 2];
            double deltaT1 = _grid.T[timeLayer - 1] - _grid.T[timeLayer - 2];
            double deltaT0 = _grid.T[timeLayer] - _grid.T[timeLayer - 1];

            var results = _collectorBase.Collect();

            results.Item1.TryGetValue("ColumnSize", out M);
            results.Item1.TryGetValue("_G", out G);
            results.Item1.TryGetValue("H", out H);

            Vector vector1 = M * _solutionService.NumericalSolves[timeLayer - 2];
            Vector vector2 = M * _solutionService.NumericalSolves[timeLayer - 1];
            Vector b = results.Item2;
            var timeCoef = (deltaT + deltaT0) / (deltaT * deltaT0);

            var vector = b - deltaT0 / (deltaT * deltaT1) * vector1 + deltaT / (deltaT1 * deltaT0) * vector2;
            var matrix = timeCoef * M + G + H;

            return new Slae(matrix, vector);
        }
    }
}