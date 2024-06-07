namespace Grid.Models
{
    public class FiniteElement
    {
        public Node[] Nodes;
        public Section2D Section { get; }
        public int FormulaNumber { get; }

        public FiniteElement(Node[] nodes, Section2D section, int formulaNumber)
        {
            Nodes = nodes;
            Section = section;
            FormulaNumber = formulaNumber;
        }
    }
}
