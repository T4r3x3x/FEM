using Grid.Models;

namespace Grid.Factories.ElementFactory.Interfaces
{
    public abstract class AbstractElementFactory
    {
        public abstract List<FiniteElement> GetElements(SpatialCoordinates coordinates, Area<double>[] subDomainsValues, List<int> missingNodesCounts);

        protected int NodeIndex3D(int zIndex, int xCount, int yCount) => zIndex * xCount * yCount;

        /// <summary></summary>
        /// <param name="yIndex"></param>
        /// <param name="xCount"></param>
        /// <param name="zIndex">В двумерном случае неуказывать, нужен для расчёта краевых элементов в 3мерных задачах (задаёт смещение по оси z)</param>
        /// <returns></returns>
        protected int NodeIndex2D(int yIndex, int xCount, int zIndex = 1) => zIndex * yIndex * xCount;

        /// <summary></summary>
        /// <param name="xIndex"></param>
        /// <param name="yIndex">В одномерном случае неуказывать, нужен для расчёта краевых элементов в многомерных задачах (задаёт смещение по оси y)</param>
        /// <param name="zIndex">В одномерном случае неуказывать, нужен для расчёта краевых элементов в многомерных задачах (задаёт смещение по оси z)</param>
        /// <returns></returns>
        protected int NodeIndex1D(int xIndex, int yIndex = 1, int zIndex = 1) => zIndex * yIndex * xIndex;
    }
}
