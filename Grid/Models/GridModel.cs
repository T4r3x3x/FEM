
using MathModels;

namespace Grid.Models
{
    public class GridModel
    {
        public readonly IReadOnlyList<double> T;
        public readonly IReadOnlyList<double> Ht;
        public readonly IReadOnlyList<double> X;
        public readonly IReadOnlyList<double> Y;
        public readonly IReadOnlyList<double> Z;
        private readonly int _nodesInElementCount; //количество узлов в кэ.

        public GridModel(IList<FiniteElementScheme> elements, IList<Node> nodes,
            IEnumerable<int> firstBoundaryNodes, IEnumerable<FiniteElementScheme> secondBoundaryNodes, IEnumerable<FiniteElementScheme> thirdBoundaryNodes, IList<Point[]> subdomains,
            int nodesInElementCount, double[] x, double[] y, double[] z, Area<int>[] areas, double[] t = null, List<double> ht = null)
        {
            FirstBoundaryNodes = firstBoundaryNodes;
            SecondBoundaryNodes = secondBoundaryNodes;
            ThirdBoundaryNodes = thirdBoundaryNodes;
            Elements = elements;
            Nodes = nodes;
            T = t ?? [0];
            Ht = ht;
            Subdomains = subdomains;
            _nodesInElementCount = nodesInElementCount;
            X = x;
            Y = y;
            Z = z;
            _areas = areas;
        }

        public IList<Point[]> Subdomains { get; }
        public IList<FiniteElementScheme> Elements { get; private set; }
        public IList<Node> Nodes { get; private set; }
        public IEnumerable<int> FirstBoundaryNodes { get; private set; }
        public IEnumerable<FiniteElementScheme> SecondBoundaryNodes { get; private set; }
        public IEnumerable<FiniteElementScheme> ThirdBoundaryNodes { get; private set; }
        public int TimeLayersCount => T.Count;
        public int ElementsCount => Elements.Count;
        public int NodesCount => Nodes.Count;
        public int XCount => X.Count;
        public int YCount => Y.Count;
        public int ZCount => Z.Count;

        private Area<int>[] _areas;

        public IList<Node> ElementToNodes(FiniteElementScheme element)
        {
            Node[] nodes = new Node[element.NodesIndexes.Length];

            for (int i = 0; i < nodes.Length; i++)//переделать, пусть спраишвает у элемента
                nodes[i] = Nodes[element.NodesIndexes[i]];

            return nodes;
        }
        private double YLine(double x, double x1, double y1, double x2, double y2)
        {
            if (y1 == y2)
                return y1;

            if (y1 > y2)
            {
                var temp = x1;
                x1 = x2;
                x2 = temp;
                temp = y1;
                y1 = y2;
                y2 = temp;
            }

            var y = (x - x1) * (y2 - y1) / (x2 - x1) + y1;
            return y;
        }


        public int GetSubDomain(Node node)
        {
            double y0, y1;
            for (int i = 0; i < Subdomains.Count; i++)
            {
                y0 = YLine(node.X, Subdomains[i][0].X, Subdomains[i][0].Y, Subdomains[i][1].X, Subdomains[i][1].Y);
                y1 = YLine(node.X, Subdomains[i][2].X, Subdomains[i][2].Y, Subdomains[i][3].X, Subdomains[i][3].Y);
                if (Subdomains[i][0].X <= node.X && node.X <= Subdomains[i][1].X)
                    if (y0 <= node.Y && node.Y <= y1)
                        return _areas[i].FormulaNumber;
            }
            return -1;
        }

        /// <summary>
        /// Возвращает кэ, в котором находится точка.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public FiniteElementScheme GetElement(Point point)
        {
            for (int i = 0; i < Elements.Count; i++)
                if (IsElementContainPoint(Elements[i], point))
                    return Elements[i];

            return null;
        }

        private bool IsElementContainPoint(FiniteElementScheme element, Point point)
        {
            if (Nodes[element.NodesIndexes[0]].X < point.X && point.X < Nodes[element.NodesIndexes[1]].X)
                if (Nodes[element.NodesIndexes[0]].Y < point.Y && point.Y < Nodes[element.NodesIndexes[3]].Y)
                    return true;

            return false;
        }

        private bool BelongToTriangle(Node center, Node p1, Node p2, Node p3)
        {
            var a = (p1.X - center.X) * (p2.Y - p1.Y) - (p2.X - p1.X) * (p1.Y - center.Y);
            var b = (p2.X - center.X) * (p3.Y - p2.Y) - (p3.X - p2.X) * (p2.Y - center.Y);
            var c = (p3.X - center.X) * (p1.Y - p3.Y) - (p1.X - p3.X) * (p3.Y - center.Y);

            if ((a >= 0 && b >= 0 && c >= 0) || (a <= 0 && b <= 0 && c <= 0))
                return true;

            return false;
        }

        [Obsolete]
        public double GetCoefficient(Node elementCenter, int areaNumber)
        {
            Node centerOfDomain = new Node((X[X.Count - 1] + X[0]) / 2, (Y[Y.Count - 1] + Y[0]) / 2);//hx/2 hy/2
            var distance = new Node(Math.Abs(elementCenter.X - centerOfDomain.X), Math.Abs(elementCenter.Y - centerOfDomain.Y));

            var a = areaNumber % 2;
            return a switch
            {
                0 => (centerOfDomain.X - distance.X) / centerOfDomain.X,
                1 => (centerOfDomain.Y - distance.Y) / centerOfDomain.Y,
            };
        }

        public int GetAreaNumber(Node center)
        {
            Node p1 = new Node(X[0], Y[0]);
            Node p2 = new Node(X[X.Count - 1], Y[0]);
            Node p3 = new Node(X[0], Y[Y.Count - 1]);
            Node p4 = new Node(X[X.Count - 1], Y[Y.Count - 1]);
            Node centerOfDomain = new Node((X[X.Count - 1] + X[0]) / 2, (Y[Y.Count - 1] + Y[0]) / 2);

            if (BelongToTriangle(center, p1, p2, centerOfDomain))
                return 0;
            if (BelongToTriangle(center, p2, p4, centerOfDomain))
                return 1;
            if (BelongToTriangle(center, p3, p4, centerOfDomain))
                return 2;
            if (BelongToTriangle(center, p1, p3, centerOfDomain))
                return 3;

            throw new Exception();
        }
    }
}