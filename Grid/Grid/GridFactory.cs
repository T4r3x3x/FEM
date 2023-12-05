using FemProducer.DTO;

namespace ReaserchPaper.Grid
{
	internal class GridFactory
	{
		//private double[] _t;
		//private double[] _x, _y, _hy, _hx, _ht;
		//private int[][] _boreholes;

		//private int[] _IX, _IY;
		//private int[][] _areas;
		//private int _n, _m;

		//private double _h;
		//private double[] XW, YW;
		//private double[] _q;
		//private double q;

		public GridFactory()
		{


		}

		private List<Grid.Node> GetNodes(double[] x, double[] y)
		{
			List<Grid.Node> nodes = new List<Grid.Node>();

			for (int i = 0; i < y.Length; i++)
				for (int j = 0; j < x.Length; j++)
					nodes.Add(new Grid.Node(x[j], y[i]));

			return nodes;
		}

		private int[] GetIndexes(int nodeIndex, int countOfNodesInHorizontalLine, int countOfNodesInElement)
		{
			var indexes = new int[countOfNodesInElement];
			indexes[0] = nodeIndex;
			indexes[1] = nodeIndex + 1;
			indexes[2] = nodeIndex + countOfNodesInHorizontalLine;
			indexes[3] = nodeIndex + countOfNodesInHorizontalLine + 1;
			return indexes;
		}

		private List<Grid.Element> GetElements(List<Grid.Node> nodes, int countOfNodesInHorizontalLine, int countOfNodesInElement)
		{
			List<Grid.Element> elements = new List<Grid.Element>();

			for (int i = 0; i < nodes.Count - countOfNodesInHorizontalLine - 1; i++)
			{
				if ((i + 1) % countOfNodesInHorizontalLine == 0)
					i++;
				var element = new Grid.Element(GetIndexes(i, countOfNodesInHorizontalLine, countOfNodesInElement));
				elements.Add(element);
			}
			return elements;
		}


		//toDo вынести всё это в отдельный метод.
		public Grid GetGrid(GridParameters gridParametrs)
		{
			double h;
			int startPos = 0;
			var n = 0;
			foreach (var item in gridParametrs.xSplitsCount)
				n += item;

			var m = 0;
			foreach (var item in gridParametrs.ySplitsCount)
				m += item;

			var result = MakeSplit(n, gridParametrs.qx, gridParametrs.XW, gridParametrs.xSplitsCount);
			var IX = result.Item1;
			var hx = result.Item2;
			var x = result.Item3;

			result = MakeSplit(m, gridParametrs.qy, gridParametrs.YW, gridParametrs.ySplitsCount);
			var IY = result.Item1;
			var hy = result.Item2;
			var y = result.Item3;

			var nodes = GetNodes(x, y);
			var elements = GetElements(nodes, n, 4);

			//if (q == 1)
			//	_h = (_t[layersCount - 1] - _t[0]) / (layersCount - 1);
			//else
			//	_h = (_t[layersCount - 1] - _t[0]) * (q - 1) / (Math.Pow(q, layersCount - 1) - 1);

			//for (int i = 1; i < layersCount; i++)
			//{
			//	_t[i] = _t[i - 1] + _h;
			//	_ht[i - 1] = _h;
			//	_h *= q;
			//}
			return new Grid(new double[] { 0 }, x, y, hy, hx, null, null, IX, IY, gridParametrs.areas, n, m, elements, nodes);
		}


		private (int[], double[], double[]) MakeSplit(int size, double[] q, double[] W, IList<int> splitsCount)
		{
			double h = 0;
			int startPos = 0;
			var steps = new double[size - 1];
			var points = new double[size];
			var I = new int[q.Length + 1];
			points[0] = W[0];

			for (int i = 0; i < q.Length; i++)
			{
				if (q[i] == 1)
					h = (W[i + 1] - W[i]) / (splitsCount[i] - 1);
				else
				{
					h = (W[i + 1] - W[i]) * (q[i] - 1) / (Math.Pow(q[i], splitsCount[i] - 1) - 1);
				}

				MakeArea(h, points, steps, startPos, splitsCount[i], q[i]);
				points[startPos + splitsCount[i] - 1] = W[i + 1];
				startPos += splitsCount[i] - 1;
				I[i + 1] = I[i] + splitsCount[i] - 1;
			}
			return (I, steps, points);
		}


		void MakeArea(double _h, double[] points, double[] steps, int startPos, int areaLenght, double _q)//режим область на части и записываем в массив, _h - шаг,  j - номер подобласти
		{
			areaLenght--;
			for (int j = startPos; j < areaLenght + startPos; j++)
			{
				points[j + 1] = points[j] + _h;
				steps[j] = _h;
				_h *= _q;
			}
		}

	}
}
