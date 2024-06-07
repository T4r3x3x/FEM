using Grid.Models;

namespace Grid.Factories.ElementFactory.Interfaces
{
    public abstract class AbstractElementFactory
    {
        public abstract List<FiniteElementScheme> GetElements(SpatialCoordinates coordinates, Area<double>[] subDomainsValues, int[] missingNodesCounts);

        /// <summary>
        /// Расчёт индекса узла в 3х мерном пространстве
        /// </summary>
        /// <param name="zIndex">если указать 0, то получится номер в 2ном пространстве (см. пример ниже)</param>
        /// <param name="yIndex">если указать 0, то получится номер в 1ном пространстве (см. пример ниже)</param>
        /// <param name="xIndex"></param>
        /// <param name="xCount"></param>
        /// <param name="yCount"></param>
        /// <returns></returns>
        protected int GetNodeIndex3D(int zIndex, int yIndex, int xIndex, int xCount, int yCount) => LayerStartIndex(zIndex, xCount, yCount) + LineStartIndex(yIndex, xCount) + xIndex;

        /// <summary></summary>
        /// <param name="yIndex"></param>
        /// <param name="xCount"></param>
        /// <returns></returns>
        protected int GetNodeIndex2D(int yIndex, int xIndex, int xCount, int yCount) => GetNodeIndex3D(0, yIndex, xIndex, xCount, yCount);

        /// <summary></summary>
        /// <param name="xIndex"></param>
        /// <returns></returns>
        protected int GetNodeIndex1D(int xIndex) => GetNodeIndex3D(0, 0, xIndex, 0, 0);

        private int LayerStartIndex(int zIndex, int xCount, int yCount) => zIndex * xCount * yCount;
        private int LineStartIndex(int yIndex, int xCount) => yIndex * xCount;
    }
}
