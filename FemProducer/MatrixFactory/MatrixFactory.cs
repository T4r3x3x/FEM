using Grid.Models;

using MathModels.Models;

namespace FemProducer.MatrixBuilding
{
    public class MatrixFactory : IMatrixFactory
    {
        private double[] _di;
        private List<double> _al, _au;
        private List<int> _ja, _ia;

        private void Initialize(int nodesCount)
        {
            _di = new double[nodesCount];
            _ia = new List<int>(nodesCount + 1);
            _al = new List<double>();
            _au = new List<double>();
            _ja = new List<int>();
        }

        public int GetNodesCount() => _di.Length;

        public Matrix CreateMatrix(GridModel grid)
        {
            int nodesCount = grid.NodesCount;
            Initialize(nodesCount);

            int memory = nodesCount * 27;//завист от размерности задачи
            List<List<int>> list = new List<List<int>>(2);
            list.Add(new List<int>(memory));
            list.Add(new List<int>(memory));
            list[0].AddRange(Enumerable.Repeat(0, list[0].Capacity));
            list[1].AddRange(Enumerable.Repeat(0, list[1].Capacity));
            _ja = new List<int>(memory);
            List<int> listbeg = new List<int>(nodesCount);
            listbeg.AddRange(Enumerable.Repeat(0, listbeg.Capacity));
            int listSize = 0;

            foreach (var element in grid.Elements)
            {
                for (int i = 0; i < 8; i++)//завист от размерности задачи
                {
                    int k = element.NodesIndexes[i];

                    for (int j = i + 1; j < 8; j++)//завист от размерности задачи
                    {
                        int ind1 = k;

                        int ind2 = element.NodesIndexes[j];
                        if (ind2 < ind1)
                        {
                            ind1 = ind2;
                            ind2 = k;
                        }
                        int iaddr = listbeg[ind2];
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
            for (int i = 0; i < nodesCount; i++)
            {
                _ia.Add(_ia[i]);
                int iaddr = listbeg[i];
                while (iaddr != 0)
                {
                    _ja.Add(list[0][iaddr]);
                    _ia[i + 1]++;
                    iaddr = list[1][iaddr];
                }
            }
            _al = new List<double>(_ja.Count());
            _al.AddRange(Enumerable.Repeat(0.0, _al.Capacity));
            _au = new List<double>(_ja.Count());
            _au.AddRange(Enumerable.Repeat(0.0, _au.Capacity));

            return new Matrix(_di, _al.ToArray(), _au.ToArray(), _ja.ToArray(), _ia.ToArray());
        }
    }
}