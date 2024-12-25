using Grid.Models;

using MathModels.Models;

namespace FemProducer.MatrixFactory;

public class MatrixFactory : IMatrixFactory
{
    private double[] _di;
    private List<double> _al = [], _au = [];
    private List<int> _ja = [], _ia;

    public Matrix CreateMatrix(GridModel grid)
    {
        var countOfNodesInElement = grid.NodesInElementCount;
        var nodesCount = grid.NodesCount;
        var listSize = 0;

        (var listbeg, var list) = Initialize(nodesCount, countOfNodesInElement);
        MakePortrait(grid, countOfNodesInElement, nodesCount, listSize, listbeg, list);
        return new(_di, [.. _al], [.. _au], [.. _ja], [.. _ia]);
    }

    private void MakePortrait(GridModel grid, int countOfNodesInElement, int nodesCount, int listSize, int[] listbeg, int[][] list)
    {
        foreach (var element in grid.Elements)
        {
            for (var i = 0; i < countOfNodesInElement; i++)
            {
                var k = element.NodesIndexes[i];

                for (var j = i + 1; j < countOfNodesInElement; j++)
                {
                    var ind1 = k;

                    var ind2 = element.NodesIndexes[j];
                    if (ind2 < ind1)
                    {
                        ind1 = ind2;
                        ind2 = k;
                    }
                    var iaddr = listbeg[ind2];
                    if (iaddr == 0)
                    {
                        listSize++;
                        listbeg[ind2] = listSize;
                        list[0][listSize] = ind1;
                        list[1][listSize] = 0;
                    }
                    else
                    {
                        while (list[0][iaddr] < ind1 && list[1][iaddr] > 0)
                        {
                            iaddr = list[1][iaddr];
                        }
                        if (list[0][iaddr] > ind1)
                        {
                            listSize++;
                            list[0][listSize] = list[0][iaddr];
                            list[1][listSize] = list[1][iaddr];
                            list[0][iaddr] = ind1;
                            list[1][iaddr] = listSize;
                        }
                        else
                        {
                            if (list[0][iaddr] < ind1)
                            {
                                listSize++;
                                list[1][iaddr] = listSize;
                                list[0][listSize] = ind1;
                                list[1][listSize] = 0;
                            }
                        }
                    }
                }
            }
        }

        _ia.Add(0);
        for (var i = 0; i < nodesCount; i++)
        {
            _ia.Add(_ia[i]);
            var iaddr = listbeg[i];
            while (iaddr != 0)
            {
                _ja.Add(list[0][iaddr]);
                _ia[i + 1]++;
                iaddr = list[1][iaddr];
            }
        }
        _al = new(_ja.Count);
        _al.AddRange(Enumerable.Repeat(0.0, _al.Capacity));
        _au = new(_ja.Count);
        _au.AddRange(Enumerable.Repeat(0.0, _au.Capacity));
    }

    private static int GetNeighboursCount(int nodesInElementCount) => nodesInElementCount switch
    {
        2 => 2,
        4 => 5,
        8 => 26,
        _ => throw new ArgumentException($"Invalid value -- {nodesInElementCount}!")
    };

    private (int[], int[][]) Initialize(int nodesCount, int countOfNodesInElement)
    {
        var neighboursCount = GetNeighboursCount(countOfNodesInElement);
        var memory = nodesCount * neighboursCount;
        _di = new double[nodesCount];
        _ia = new(nodesCount + 1);
        _ja = new(memory);
        var listbeg = new int[nodesCount];
        int[][] list = [new int[memory], new int[memory]];
        return (listbeg, list);
    }
}