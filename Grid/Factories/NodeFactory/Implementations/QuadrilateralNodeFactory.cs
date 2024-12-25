using Grid.Enum;
using Grid.Factories.NodeFactory.Interfaces;
using Grid.Models;

using MathModels;
using MathModels.Models;

namespace Grid.Factories.NodeFactory.Implementations;

public class QuadrilateralNodeFactory : INodeFactory
{
    private const GridDimensional Dimensional = GridDimensional.Two;
    private const int CountOfNodesInElement = 4;

    public (List<Node>, List<int>) GetNodes(Area<int>[] areas, Area<double>[] subDomains, Point[][] lines, SpatialCoordinates coordinates)
    {
        var nodes = new List<Node>();
        var missingNodesCounts = new List<int>();
        var missingNodesCount = 0;

        foreach (var node in (IEnumerable<Node>)coordinates)
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

    private Node TransformRectangleToQuadrilateral(Node node, Point xLimits, Point yLimits, Point[] points)
    {
        var newNode = new Node(0, 0);
        for (var j = 0; j < CountOfNodesInElement; j++)
        {
            newNode.X += BasisFunctions.Psi(j, node, xLimits, yLimits) * points[j].X;
            newNode.Y += BasisFunctions.Psi(j, node, xLimits, yLimits) * points[j].Y;
        }
        return newNode;
    }

    private Point[] GetQuadrilateralPoints(Point[][] lines, int areaNumber, Area<int>[] areas)
    {
        var area = areas[areaNumber];
        return
        [
            lines[area.YBottom][area.XLeft],
            lines[area.YBottom][area.XRight], //вершины четырёхугольной подобласти
            lines[area.YTop][area.XLeft],
            lines[area.YTop][area.XRight]
        ];
    }

    /// <summary>
    /// Метод определяет границы прямоугольной области, в которую вписана данная четырёхугольная область
    /// </summary>
    /// <param name="points">вершины четырёхугольника</param>
    private (Point xLimits, Point yLimits) GetRectangleAreaLimits(Point[] points)
    {
        var x = points.Select(point => point.X);
        var y = points.Select(point => point.Y);
        return (new(x.Min(), x.Max()), new(y.Min(), y.Max()));
    }
}