using Grid.Enum;
using Grid.Factories.ElementFactory.Abstractions;
using Grid.Factories.GridFactories.Interfaces;
using Grid.Factories.NodeFactory.Implementations;
using Grid.Factories.NodeFactory.Interfaces;
using Grid.Models;
using Grid.Models.InputModels;

using MathModels;
using MathModels.Models;

using static Grid.Factories.AxisFactory;

namespace Grid.Factories.GridFactories.Implementations;

public class GridFactory(AbstractElementFactory elementFactory, INodeFactory nodeFactory, BoundaryFactory boundaryFactory) : IGridFactory
{

    public GridModel GetGrid(GridInputParameters @params)
    {
        (var coordinates, var t) = GetAxisesToPoints(@params);

        var subDomainsBoundaries = (@params.XW is not null && @params.YW is not null) switch
        {
            true => GetSubDomainsBoundariesByLimits(@params.XW!, @params.YW!, @params.ZW, @params.Areas),
            false => GetSubDomainsBoundariesByLines(@params.ZW, @params.LinesNodes, @params.Areas)
        };

        (var nodes, var missingNodes) = nodeFactory.GetNodes(@params.Areas, subDomainsBoundaries, @params.LinesNodes, coordinates);

        var elements = elementFactory.GetElements(coordinates, subDomainsBoundaries, [.. missingNodes]);

        var boundaryNodes = boundaryFactory.GetBoundaryNodes(@params.Areas, @params.BoundaryConditions, coordinates, @params.XParams.SplitsCount,
            @params.YParams.SplitsCount, @params.ZParams.SplitsCount, [.. missingNodes]);

        var realSubdomains = GetRealSubdomains(@params.LinesNodes, @params.Areas);

        var nodesInElementCount = CalculateNodesCountInElement(@params.GridDimensional);

        var nodesIndexes = new Dictionary<Node, int>(nodes.Select((n, i) => new KeyValuePair<Node, int>(n, i)));

        return new(elements, nodes ?? throw new("Nodes is null!"), boundaryNodes.Item1.Order(), boundaryNodes.Item2, boundaryNodes.Item3, realSubdomains,
            nodesInElementCount, coordinates.X, coordinates.Y, coordinates.Z, @params.Areas, null!, nodesIndexes, t);
    }

    private static int CalculateNodesCountInElement(GridDimensional dimensional) => (int)Math.Pow(2, (int)dimensional);

    /// <summary></summary>
    /// <param name="ZW"></param>
    /// <param name="lines"></param>
    /// <param name="areas"></param>
    /// <returns>границы подобластей в виде значений по х,у,z</returns>
    private static Area<double>[] GetSubDomainsBoundariesByLines(double[] ZW, Point[][] lines, Area<int>[] areas)
    {
        var subDomains = new Area<double>[areas.Length];
        for (var i = 0; i < subDomains.Length; i++)
        {
            var area = areas[i];
            subDomains[i] = new(
                lines[area.YBottom][area.XLeft].X,
                lines[area.YTop][area.XRight].X,
                lines[area.YBottom][area.XRight].Y,
                lines[area.YTop][area.XRight].Y,
                ZW[area.ZBack],
                ZW[area.ZFront],
                area.FormulaNumber, area.AreaType);
        }
        return subDomains;
    }

    /// <summary></summary>
    /// <param name="zw"></param>
    /// <param name="lines"></param>
    /// <param name="areas"></param>
    /// <returns>границы подобластей в виде значений по х,у,z</returns>
    private static Area<double>[] GetSubDomainsBoundariesByLimits(double[] xw, double[] yw, double[] zw, Area<int>[] areas)
    {
        var subDomains = new Area<double>[areas.Length];
        for (var i = 0; i < subDomains.Length; i++)
        {
            var area = areas[i];
            subDomains[i] = new(
                xw[area.XLeft],
                xw[area.XRight],
                yw[area.YBottom],
                yw[area.YTop],
                zw[area.ZBack],
                zw[area.ZFront],
                area.FormulaNumber,
                area.AreaType);
        }
        return subDomains;
    }

    private List<Point[]> GetRealSubdomains(Point[][] lines, Area<int>[] areas) //+
    {
        List<Point[]> subdomains = [];
        for (var i = 0; i < areas.Length; i++)
        {
            var area = areas[i];
            Point[] points =
            [
                lines[area.YBottom][area.XLeft],
                lines[area.YBottom][area.XRight],
                lines[area.YTop][area.XLeft],
                lines[area.YTop][area.XRight]
            ];
            subdomains.Add(points);
        }
        return subdomains;
    }
}