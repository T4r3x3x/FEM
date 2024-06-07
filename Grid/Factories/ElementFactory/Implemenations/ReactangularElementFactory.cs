using Grid.Enum;
using Grid.Factories.ElementFactory.Interfaces;
using Grid.Models;

namespace Grid.Factories.ElementFactory.Implemenations
{
    public class ReactangularElementFactory : AbstractElementFactory
    {
        private readonly Section2D _section;
        private readonly int _sectionIndex; //индекс точки на оси, по которой происходит сечение

        public ReactangularElementFactory(Section2D section, int sectionIndex)
        {
            _section = section;
            _sectionIndex = sectionIndex;
        }

        //Сделать выбор, по каким координатам строить сетку
        public override List<FiniteElementScheme> GetElements(SpatialCoordinates coordinates, Area<double>[] subDomainsValues, int[] missingNodesCounts)
        {
            List<FiniteElementScheme> elements = new List<FiniteElementScheme>();
            (var innerAxis, var outerAxis) = coordinates.GetAxises(_section);
            int areaNumber = -2;
            for (int outerIndex = 0; outerIndex < outerAxis.Length - 1; outerIndex++)
            {
                for (int innerIndex = 0; innerIndex < innerAxis.Length - 1; innerIndex++)
                {
                    if (subDomainsValues != null)//костыль, нужен для создание краевых элементов, ибо там не важно к какой области он принадлежит
                    {
                        areaNumber = GetAreaNumber(innerAxis, outerAxis, outerIndex, innerIndex, subDomainsValues);
                        if (areaNumber == -1)
                            continue;
                    }

                    var nodesIndexes = GetNodesIndexes(missingNodesCounts, innerIndex, outerIndex, coordinates.X.Length, coordinates.Y.Length);
                    var finiteElemnent = new FiniteElementScheme(nodesIndexes, areaNumber != -2 ? subDomainsValues[areaNumber].FormulaNumber : 0);
                    elements.Add(finiteElemnent);
                }
            }
            return elements;
        }

        private int[] GetNodesIndexes(int[] missingNodesCounts, int innerIndex, int outerIndex, int xCount, int yCount)
        {
            var nodesIndexes = GetNodesIndexesWithoutMissingNodes(innerIndex, outerIndex, xCount, yCount);
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
        /// <param name="xCount"></param>
        /// <returns></returns>
        private int[] GetNodesIndexesWithoutMissingNodes(int innerIndex, int outerIndex, int xCount, int yCount)
        {
            return _section switch
            {
                //outer - y, inner - x
                Section2D.XY =>
                    [GetNodeIndex3D(_sectionIndex, outerIndex, innerIndex, xCount, yCount),
                        GetNodeIndex3D(_sectionIndex, outerIndex, innerIndex + 1, xCount, yCount),
                        GetNodeIndex3D(_sectionIndex, outerIndex + 1, innerIndex, xCount, yCount),
                        GetNodeIndex3D(_sectionIndex, outerIndex + 1, innerIndex + 1, xCount, yCount),
                    ],
                //outer - z, inner - x
                Section2D.XZ =>
                    [GetNodeIndex3D(outerIndex, _sectionIndex, innerIndex, xCount, yCount),
                        GetNodeIndex3D(outerIndex, _sectionIndex, innerIndex + 1, xCount, yCount),
                        GetNodeIndex3D(outerIndex + 1, _sectionIndex, innerIndex, xCount, yCount),
                        GetNodeIndex3D(outerIndex + 1, _sectionIndex, innerIndex + 1, xCount, yCount),
                    ],
                //outer - z, inner - y
                Section2D.YZ =>
                    [GetNodeIndex3D(outerIndex, innerIndex, _sectionIndex, xCount, yCount),
                        GetNodeIndex3D(outerIndex, innerIndex + 1, _sectionIndex, xCount, yCount),
                        GetNodeIndex3D(outerIndex + 1, innerIndex, _sectionIndex, xCount, yCount),
                        GetNodeIndex3D(outerIndex + 1, innerIndex + 1, _sectionIndex, xCount, yCount),
                    ],
                _ => throw new ArgumentException($"Invalid argument - {_section}!")
            };
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