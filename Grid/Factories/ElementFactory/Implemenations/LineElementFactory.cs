using Grid.Enum;
using Grid.Factories.ElementFactory.Abstractions;
using Grid.Models;

namespace Grid.Factories.ElementFactory.Implemenations
{
    public class LineElementFactory : AbstractElementFactory
    {
        private const GridDimensional Dimensional = GridDimensional.One;

        private readonly AxisOrientation _section;
        private readonly int _sectionIndex; //индекс точки на оси, по которой происходит сечение

        public LineElementFactory(AxisOrientation section, int sectionIndex)
        {
            _section = section;
            _sectionIndex = sectionIndex;
        }

        public override List<FiniteElementScheme> GetElements(SpatialCoordinates coordinates, Area<double>[] subDomains, int[] missingNodesCounts)
        {
            List<FiniteElementScheme> elements = [];
            var x = coordinates.X;

            for (int xIndex = 0; xIndex < x.Length - 1; xIndex++)
            {
                int areaNumber = GetAreaNumber(x, xIndex, subDomains);
                if (areaNumber == -1)
                    continue;

                var nodesIndexes = GetNodesIndexes(missingNodesCounts, xIndex);
                var finiteElemnent = new FiniteElementScheme(nodesIndexes, subDomains[areaNumber].FormulaNumber, AxisOrientation.XY);
                elements.Add(finiteElemnent);
            }
            return elements;
        }

        private int[] GetNodesIndexes(int[] missingNodesCounts, int xIndex)
        {
            var nodesIndexes = GetNodesIndexesWithoutMissingNodes(xIndex);
            AccountMissingNodes(missingNodesCounts, nodesIndexes);
            return nodesIndexes;
        }

        private static void AccountMissingNodes(int[] missingNodesCounts, int[] nodesIndexes)
        {
            for (int h = 0; h < nodesIndexes.Length; h++)
                nodesIndexes[h] -= missingNodesCounts[nodesIndexes[h]];
        }

        /// <summary>
        /// высчитывает индексы узлов кэ с учётом пропущенных (фиктивных узлов)
        /// </summary>
        /// <param name="Oxy1"></param>
        /// <param name="Oxy2"></param>
        /// <param name="lineIndex1"></param>
        /// <param name="lineIndex2"></param>
        /// <param name="xIndex"></param>
        /// <returns></returns>
        private int[] GetNodesIndexesWithoutMissingNodes(int xIndex)
        {
            return [
                xIndex,
                xIndex + 1];
        }

        private static int GetAreaNumber(double[] x, int xIndex, Area<double>[] subDomains)
        {
            var elementCenter = GetCenter(x, xIndex);
            return BaseMethods.GetAreaNumber(subDomains, elementCenter, Dimensional);
        }

        private static Node GetCenter(double[] x, int xIndex) =>
            new((x[xIndex] + x[xIndex + 1]) / 2.0);
    }
}