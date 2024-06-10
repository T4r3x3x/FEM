using FemProducer;

using Grid.Enum;
using Grid.Factories.NodeFactory.Interfaces;
using Grid.Models;

using MathModels;

namespace Grid.Factories.NodeFactory.Implementations
{
    public class QuadrilateralNodeFactory : INodeFactory
    {
        private const GridDimensional Dimensional = GridDimensional.Two;
        private const int CountOfNodesInElement = 4;

        public (List<Node>, List<int>) GetNodes(Area<int>[] areas, Area<double>[] subDomains, Point[][] lines, SpatialCoordinates coordinates)
        {
            List<Node> nodes = new List<Node>();
            List<int> missingNodesCounts = new List<int>();
            int missingNodesCount = 0;

            foreach (var node in (IEnumerable<Node>) coordinates)
            {
                var area = BaseMethods.GetAreaNumber(subDomains, node, Dimensional);
                if (area != -1)
                {
                    var points = GetQuadrilateralPoints(lines, area, areas);
                    (var xLimits, var yLimits) = GetRectangleAreaLimits(points);
                    var newNode = TransformRectangleToQuadrilateral(node, xLimits, yLimits, points);
                    newNode.Z = node.Z;
                    nodes.Add(newNode);
                }
                else
                    missingNodesCount++;

                missingNodesCounts.Add(missingNodesCount);
            }
            return (nodes, missingNodesCounts);
        }

        Node TransformRectangleToQuadrilateral(Node node, Point xLimits, Point yLimits, Point[] points)
        {
            Node newNode = new Node(0, 0);
            for (int j = 0; j < CountOfNodesInElement; j++)
            {
                newNode.X += FEM.Psi(j, node, xLimits, yLimits) * points[j].X;
                newNode.Y += FEM.Psi(j, node, xLimits, yLimits) * points[j].Y;
            }
            return newNode;
        }

        private Point[] GetQuadrilateralPoints(Point[][] lines, int areaNumber, Area<int>[] areas)
        {
            var area = areas[areaNumber];
            return [lines[area.YBottom][area.XLeft],
                lines[area.YBottom][area.XRight], //вершины четырёхугольной подобласти
                lines[area.YTop][area.XLeft],
                lines[area.YTop][area.XRight]];
        }

        /// <summary>
        /// Метод определяет границы прямоугольной области, в которую вписана данная четырёхугольная область
        /// </summary>
        /// <param name="points">вершины четырёхугольника</param>
        private (Point xLimits, Point yLimits) GetRectangleAreaLimits(Point[] points)
        {
            var x = points.Select(x => x.X);
            var y = points.Select(x => x.Y);
            return (new Point(x.Min(), x.Max()), new Point(y.Min(), y.Max()));
        }
    }
}