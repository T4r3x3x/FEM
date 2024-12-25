﻿using MathModels;
using MathModels.Models;

namespace Grid.Models;

public class GridModel
{
    public readonly int NodesInElementCount;
    public readonly IReadOnlyList<double> T;
    public readonly IReadOnlyList<double> Ht;
    public readonly IReadOnlyList<double> X;
    public readonly IReadOnlyList<double> Y;
    public readonly IReadOnlyList<double> Z;
    public readonly Dictionary<Node, int> NodesIndexes;

    /// <summary>
    /// Сосредоточенные источники
    /// </summary>
    public readonly IReadOnlyList<FiniteElementScheme> Sources;

    public GridModel(IList<FiniteElementScheme> elements, IList<Node> nodes,
        IEnumerable<int> firstBoundaryNodes, IEnumerable<FiniteElementScheme> secondBoundaryNodes, IEnumerable<FiniteElementScheme> thirdBoundaryNodes, IList<Point[]> subdomains,
        int nodesInElementCount, double[] x, double[] y, double[] z, Area<int>[] areas, IReadOnlyList<FiniteElementScheme> sources, Dictionary<Node, int> nodesIndexes, double[] t = null!, List<double> ht = null!)
    {
        FirstBoundaryNodes = firstBoundaryNodes;
        SecondBoundaryNodes = secondBoundaryNodes;
        ThirdBoundaryNodes = thirdBoundaryNodes;
        Elements = elements;
        Nodes = nodes;
        T = t ?? [0];
        Ht = ht;
        Subdomains = subdomains;
        NodesInElementCount = nodesInElementCount;
        X = x;
        Y = y;
        Z = z;
        _areas = areas;
        Sources = sources;
        NodesIndexes = nodesIndexes;
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

    /// <summary>
    /// Может вообще избавится от схемы, так как все равно для каждого кэ создавать элемент. Выгода неочевидна.
    /// </summary>
    /// <param name="sheme"></param>
    /// <returns></returns>
    public FiniteElement GetFiniteElement(FiniteElementScheme sheme)
    {
        var nodes = new Node[sheme.NodesIndexes.Length];
        for (var i = 0; i < sheme.NodesIndexes.Length; i++)
            nodes[i] = Nodes[sheme.NodesIndexes[i]];

        return new(nodes, sheme.Section, sheme.FormulaNumber);
    }
    private double YLine(double x, double x1, double y1, double x2, double y2)
    {
        if (y1 == y2)
            return y1;

        if (y1 > y2)
        {
            (x2, x1) = (x1, x2);
            (y2, y1) = (y1, y2);
        }

        var y = (x - x1) * (y2 - y1) / (x2 - x1) + y1;
        return y;
    }

    public int GetSubDomain(Node node)
    {
        double y0, y1;
        for (var i = 0; i < Subdomains.Count; i++)
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
        for (var i = 0; i < Elements.Count; i++)
            if (IsElementContainPoint(Elements[i], point))
                return Elements[i];

        throw new("Can't find an element!");
    }

    private bool IsElementContainPoint(FiniteElementScheme element, Point point)
    {
        if (Nodes[element.NodesIndexes[0]].X < point.X && point.X < Nodes[element.NodesIndexes[1]].X)
            if (Nodes[element.NodesIndexes[0]].Y < point.Y && point.Y < Nodes[element.NodesIndexes[3]].Y)
                return true;

        return false;
    }
}