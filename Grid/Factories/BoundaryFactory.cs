using Grid.Models;

namespace Grid.Factories
{
    public class BoundaryFactory
    {
        private (int, int) GetAxisLimitIndexes(int leftLimit, int rightLimit, int[] SplitsCount)
        {
            int firstLimit = 0, secondLimit, i;

            for (i = 0; i < leftLimit; i++)
                firstLimit += SplitsCount[i] - 1;

            secondLimit = firstLimit;

            for (; i < rightLimit; i++)
                secondLimit += SplitsCount[i] - 1;

            return (firstLimit, secondLimit);
        }

        #region Одно и тоже, только для разных мерностей
        /// <summary>
        /// Возвращает индексы x'ов и y'ов которые являются границами указанной подобласти
        /// </summary>
        /// <param name="boundaryConditions"></param>
        /// <param name="xSplitsCount"></param>
        /// <param name="ySplitsCount"></param>
        /// <returns></returns>
        private int[] GetAreaLimitIndexes(Area<int> limits, int[] xSplitsCount, int[] ySplitsCount)
        {
            var xLimits = GetAxisLimitIndexes(limits.XLeft, limits.XRight, xSplitsCount);
            var yLimits = GetAxisLimitIndexes(limits.YBottom, limits.YTop, ySplitsCount);

            return [xLimits.Item1, xLimits.Item2, yLimits.Item1, yLimits.Item2];
        }
        /// <summary>
        /// Возвращает индексы x'ов и y'ов которые являются границами указанной подобласти
        /// </summary>
        /// <param name="boundaryConditions"></param>
        /// <param name="xSplitsCount"></param>
        /// <param name="ySplitsCount"></param>
        /// <returns></returns>
        private int[] GetAreaLimitIndexes(Area<int> limits, int[] xSplitsCount, int[] ySplitsCount, int[] zSplitsCount)
        {
            var xLimits = GetAxisLimitIndexes(limits.XLeft, limits.XRight, xSplitsCount);
            var yLimits = GetAxisLimitIndexes(limits.YBottom, limits.YTop, ySplitsCount);
            var zLimits = GetAxisLimitIndexes(limits.ZBack, limits.ZFront, zSplitsCount);

            return [xLimits.Item1, xLimits.Item2, yLimits.Item1, yLimits.Item2, zLimits.Item1, zLimits.Item2];
        }
        #endregion

        private IList<IList<int>> GetBoundaryLimitsIndexes(Area<int>[] boundaryConditionsLimits, int[] xSplitsCount, int[] ySplitsCount)
        {
            IList<IList<int>> boundaryLimitsIndexes = new List<IList<int>>();

            for (int i = 0; i < boundaryConditionsLimits.Length; i++)
            {
                boundaryLimitsIndexes.Add(GetAreaLimitIndexes(boundaryConditionsLimits[i], xSplitsCount, ySplitsCount));
            }

            return boundaryLimitsIndexes;
        }


        private int[][] GetBoundaryLimitsIndexes(Area<int>[] boundaryConditionsLimits, int[] xSplitsCount, int[] ySplitsCount, int[] zSplitsCount)
        {
            List<int[]> boundaryLimitsIndexes = new();

            for (int i = 0; i < boundaryConditionsLimits.Length; i++)
                boundaryLimitsIndexes.Add(GetAreaLimitIndexes(boundaryConditionsLimits[i], xSplitsCount, ySplitsCount, zSplitsCount));

            return boundaryLimitsIndexes.ToArray();
        }

        public (int[], IList<IList<int>>, IList<IList<int>>) GetBoundaryNodes(Area<int>[] areas, Area<int>[] boundaryConditions, SpatialCoordinates coordinates, int[] xSplitsCount, int[] ySplitsCount,
            int[] zSplitsCount, int[] missingNodesCounts)
        {            //трансформируем индексы из W в реальные номера узлов
            var x = coordinates.X;
            var y = coordinates.Y;
            var limits = GetBoundaryLimitsIndexes(boundaryConditions, xSplitsCount, ySplitsCount, zSplitsCount);
            return CalculateBoundaryNodes(boundaryConditions, limits, x.Length, y.Length, missingNodesCounts);
        }

        private (int[], IList<IList<int>>, IList<IList<int>>) CalculateBoundaryNodes(Area<int>[] boundaryConditions, int[][] limits, int xCount, int yCount, int[] missingNodesCounts)
        {
            HashSet<int> fisrtBoundaries = new();
            HashSet<int> secondBoundaryNodes = new HashSet<int>();
            List<List<int>> secondBoundaryIndexes = new();
            for (int i = 0; i < limits.Length; i++)
            {
                switch (boundaryConditions[i].FormulaNumber) //boundaryType
                {
                    case 0://первое к.у.
                        {
                            var nodes = GetFisrtBoundaryNodes(limits[i], xCount, yCount, missingNodesCounts);
                            for (int j = 0; j < nodes.Count; j++)
                                fisrtBoundaries.Add(nodes[j]);
                        }
                        break;
                    case 1:
                        {
                            break;
                        }
                    default:
                        throw new ArgumentException($"Boundary type was wrong - {limits[i][6]}");

                }
            }
            return (fisrtBoundaries.ToArray(), secondBoundaryIndexes.ToArray(), null);
        }


        private List<int> GetFisrtBoundaryNodes(int[] limits, int xCount, int yCount, int[] missingNodesCounts)
        {
            List<int> boundaryNodes = new List<int>();
            for (int zIndex = limits[4]; zIndex <= limits[5]; zIndex++)
                for (int yIndex = limits[2]; yIndex <= limits[3]; yIndex++)
                    for (int xIndex = limits[0]; xIndex <= limits[1]; xIndex++)
                    {
                        var layerStartIndex = zIndex * xCount * yCount;
                        var lineStartIndex = yIndex * xCount;
                        var index = layerStartIndex + lineStartIndex + xIndex;

                        var realIndex = index - missingNodesCounts[index];
                        boundaryNodes.Add(realIndex);
                    }
            return boundaryNodes;
        }

        //private FiniteElement[] GetSecondBoundaryNodes()
        //{

        //}
        //private FiniteElement[] GetThirdBoundaryNodes()
        //{

        //}
    }
}