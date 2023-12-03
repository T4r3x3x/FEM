using ReaserchPaper.Grid;

using ResearchPaper;

namespace Tensus
{
	public class MatrixFactory
	{
		private double[] _di;
		private List<double> _al, _au;
		private List<int> _ja, _ia;

		private readonly Grid _grid;

		public MatrixFactory(Grid grid)
		{
			_grid = grid;
		}

		private void Initialize(int nodesCount)
		{
			_di = new double[nodesCount];
			_ia = new List<int>(nodesCount + 1);
			_al = new List<double>();
			_au = new List<double>();
			_ja = new List<int>();
		}
		public int GetNodesCount()
		{
			return _di.Length;
		}

		public Matrix CreateMatrix()
		{
			int nodesCount = _grid.NodesCount;
			int elemCount = _grid.ElementsCount;
			Initialize(nodesCount);

			int memory = nodesCount * 8;
			List<List<int>> list = new List<List<int>>(2);
			list[0].AddRange(Enumerable.Repeat(0, list[0].Capacity));
			list[1].AddRange(Enumerable.Repeat(0, list[1].Capacity));
			_ja = new List<int>(memory);
			List<int> listbeg = new List<int>(nodesCount);
			listbeg.AddRange(Enumerable.Repeat(0, listbeg.Capacity));
			int listSize = 0;

			for (int t = 0; t < elemCount; t++)
			{
				for (int i = 0; i < 4; i++)
				{
					int k = _grid.GetNodeGlobalNumber(i, t);

					for (int j = i + 1; j < 4; j++)
					{
						int ind1 = k;

						int ind2 = _grid.GetNodeGlobalNumber(j, t);
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