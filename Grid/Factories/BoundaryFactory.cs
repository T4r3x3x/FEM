using Grid.Factories.ElementFactory.Implemenations;
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

        public (int[], FiniteElementScheme[], FiniteElementScheme[]) GetBoundaryNodes(Area<int>[] areas, Area<int>[] boundaryConditions, SpatialCoordinates coordinates, int[] xSplitsCount, int[] ySplitsCount,
            int[] zSplitsCount, int[] missingNodesCounts)
        {            //трансформируем индексы из W в реальные номера узлов
            var x = coordinates.X;
            var y = coordinates.Y;
            var limits = GetBoundaryLimitsIndexes(boundaryConditions, xSplitsCount, ySplitsCount, zSplitsCount);
            return CalculateBoundaryNodes(boundaryConditions, limits, x.Length, y.Length, missingNodesCounts, coordinates);
        }

        private (int[], FiniteElementScheme[], FiniteElementScheme[]) CalculateBoundaryNodes(Area<int>[] boundaryConditions, int[][] limits, int xCount, int yCount, int[] missingNodesCounts, SpatialCoordinates coordinates)
        {
            HashSet<int> fisrtBoundaries = new();
            List<FiniteElementScheme> secondBoundaryElems = new();
            List<FiniteElementScheme> thirdBoundaryElems = new();
            for (int i = 0; i < limits.Length; i++)
            {
                switch (boundaryConditions[i].FormulaNumber) //boundaryType
                {
                    case 0://первое к.у.
                        {
                            var nodes = GetFisrtBoundaryNodes(limits[i], xCount, yCount, missingNodesCounts);
                            for (int j = 0; j < nodes.Count; j++)
                                fisrtBoundaries.Add(nodes[j]);
                            break;
                        }
                    case 1://второе ку
                        {
                            var elems = GetSecondBoundaryNodes(limits[i], missingNodesCounts, coordinates, 0);
                            secondBoundaryElems.AddRange(elems);
                            break;
                        }
                    case 2: //третье ку
                        {
                            var elems = GetSecondBoundaryNodes(limits[i], missingNodesCounts, coordinates, 0);
                            thirdBoundaryElems.AddRange(elems);
                            break;
                        }
                    default:
                        throw new ArgumentException($"Boundary type was wrong - {limits[i][6]}");

                }
            }
            return (fisrtBoundaries.ToArray(), secondBoundaryElems.ToArray(), thirdBoundaryElems.ToArray());
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

        //надо как-то учитывать по какой формуле должен происходить учёт ку 
        private FiniteElementScheme[] GetSecondBoundaryNodes(int[] limits, int[] missingNodesCounts, SpatialCoordinates coordinates, int formulaNumber)
        {
            var sectionData = GetSectionData(limits, coordinates);
            ReactangularElementFactory elementFactory = new(sectionData.section, sectionData.secitonIndex);
            var elements = elementFactory.GetElements(coordinates, null!, missingNodesCounts);
            //  AccountOffset(elements, sectionData.offset);
            foreach (var element in elements) //решить где и как будет решаться по какой формуле надо вычитывать ку 
                element.FormulaNumber = formulaNumber;

            return elements.ToArray();
        }

        private FiniteElementScheme[] GetThirdBoundaryNodes(int[] limits, int[] missingNodesCounts, SpatialCoordinates coordinates)
        {
            throw new NotImplementedException();
            //var sectionData = GetSectionData(limits, coordinates);
            //ReactangularElementFactory elementFactory = new(sectionData.section);
            //var elements = elementFactory.GetElements(coordinates, null!, missingNodesCounts);
            //AccountOffset(elements, sectionData.offset);
            //return elements.ToArray();
        }

        /// <summary>
        /// Учитываем смещение по оси, по которой происходит смещение
        /// </summary>
        /// <param name="elements"></param>
        /// <param name="offset">смещение индексов, которое надо учесть</param>
        private void AccountOffset(List<FiniteElementScheme> elements, int offset)
        {
            foreach (var element in elements)
                for (int i = 0; i < element.NodesIndexes.Length; i++)
                    element.NodesIndexes[i] += offset;
        }

        /// <summary>
        /// Определяем по какой оси происходит сечение, и где
        /// </summary>
        /// <param name="limits"></param>
        /// <param name="coordinates"></param>
        /// <returns>сечение, и смещение индексов</returns>
        private (Section2D section, int secitonIndex) GetSectionData(int[] limits, SpatialCoordinates coordinates)
        {
            if (limits[0] == limits[1])
                return (Section2D.YZ, limits[0]);
            if (limits[2] == limits[3])
                return (Section2D.XZ, limits[2]);
            else
                return (Section2D.XY, limits[4]);
        }
    }
}