using Grid.Enum;

namespace Grid.Models
{
    public class FiniteElementScheme
    {
        public int[] NodesIndexes { get; }
        public int FormulaNumber;
        public Section2D Section { get; }

        public FiniteElementScheme(int[] nodesIndexes, int formulaNumber, Section2D section)
        {
            NodesIndexes = nodesIndexes;
            FormulaNumber = formulaNumber;
            Section = section;
        }



    }
}