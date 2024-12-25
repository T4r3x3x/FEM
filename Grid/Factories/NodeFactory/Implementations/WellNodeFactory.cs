using Grid.Enum;
using Grid.Models;

using Point = (double x, double y);

namespace Grid.Factories.NodeFactory.Implementations;

using SupportLine = (Point outer, Point inner);

public static class WellNodeFactory
{
    private const double Tolerance = 1e-15;
    private const double OkopkaStep = 0.02;

    public static (IList<Node> nodes, IList<FiniteElementScheme> finiteElements, int[] firstBoundaryNodes, FiniteElementScheme source) BuildWellArea(WellArea wellDomain, int stepsCount, double q, double[] x, double[] y, double[] z, List<Node> nodes, IList<int> boundaryNodes,
        FiniteElementScheme[] finiteElementSchemes, IList<FiniteElementScheme>? sources = null!)
    {
        for (var i = 0; i < nodes.Count; i++)
        {
            nodes[i].Y = Math.Round(nodes[i].Y, 4);
        }

        var boundaryX = x.SkipWhile(x => x < wellDomain.XLeft).TakeWhile(x => x <= wellDomain.XRight).ToArray();
        var boundaryY = y.SkipWhile(y => y < wellDomain.YBottom).TakeWhile(y => y <= wellDomain.YTop).ToArray();

        var supportLines = BuildSupportLines(wellDomain, boundaryX, boundaryY);


        (var elems, var newNodes) = BuildElems(wellDomain, stepsCount, q, z, supportLines);

        newNodes = newNodes.Distinct().OrderBy(node => node.Z).ThenBy(node => node.Y).ThenBy(node => node.X).ToArray();

        var localNodesIndexes = new Dictionary<Node, int>();
        for (var i = 0; i < newNodes.Length; i++)
            localNodesIndexes.Add(newNodes[i], i);

        var globalNodesIndexes = new Dictionary<Node, int>();

        foreach (var _z in z)
        {
            for (var i = 0; i < boundaryY.Length - 1; i++)
            {
                var innerNodes = newNodes.Where(node => !IsBoundaryNode(node, boundaryX, boundaryY));
                var insertNodes = innerNodes.Where(node => boundaryY[i] <= node.Y && node.Y < boundaryY[i + 1] && Math.Abs(node.Z - _z) < Tolerance).ToArray();

                var nodeIndex = nodes.FindIndex(node => Math.Abs(node.X - boundaryX[0]) < Tolerance && Math.Abs(node.Y - boundaryY[i]) < Tolerance && Math.Abs(node.Z - _z) < Tolerance);
                //плюс еще сколько до след узла
                var nodesCountAtRight = nodes.Skip(nodeIndex).TakeWhile(node => node.Y <= nodes[nodeIndex].Y).Count();
                var insertIndex = nodeIndex + nodesCountAtRight;
                //  todo + 1 + количество узлов по х справа
                nodes.InsertRange(insertIndex, insertNodes);

                foreach (var scheme in finiteElementSchemes)
                    for (var j = 0; j < scheme.NodesIndexes.Length; j++)
                        if (scheme.NodesIndexes[j] >= insertIndex)
                            scheme.NodesIndexes[j] += insertNodes.Length; //не будет работать если будут дополнительные линии 

                if (sources != null)
                    foreach (var sourceScheme in sources)
                        for (var j = 0; j < sourceScheme.NodesIndexes.Length; j++)
                            if (sourceScheme.NodesIndexes[j] >= insertIndex)
                                sourceScheme.NodesIndexes[j] += insertNodes.Length; //не будет работать если будут дополнительные линии 

                for (var j = 0; j < boundaryNodes.Count; j++)
                    if (boundaryNodes[j] >= insertIndex)
                        boundaryNodes[j] += insertNodes.Length;
            }
        }

        // nodes.AddRange(newNodes);
        var k = 0;
        foreach (var node in nodes.Where(node => !globalNodesIndexes.TryGetValue(node, out _)))
            globalNodesIndexes.TryAdd(node, k++);

        nodes = globalNodesIndexes.Select(pair => pair.Key).ToList();

        var schemes = ElementsToSchemes(elems, localNodesIndexes);

        TransformLocalNumericInGlobal(schemes, newNodes, globalNodesIndexes);

        var source = GetSourceElementScheme(wellDomain.SourceZ, supportLines, globalNodesIndexes, z, wellDomain.FormulaNumber);
        var firstBoundaryNodes = GetFirstBoundaryNodes(supportLines, newNodes, globalNodesIndexes, z, boundaryX, boundaryY, globalNodesIndexes);

        return (nodes, schemes, firstBoundaryNodes, source);
    }

    /// <summary>
    /// Находим кэ в котором находится источник
    /// </summary>
    /// <returns></returns>
    private static FiniteElementScheme GetSourceElementScheme(double sourceZ, SupportLine[] supportLine, Dictionary<Node, int> nodeIndexes, double[] zW, int formulaNumber)
    {
        var innerPoints = supportLine.Select(line => line.inner).ToArray();
        (var z1, var z2) = (zW.Last(z => z <= sourceZ), zW.First(z => z > sourceZ));

        var elementNodes = innerPoints.Select(point => new Node(point.x, point.y, z1)).Concat(innerPoints.Select(point => new Node(point.x, point.y, z2)));

        var indexes = elementNodes.Select(node => nodeIndexes[node]).Order().ToArray();

        return new(indexes, formulaNumber, AxisOrientation.XYZ);
    }

    private static FiniteElementScheme[] ElementsToSchemes(FiniteElement[] elements, Dictionary<Node, int> nodesIndexes)
    {
        var elementSchemes = new FiniteElementScheme[elements.Length];

        for (var i = 0; i < elements.Length; i++)
        {
            var element = elements[i];
            var scheme = new FiniteElementScheme(element.Nodes.Select(node => nodesIndexes[node]).ToArray(), element.FormulaNumber, AxisOrientation.XYZ);
            elementSchemes[i] = scheme;
        }

        return elementSchemes;
    }

    private static int[] GetFirstBoundaryNodes(SupportLine[] supportLines, Node[] nodes, Dictionary<Node, int> nodesIndexes, double[] zW, double[] xW, double[] yW, Dictionary<Node, int> globalIndexes)
    {
        var boundaryNodesIndexes = new List<int>();
        var innerPoints = supportLines.Select(line => line.inner);
        foreach (var innerPoint in innerPoints)
        {
            var boundaryNodes = nodes.Where(node => Math.Abs(node.X - innerPoint.x) < Tolerance && Math.Abs(node.Y - innerPoint.y) < Tolerance);
            boundaryNodesIndexes.AddRange(boundaryNodes.Select(boundaryNode => nodesIndexes[boundaryNode]));
        }

        var outerNodesX = ArraySegment<int>.Empty; // nodes.Select((node, i) => (node, i)).Where(pair => Math.Abs(pair.node.X - xW.First()) < TOLERANCE || Math.Abs(pair.node.X - xW.Last()) < TOLERANCE).Select(pair => pair.i);
        var outerNodesY = ArraySegment<int>.Empty; //nodes.Select((node, i) => (node, i)).Where(pair => Math.Abs(pair.node.Y - yW.First()) < TOLERANCE || Math.Abs(pair.node.Y - yW.Last()) < TOLERANCE).Select(pair => pair.i);
        var outerNodesZ = nodes.Where(node => Math.Abs(node.Z - zW.First()) < Tolerance || Math.Abs(node.Z - zW.Last()) < Tolerance);

        var firstBoundaryNodesGlobalIndexes = outerNodesZ.Select(node => globalIndexes[node]);

        return boundaryNodesIndexes.Concat(firstBoundaryNodesGlobalIndexes).Concat(outerNodesX).Concat(outerNodesY).Distinct().Order().ToArray();
    }

    /// <summary>
    /// Меняем нумерацию узлов с поправкой на глобальную
    /// </summary>
    /// <param name="finiteElements"></param>
    /// <param name="nodes">узлы из подобласти</param>
    /// <param name="globalNodesIndexes"></param>
    private static void TransformLocalNumericInGlobal(IList<FiniteElementScheme> finiteElements, IList<Node> nodes, Dictionary<Node, int> globalNodesIndexes)
    {
        foreach (var finiteElement in finiteElements)
            for (var i = 0; i < finiteElement.NodesIndexes.Length; i++)
                finiteElement.NodesIndexes[i] = globalNodesIndexes[nodes[finiteElement.NodesIndexes[i]]];
    }

    private static bool IsBoundaryNode(Node node, double[] x, double[] y) => x.Contains(node.X) && y.Contains(node.Y);

    private static SupportLine[] BuildSupportLines(WellArea wellArea, double[] x, double[] y)
    {
        var bottomSupportLines = new List<SupportLine>();
        var topSupportLines = new List<SupportLine>();
        var rightSupportLines = new List<SupportLine>();
        var leftSupportLines = new List<SupportLine>();

        for (var i = 0; i < x.Length / 2; i++)
        {
            var intersectionPoints = GetIntersectionPoints(wellArea.YBottom, wellArea.YTop, x[i], x[^(i + 1)], wellArea);
            var bottomSupportLine = ((x[i], wellArea.YBottom), intersectionPoints.second);
            var topSupportLine = ((x[^(i + 1)], wellArea.YTop), intersectionPoints.fisrt);
            bottomSupportLines.Add(bottomSupportLine);
            topSupportLines.Add(topSupportLine);
        }

        for (var i = x.Length / 2; i < x.Length; i++)
        {
            var intersectionPoints = GetIntersectionPoints(wellArea.YBottom, wellArea.YTop, x[i], x[^(i + 1)], wellArea);
            var bottomSupportLine = ((x[i], wellArea.YBottom), intersectionPoints.fisrt);
            var topSupportLine = ((x[^(i + 1)], wellArea.YTop), intersectionPoints.second);
            bottomSupportLines.Add(bottomSupportLine);
            topSupportLines.Add(topSupportLine);
        }

        for (var i = 1; i < y.Length - 1; i++)
        {
            var intersectionPoints = GetIntersectionPoints(y[i], y[^(i + 1)], wellArea.XLeft, wellArea.XRight, wellArea);
            var leftSupportLine = ((wellArea.XLeft, y[i]), intersectionPoints.second);
            var rightSupportLine = ((wellArea.XRight, y[^(i + 1)]), intersectionPoints.fisrt);
            leftSupportLines.Add(leftSupportLine);
            rightSupportLines.Add(rightSupportLine);
        }

        leftSupportLines.Reverse();
        rightSupportLines.Reverse();
        return bottomSupportLines.Concat(rightSupportLines).Concat(topSupportLines).Concat(leftSupportLines).ToArray();
    }

    private static (Point fisrt, Point second) GetIntersectionPoints(double y1, double y2, double x1, double x2, WellArea wellArea)
    {
        if (Math.Abs(x2 - x1) < Tolerance) return ((wellArea.XCenter, wellArea.YCenter - wellArea.WellDomainSettings.WellRadius), (wellArea.XCenter, wellArea.YCenter + wellArea.WellDomainSettings.WellRadius));

        var k = (y2 - y1) / (x2 - x1);
        var wellRadius = wellArea.WellDomainSettings.WellRadius;
        (var x0, var y0) = (wellArea.XCenter, wellArea.YCenter);
        var firstPointX = x0 + wellRadius / Math.Sqrt(1 + k * k);
        var firstPointY = y0 + k * wellRadius / Math.Sqrt(1 + k * k);
        var secondPointX = x0 - wellRadius / Math.Sqrt(1 + k * k);
        var secondPointY = y0 - k * wellRadius / Math.Sqrt(1 + k * k);

        return ((firstPointX, firstPointY), (secondPointX, secondPointY));
    }


    private static (FiniteElement[], Node[]) BuildElems(WellArea wellDomain, int stepsCount, double q, double[] z, SupportLine[] supportLines)
    {
        var elems = new List<FiniteElement>();
        var nodes = new List<Node>();

        // todo сработает только если количество разбиений по x у будет одинаковым
        var linesCountInQuarter = supportLines.Length / 4;
        for (var i = 0; i < supportLines.Length - 1; i++)
        {
            var res = BuildElementsBetweenTwoSupportLines(wellDomain, stepsCount, q, z, supportLines[i], supportLines[i + 1], GetQuarter(i, linesCountInQuarter));

            (var newElems, var newNodes) = res.Item1;
            elems.AddRange(newElems);
            nodes.AddRange(newNodes);
        }

        var elemsBetweenFirstAndLastLines = BuildElementsBetweenTwoSupportLines(wellDomain, stepsCount, q, z, supportLines[^1], supportLines[0], GetQuarter(supportLines.Length - 1, linesCountInQuarter));
        elems.AddRange(elemsBetweenFirstAndLastLines.Item1.Item1);
        return (elems.ToArray(), nodes.ToArray());
    }

    private static int GetQuarter(int firstLineNumber, int linesCountInQuarter) => firstLineNumber / linesCountInQuarter;

    private static ((FiniteElement[], Node[] nodes), Node[] boundaryNodes) BuildElementsBetweenTwoSupportLines(WellArea wellDomain, int stepsCount, double q, double[] z, SupportLine firstLine, SupportLine secondLine, int quarterNum)
    {
        var supportFunc1 = GetLinearFunc(firstLine.outer.y, firstLine.inner.y, firstLine.outer.x, firstLine.inner.x);
        var supportFunc2 = GetLinearFunc(secondLine.outer.y, secondLine.inner.y, secondLine.outer.x, secondLine.inner.x);

        var realStepsCount = stepsCount;

        // if (wellDomain.WellDomainSettings.Okopka)
        // {
        //     realStepsCount--;
        // }

        var step1 = AxisFactory.GetStep(q, firstLine.outer.x, firstLine.inner.x, realStepsCount);
        var step2 = AxisFactory.GetStep(q, secondLine.outer.x, secondLine.inner.x, realStepsCount);

        List<double> x1, x2;

        x1 = [..AxisFactory.GetPointsInInterval(step1, q, firstLine.outer.x, realStepsCount), firstLine.inner.x];
        x2 = [.. AxisFactory.GetPointsInInterval(step2, q, secondLine.outer.x, realStepsCount), secondLine.inner.x];

        if (wellDomain.WellDomainSettings.Okopka)
        {
            var reference = x1.Last();
            if (IsDescending(x1))
                x1.Insert(x1.Count - 1, reference + OkopkaStep);
            else
                x1.Insert(x1.Count - 1, reference - OkopkaStep);

            var reference2 = x2.Last();
            if (IsDescending(x2))
                x2.Insert(x2.Count - 1, reference2 + OkopkaStep);
            else
                x2.Insert(x2.Count - 1, reference2 - OkopkaStep);
        }

        //нумерация как с кэ, слева направо, снизу вверх
        var y1 = x1.Select(x => supportFunc1(x)).ToArray();
        var y2 = x2.Select(x => supportFunc2(x)).ToArray();

        y1[^1] = firstLine.inner.y;
        y2[^1] = secondLine.inner.y;

        y1 = y1.Select((y, i) => double.IsNaN(y) ? y2[i] : y).ToArray();
        y2 = y2.Select((y, i) => double.IsNaN(y) ? y1[i] : y).ToArray();

        var xy1 = x1.Zip(y1).ToArray(); //материализуем, чтобы 2 раза не вычислять коллекцию
        var xy2 = x2.Zip(y2);

        var xyz1 = GetPoints(z, xy1);
        var xyz2 = GetPoints(z, xy2);

        var xyLineLength = xy1.Length;

        //порядок нумерации как у кэ:
        // 3    4
        // 1    2
        return (
            GetNodesBetweenTwoSupportLines(wellDomain.FormulaNumber, z.Length, xyLineLength, xyz1, xyz2, quarterNum),
            [xyz1[z.Length - 1], xyz1[z.Length * 2 - 1], xyz2[z.Length - 1], xyz2[z.Length * 2 - 1]]
        );
    }

    private static (FiniteElement[], Node[]) GetNodesBetweenTwoSupportLines(int formulaNumber, int zLineLength, int xyLineLength, IList<Node> xyz1, IList<Node> xyz2, int quarterNum)
    {
        var elems = new List<FiniteElement>();
        var nodes = new List<Node>();
        for (var i = 0; i < zLineLength - 1; i++)
        for (var j = 0; j < xyLineLength - 1; j++)
        {
            var indexInLowerLine = j + i * xyLineLength; //индекс в нижней линии (z0) 
            var indexInUpperLine = j + (i + 1) * xyLineLength; //индекс в верхней линии (z1)

            var elemNodes = GetNodesByQuarter(quarterNum, xyz1, xyz2, indexInLowerLine, indexInUpperLine);

            var newElem = new FiniteElement(elemNodes, AxisOrientation.XYZ, formulaNumber);
            elems.Add(newElem);
            nodes.AddRange(elemNodes);
        }
        return (elems.ToArray(), nodes.ToArray());
    }

    private static Node[] GetNodesByQuarter(int quarterNum, IList<Node> xyz1, IList<Node> xyz2, int indexInLowerLine, int indexInUpperLine) => quarterNum switch
    {
        0 =>
        [
            xyz1[indexInLowerLine],
            xyz2[indexInLowerLine],
            xyz1[indexInLowerLine + 1],
            xyz2[indexInLowerLine + 1],
            xyz1[indexInUpperLine],
            xyz2[indexInUpperLine],
            xyz1[indexInUpperLine + 1],
            xyz2[indexInUpperLine + 1]
        ],

        1 =>
        [
            xyz1[indexInLowerLine + 1],
            xyz1[indexInLowerLine],
            xyz2[indexInLowerLine + 1],
            xyz2[indexInLowerLine],
            xyz1[indexInUpperLine + 1],
            xyz1[indexInUpperLine],
            xyz2[indexInUpperLine + 1],
            xyz2[indexInUpperLine]
        ],

        2 =>
        [
            xyz2[indexInLowerLine + 1],
            xyz1[indexInLowerLine + 1],
            xyz2[indexInLowerLine],
            xyz1[indexInLowerLine],
            xyz2[indexInUpperLine + 1],
            xyz1[indexInUpperLine + 1],
            xyz2[indexInUpperLine],
            xyz1[indexInUpperLine]
        ],

        3 =>
        [
            xyz2[indexInLowerLine],
            xyz2[indexInLowerLine + 1],
            xyz1[indexInLowerLine],
            xyz1[indexInLowerLine + 1],
            xyz2[indexInUpperLine],
            xyz2[indexInUpperLine + 1],
            xyz1[indexInUpperLine],
            xyz1[indexInUpperLine + 1]
        ],
        _ => throw new ArgumentOutOfRangeException(nameof(quarterNum), quarterNum, null)
    };

    private static Node[] GetPoints(double[] z, IEnumerable<(double First, double Second)> xy1) =>
        (from _z in z
            from xy in xy1
            select new Node(xy.First, xy.Second, _z)).ToArray();

    private static bool IsDescending(IList<double> points) => points[0] > points[1];

    private static Func<double, double> GetLinearFunc(double y1, double y2, double x1, double x2) => x => y1 + (y2 - y1) / (x2 - x1) * (x - x1);
}

public record WellArea : Area<double>
{
    public WellDomainSettings WellDomainSettings { get; }

    public double XCenter => (XRight + XLeft) / 2;

    public double YCenter => (YTop + YBottom) / 2;

    public double SourceZ { get; }

    public WellArea(double XLeft, double XRight, double YBottom, double YTop, double ZBack, double ZFront, int FormulaNumber, double sourceZ, WellDomainSettings wellDomainSettings) : base(XLeft, XRight, YBottom, YTop, ZBack, ZFront, FormulaNumber, EAreaType.Well)
    {
        WellDomainSettings = wellDomainSettings;
        SourceZ = sourceZ;
    }
}

public record WellDomainSettings(double WellRadius, bool Okopka);