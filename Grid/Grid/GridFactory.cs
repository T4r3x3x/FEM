using System.Diagnostics;

using FemProducer.DTO;

namespace ReaserchPaper.Grid
{
	internal class GridFactory
	{
		private const int boundariesInLineCount = 2;
		private const int countOfNodesInElement = 4;

		private int GetAreaNumber(double[][] subDomains, Grid.Node node)
		{
			for (int i = subDomains.Length - 1; i > -1; i--)//идём в обратном порядке чтобы не было бага, когда в качестве подобласти возвращается 0 (0 - вся расчётная область, которая уже вкл. подобласти)
				if (subDomains[i][2] <= node.Y && node.Y <= subDomains[i][3])
					if (subDomains[i][0] <= node.X && node.X <= subDomains[i][1])
						return i;

			return -1;
		}

		private (List<Grid.Node>, List<int>) GetNodes(int[][] areas, double[][] subDomains, Point[][] lines, double[] x, double[] y)
		{
			List<Grid.Node> nodes = new List<Grid.Node>();
			List<int> missingNodes = new List<int>();
			int missingNodesCount = 0;
			Grid.Node node;
			for (int i = 0; i < y.Length; i++)
				for (int j = 0; j < x.Length; j++)
				{
					node = new Grid.Node(x[j], y[i]);
					var area = GetAreaNumber(subDomains, node);
					if (area != -1)
					{
						var points = GetQuadrilateralPoints(lines, area, areas);

						var limits = GetRectangleArea(points);
						var xLimits = new Point(limits[0], limits[1]);//вершины прямоугольной подобласти
						var yLimits = new Point(limits[2], limits[3]);

						node = TransformRectangleToQuadrilateral(node, xLimits, yLimits, points);
						nodes.Add(node);
					}
					else
						missingNodesCount++;
					missingNodes.Add(missingNodesCount);
				}
			return (nodes, missingNodes);
		}

		private Point[] GetQuadrilateralPoints(Point[][] lines, int areaNumber, int[][] areas)
		{
			return new Point[] { lines[areas[areaNumber][2]][areas[areaNumber][0]], lines[areas[areaNumber][2]][areas[areaNumber][1]], //вершины четырёхугольной подобласти
							lines[areas[areaNumber][3]][areas[areaNumber][0]], lines[areas[areaNumber][3]][areas[areaNumber][1]] };
		}

		private (double[], double[]) GetSegmetnsBoundaries(Point[][] lines)
		{
			HashSet<double> XW = new(), YW = new();
			for (int i = 0; i < lines.Length; i++)
			{
				for (int j = 0; j < lines[0].Length; j++)
				{
					XW.Add(lines[i][j].X);
					YW.Add(lines[i][j].X);
				}
			}
			return (XW.ToArray(), YW.ToArray());
		}

		private List<Grid.Element> GetElements(double[] x, double[] y, double[][] subDomains, List<int> missingNodes)
		{
			List<Grid.Element> elements = new List<Grid.Element>();
			int[] indexes = new int[4];
			for (int j = 0; j < y.Length - 1; j++)
			{
				int jx_0 = j * x.Length;
				int jx_1 = (j + 1) * x.Length;

				for (int i = 0; i < x.Length - 1; i++)
				{
					var node = new Grid.Node((x[i] + x[i + 1]) / 2.0, (y[j] + y[j + 1]) / 2.0);
					if (GetAreaNumber(subDomains, node) != -1)
					{
						indexes = new int[4] { jx_0 + i, jx_0 + i + 1, jx_1 + i, jx_1 + i + 1 };
						for (int k = 0; k < indexes.Length; k++)
							indexes[k] -= missingNodes[indexes[k]];
						elements.Add(new Grid.Element(indexes));
					}
				}
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
				n += item - 1;

			var m = 0;
			foreach (var item in gridParametrs.ySplitsCount)
				m += item - 1;

			n++;
			m++;

			var boundaries = GetSegmetnsBoundaries(gridParametrs.linesNodes);
			double[] XW = boundaries.Item1;
			double[] YW = boundaries.Item2;

			double[][] subDomains = new double[gridParametrs.linesNodes.Length][];

			var lines = gridParametrs.linesNodes;
			var areas = gridParametrs.areas;

			for (int i = 0; i < subDomains.Length; i++)
			{
				subDomains[i] = GetRectangleArea(new Point[] { lines[areas[i][2]][areas[i][0]], lines[areas[i][2]][areas[i][1]],
							lines[areas[i][3]][areas[i][0]], lines[areas[i][3]][areas[i][1]] });
			}
			///	subDomains = new double[][] {}

			var result = MakeSplit(n, gridParametrs.qx, XW, gridParametrs.xSplitsCount);
			var IX = result.Item1;
			var hx = result.Item2;
			var x = result.Item3;

			result = MakeSplit(m, gridParametrs.qy, YW, gridParametrs.ySplitsCount);
			var IY = result.Item1;
			var hy = result.Item2;
			var y = result.Item3;

			var nodeses = GetNodes(gridParametrs.areas, subDomains, gridParametrs.linesNodes, x, y);
			var nodes = nodeses.Item1;
			var elements = GetElements(x, y, subDomains, nodeses.Item2);

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
			using (StreamWriter sw = new StreamWriter("grid2.txt"))
			{
				sw.Write(string.Format("{0} {1} {2} {3}", XW[0] - 1, XW[XW.Length - 1] + 1, YW[0] - 1, YW[YW.Length - 1] + 1));
				sw.Write('\n');
				sw.WriteLine(elements.Count);
				foreach (var element in elements)
				{
					sw.WriteLine(nodes[element.indexes[0]].ToString().Replace(",", "."));
					sw.WriteLine(nodes[element.indexes[1]].ToString().Replace(",", "."));
					sw.WriteLine(nodes[element.indexes[3]].ToString().Replace(",", "."));
					sw.WriteLine(nodes[element.indexes[2]].ToString().Replace(",", "."));
					sw.WriteLine();
				}
			}

			Start();

			return new Grid(new double[] { 0 }, x, y, hy, hx, null, null, IX, IY, gridParametrs.areas, n, m, elements, nodes);
		}

		void Start()
		{
			ProcessStartInfo start = new ProcessStartInfo();
			start.FileName = "C:\\Python\\python.exe";
			start.Arguments = string.Format("grid2.py");
			start.UseShellExecute = false;
			start.RedirectStandardOutput = true;
			Process.Start(start);
		}

		/// <summary>
		/// Метод определяет границы прямоугольной области, в которую вписана данная четырёхугольная область
		/// </summary>
		/// <param name="points">вершины четырёхугольника</param>
		private double[] GetRectangleArea(Point[] points)
		{
			double maxX = points[0].X, minX = points[0].X, maxY = points[0].Y, minY = points[0].Y;
			foreach (var point in points)
			{
				if (point.Y < minY)
					minY = point.Y;
				else if (point.Y > maxY)
					maxY = point.Y;
				if (point.X < minX)
					minX = point.X;
				else if (point.X > maxX)
					maxX = point.X;
			}

			return new double[4] { minX, maxX, minY, maxY };
		}

		Grid.Node TransformRectangleToQuadrilateral(Grid.Node node, Point xLimits, Point yLimits, Point[] points)
		{
			Grid.Node newNode = new Grid.Node(0, 0);

			for (int j = 0; j < countOfNodesInElement; j++)
				newNode.X += FEM.Psi(j, node, xLimits, yLimits) * points[j].X;

			for (int j = 0; j < countOfNodesInElement; j++)
			{
				double res = FEM.Psi(j, node, xLimits, yLimits) * points[j].Y;
				newNode.Y += res;
			}
			return newNode;
		}

		private (int[], double[], double[]) MakeSplit(int size, double[] q, double[] W, IList<int> splitsCount)
		{
			double h;
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