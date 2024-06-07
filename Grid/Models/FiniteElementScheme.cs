namespace Grid.Models
{
    public class FiniteElementScheme
    {
        public int[] NodesIndexes;
        public int FormulaNumber;

        public FiniteElementScheme(int[] nodesIndexes, int formulaNumber)
        {
            NodesIndexes = nodesIndexes;
            FormulaNumber = formulaNumber;
        }
    }
}