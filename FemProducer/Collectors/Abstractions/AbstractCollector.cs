
using FemProducer.Collector.CollectorBase.Interfaces;
using FemProducer.MatrixBuilding;

using Grid.Models;

using MathModels.Models;

namespace FemProducer.Collectors.Abstractions
{
    public abstract class AbstractCollector
    {
        protected readonly ICollectorBase _collectorBase;
        protected readonly GridModel _grid;
        protected readonly MatrixFactory _matrixFactory;

        protected AbstractCollector(ICollectorBase collectorBase, GridModel grid, MatrixFactory matrixFactory)
        {
            _collectorBase = collectorBase;
            _grid = grid;
            _matrixFactory = matrixFactory;
        }

        public abstract Slae Collect(int timeLayer);
    }
}
