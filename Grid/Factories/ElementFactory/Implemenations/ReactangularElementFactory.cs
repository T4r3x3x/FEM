using Grid.Factories.ElementFactory.Interfaces;
using Grid.Models;

namespace Grid.Factories.ElementFactory.Implemenations
{
    public class ReactangularElementFactory : AbstractElementFactory
    {
        //Сделать выбор, по каким координатам строить сетку
        public override List<FiniteElement> GetElements(SpatialCoordinates coordinates, Area<double>[] subDomainsValues, List<int> missingNodesCounts)
        {
            List<FiniteElement> elements = new List<FiniteElement>();
            var x = coordinates.X;
            var y = coordinates.Y;

            for (int yIndex = 0; yIndex < y.Length - 1; yIndex++)
            {
                for (int xIndex = 0; xIndex < x.Length - 1; xIndex++)
                {
                    int x0 = yIndex * x.Length;
                    int x1 = (yIndex + 1) * x.Length;

                    int areaNumber = GetAreaNumber(x, y, yIndex, xIndex, subDomainsValues);
                    if (areaNumber == -1)
                        continue;

                    var nodesIndexes = GetNodesIndexes(missingNodesCounts, x0, x1, xIndex);
                    var finiteElemnent = new FiniteElement(nodesIndexes, subDomainsValues[areaNumber].FormulaNumber);
                    elements.Add(finiteElemnent);
                }
            }
            return elements;
        }

        private int[] GetNodesIndexes(List<int> missingNodesCounts, int x0, int x1, int xIndex)
        {
            var nodesIndexes = GetNodesIndexesWithoutMissingNodes(x0, x1, xIndex);
            AccountMissingNodes(missingNodesCounts, nodesIndexes);
            return nodesIndexes;
        }

        private static void AccountMissingNodes(List<int> missingNodesCounts, int[] nodesIndexes)
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
        private int[] GetNodesIndexesWithoutMissingNodes(int x0, int x1, int xIndex)
        {
            return [
                x0 + xIndex,
                x0 + xIndex + 1,
                x1 + xIndex,
                x1 + xIndex + 1];
        }

        private static int GetAreaNumber(double[] x, double[] y, int yIndex, int xIndex, Area<double>[] subDomains)
        {
            Node elementCenter = GetCenter(x, y, yIndex, xIndex);
            return BaseMethods.GetAreaNumber(subDomains, elementCenter);
        }

        private static Node GetCenter(double[] x, double[] y, int yIndex, int xIndex) =>
            new Node((x[xIndex] + x[xIndex + 1]) / 2.0, (y[yIndex] + y[yIndex + 1]) / 2.0);
    }
}