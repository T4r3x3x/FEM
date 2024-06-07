﻿using Grid.Factories.ElementFactory.Interfaces;
using Grid.Models;

namespace Grid.Factories.ElementFactory.Implemenations
{
    public class CubeElementFactory : AbstractElementFactory
    {
        private const int NodesCount = 8;

        public override List<FiniteElementScheme> GetElements(SpatialCoordinates coordinates, Area<double>[] subDomains, int[] missingNodesCounts)
        {
            List<FiniteElementScheme> elements = new List<FiniteElementScheme>();
            var x = coordinates.X;
            var y = coordinates.Y;
            var z = coordinates.Z;
            var xy = x.Length * y.Length;

            for (int zIndex = 0; zIndex < z.Length - 1; zIndex++)
            {
                int Oxy1 = zIndex * xy;
                int Oxy2 = (zIndex + 1) * xy;

                for (int yIndex = 0; yIndex < y.Length - 1; yIndex++)
                {
                    int lineIndex1 = yIndex * x.Length;
                    int lineIndex2 = (yIndex + 1) * x.Length;

                    for (int xIndex = 0; xIndex < x.Length - 1; xIndex++)
                    {
                        var areaNumber = GetAreaNumber(x, y, z, zIndex, yIndex, xIndex, subDomains);
                        if (areaNumber == -1)
                            continue;

                        var nodesIndexes = GetNodesIndexes(missingNodesCounts, Oxy1, Oxy2, lineIndex1, lineIndex2, xIndex);
                        var finiteElement = new FiniteElementScheme(nodesIndexes, subDomains[areaNumber].FormulaNumber);
                        elements.Add(finiteElement);
                    }
                }
            }
            return elements;
        }

        private int[] GetNodesIndexes(int[] missingNodesCounts, int Oxy1, int Oxy2, int lineIndex1, int lineIndex2, int xIndex)
        {
            var nodesIndexes = GetNodesIndexesWithoutMissingNodes(Oxy1, Oxy2, lineIndex1, lineIndex2, xIndex);
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
        private int[] GetNodesIndexesWithoutMissingNodes(int Oxy1, int Oxy2, int lineIndex1, int lineIndex2, int xIndex)
        {
            return [
                Oxy1 + lineIndex1 + xIndex,
                Oxy1 + lineIndex1 + xIndex + 1,
                Oxy1 + lineIndex2 + xIndex,
                Oxy1 + lineIndex2 + xIndex + 1,
                Oxy2 + lineIndex1 + xIndex,
                Oxy2 + lineIndex1 + xIndex + 1,
                Oxy2 + lineIndex2 + xIndex,
                Oxy2 + lineIndex2 + xIndex + 1];
        }


        private static int GetAreaNumber(double[] x, double[] y, double[] z, int zIndex, int yIndex, int xIndex, Area<double>[] subDomains)
        {
            Node elementCenter = GetCenter(x, y, z, zIndex, yIndex, xIndex);
            return BaseMethods.GetAreaNumber(subDomains, elementCenter);
        }

        private static Node GetCenter(double[] x, double[] y, double[] z, int zIndex, int yIndex, int xIndex) =>
            new Node((x[xIndex] + x[xIndex + 1]) / 2.0, (y[yIndex] + y[yIndex + 1]) / 2.0, (z[zIndex] + z[zIndex + 1]) / 2.0);
    }
}