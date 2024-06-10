using Grid.Enum;

namespace Grid.Models
{
    public class FiniteElement
    {
        public Node[] Nodes;
        public AxisOrientation AxisOrientation { get; }
        public int FormulaNumber { get; }

        public FiniteElement(Node[] nodes, AxisOrientation section, int formulaNumber)
        {
            Nodes = nodes;
            AxisOrientation = section;
            FormulaNumber = formulaNumber;
        }

        public (double, double, double) GetSteps3D() => (GetXStep(Nodes), GetYStep(Nodes), GetZStep(Nodes));

        /// <summary </summary>
        /// <param name="nodes"></param>
        /// <returns>возвращает шаги по конечному элементу, согласно сечению</returns>
        public (double, double) GetSteps2D()
        {
            return AxisOrientation switch
            {
                AxisOrientation.XY => (GetXStep(Nodes), GetYStep(Nodes)),
                AxisOrientation.XZ => (GetXStep(Nodes), Nodes[2].Z - Nodes[0].Z),
                AxisOrientation.YZ => (Nodes[1].Y - Nodes[0].Y, Nodes[2].Z - Nodes[0].Z),
                _ => throw new ArgumentException("Invalid argument!")
            };
        }

        private static double GetXStep(IList<Node> Nodes) => Nodes[1].X - Nodes[0].X;
        private static double GetYStep(IList<Node> Nodes) => Nodes[2].Y - Nodes[0].Y;
        private static double GetZStep(IList<Node> Nodes) => Nodes[4].Z - Nodes[0].Z;
    }
}