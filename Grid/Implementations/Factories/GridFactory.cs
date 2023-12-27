using FemProducer;

using Grid.Interfaces;
using Grid.Models;

using MathModels;

using Tools;

namespace Grid.Implementations.Factories
{
	public class GridFactory : IGridFactory
	{
		private const int countOfNodesInElement = 4;

		public GridModel GetGrid(GridParameters gridParametrs)
		{
			(var XW, var YW) = GetSegmentsBoundaries(gridParametrs.LinesNodes);

			var ZW = gridParametrs.ZW;

			double[][] subDomains = new double[gridParametrs.Areas.Length][];

			var lines = gridParametrs.LinesNodes;
			var areas = gridParametrs.Areas;

			for (int i = 0; i < subDomains.Length; i++)
			{
				subDomains[i] = GetRectangleArea([lines[areas[i][2]][areas[i][0]],
					lines[areas[i][2]][areas[i][1]],
					lines[areas[i][3]][areas[i][0]],
					lines[areas[i][3]][areas[i][1]]]);
			}

			var x = BaseMethods.GetPointsInAxis(gridParametrs.Qx, XW, gridParametrs.XSplitsCount);
			var y = BaseMethods.GetPointsInAxis(gridParametrs.Qy, YW, gridParametrs.YSplitsCount);
			var z = BaseMethods.GetPointsInAxis(gridParametrs.Qz, ZW, gridParametrs.ZSplitsCount);
			var t = BaseMethods.GetPointsInAxis([gridParametrs.Qt], gridParametrs.TLimits, [gridParametrs.TSplitsCount]);

			(var nodes, var missingNodesIndexes) = GetNodes(gridParametrs.Areas, subDomains, gridParametrs.LinesNodes, x, y);

			var elements = GetElements(x, y, subDomains, missingNodesIndexes, gridParametrs.Areas);

			var boundaryNodes = GetBoundaryNodes(gridParametrs.BoundaryConditions, x, y, gridParametrs.XSplitsCount, gridParametrs.YSplitsCount, missingNodesIndexes);

			var realSubDomains = GetRealSubdomians(lines, gridParametrs.Areas);

			return new GridModel(elements, nodes, boundaryNodes.Item1, boundaryNodes.Item2, boundaryNodes.Item3, x.Length, y.Length, realSubDomains.ToArray(), 4, x, y, areas, t);
		}

		private List<Point[]> GetRealSubdomians(Point[][] lines, int[][] areas)
		{
			List<Point[]> subdomains = new();
			Point[] points;
			for (int i = 0; i < areas.Length; i++)
			{
				points = [lines[areas[i][2]][areas[i][0]],
					lines[areas[i][2]][areas[i][1]],
					lines[areas[i][3]][areas[i][0]],
					lines[areas[i][3]][areas[i][1]]];
				subdomains.Add(points);
			}

			return subdomains;
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

			return [minX, maxX, minY, maxY];
		}

		Node TransformRectangleToQuadrilateral(Node node, Point xLimits, Point yLimits, Point[] points)
		{
			Node newNode = new Node(0, 0);

			for (int j = 0; j < countOfNodesInElement; j++)
				newNode.X += FEM.Psi(j, node, xLimits, yLimits) * points[j].X;

			for (int j = 0; j < countOfNodesInElement; j++)
				newNode.Y += FEM.Psi(j, node, xLimits, yLimits) * points[j].Y;

			return newNode;
		}

		private int GetAreaNumber(double[][] subDomains, Node node)
		{
			for (int i = subDomains.Length - 1; i > -1; i--)//идём в обратном порядке чтобы не было бага, когда в качестве подобласти возвращается 0 (0 - вся расчётная область, которая уже вкл. подобласти)
				if (subDomains[i][2] <= node.Y && node.Y <= subDomains[i][3])
					if (subDomains[i][0] <= node.X && node.X <= subDomains[i][1])
						return i;

			return -1;
		}

		private (List<Node>, List<int>) GetNodes(int[][] areas, double[][] subDomains, Point[][] lines, double[] x, double[] y)
		{
			List<Node> nodes = new List<Node>();
			List<int> missingNodes = new List<int>();
			int missingNodesCount = 0;

			for (int i = 0; i < y.Length; i++)
				for (int j = 0; j < x.Length; j++)
				{
					Node node = new Node(x[j], y[i]);
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
			return [lines[areas[areaNumber][2]][areas[areaNumber][0]],
				lines[areas[areaNumber][2]][areas[areaNumber][1]], //вершины четырёхугольной подобласти
				lines[areas[areaNumber][3]][areas[areaNumber][0]],
				lines[areas[areaNumber][3]][areas[areaNumber][1]]];
		}

		private (double[], double[]) GetSegmentsBoundaries(Point[][] lines)
		{
			HashSet<double> xw = new(), yw = new();

			yw.Add(SearchingAlghoritms.GetMinValueInCollection(lines[0].Select(point => point.Y)));

			for (int i = 1; i < lines.Length - 1; i++)
				yw.Add(lines[i][0].Y);

			yw.Add(SearchingAlghoritms.GetMaxValueInCollection(lines[lines.Length - 1].Select(point => point.Y)));

			double min = double.MaxValue;
			for (int j = 0; j < lines[0].Length; j++)
			{
				if (lines[0][j].X < min)
					min = lines[0][j].X;

			}
			xw.Add(min);

			for (int i = 1; i < lines[0].Length - 1; i++)
				xw.Add(lines[0][i].X);

			double max = double.MinValue;
			for (int j = 0; j < lines[0].Length; j++)
			{
				if (lines[lines.Length - 1][j].X > max)
					max = lines[lines.Length - 1][j].X;
			}
			xw.Add(max);

			return (xw.ToArray(), yw.ToArray());
		}

		private (int, int) GetAxisLimitIndexes(int leftLimit, int rightLimit, List<int> SplitsCount)
		{
			int firstLimit = 0, secondLimit, i;

			for (i = 0; i < leftLimit; i++)
				firstLimit += SplitsCount[i] - 1;

			secondLimit = firstLimit;

			for (; i < rightLimit; i++)
				secondLimit += SplitsCount[i] - 1;

			return (firstLimit, secondLimit);
		}

		/// <summary>
		/// Возвращает индексы x'ов и y'ов которые являются границами указанной подобласти
		/// </summary>
		/// <param name="boundaryConditions"></param>
		/// <param name="xSplitsCount"></param>
		/// <param name="ySplitsCount"></param>
		/// <returns></returns>
		private IList<int> GetAreaLimitIndexes(IList<int> limits, List<int> xSplitsCount, List<int> ySplitsCount)
		{
			var xLimits = GetAxisLimitIndexes(limits[0], limits[1], xSplitsCount);
			var yLimits = GetAxisLimitIndexes(limits[2], limits[3], ySplitsCount);

			return [xLimits.Item1, xLimits.Item2, yLimits.Item1, yLimits.Item2];
		}

		private IList<IList<int>> GetBoundaryLimitsIndexes(IList<IList<int>> boundaryConditionsLimits, List<int> xSplitsCount, List<int> ySplitsCount)
		{
			IList<IList<int>> boundaryLimitsIndexes = new List<IList<int>>();

			for (int i = 0; i < boundaryConditionsLimits.Count; i++)
			{
				boundaryLimitsIndexes.Add(GetAreaLimitIndexes(boundaryConditionsLimits[i], xSplitsCount, ySplitsCount));
			}

			return boundaryLimitsIndexes;
		}

		private (HashSet<int>, IList<(IList<int>, int)>, IList<(IList<int>, int)>) GetBoundaryNodes(IList<IList<int>> boundaryIndexes, IList<double> x, IList<double> y, List<int> xSplitsCount, List<int> ySplitsCount, IList<int> missingNodesIndexes)
		{
			HashSet<int> firstBoundaryNodes = new HashSet<int>();
			List<(IList<int>, int)> secondBoundaryNodes = new();
			List<(IList<int>, int)> thirdBoundaryNodes = new();
			var limits = GetBoundaryLimitsIndexes(boundaryIndexes, xSplitsCount, ySplitsCount);


			//for (int i = 0; i < y.Count - 1; i++)//левая грань
			//	firstBoundaryNodes.Add(i * x.Count);
			//for (int i = 0; i < x.Count; i++)//нижняя грань
			//	firstBoundaryNodes.Add(i);
			//for (int i = 0; i < x.Count; i++)//верхняя грань
			//	firstBoundaryNodes.Add(x.Count * (y.Count - 1) + i);

			//for (int i = 0; i < y.Count - 1; i++)//правая грань
			//	firstBoundaryNodes.Add(x.Count * (i + 1) - 1);

			for (int i = 0; i < x.Count - 1; i++)//нижняя грань
				thirdBoundaryNodes.Add(([i, i + 1], 0));

			for (int i = 0; i < x.Count - 1; i++)//верхняя грань
				thirdBoundaryNodes.Add(([x.Count * (y.Count - 1) + i, x.Count * (y.Count - 1) + i + 1], 1));

			for (int i = 0; i < y.Count - 1; i++)//правая грань
				thirdBoundaryNodes.Add(([x.Count * (i + 1) - 1, x.Count * (i + 2) - 1], 2));
			return (firstBoundaryNodes, secondBoundaryNodes, thirdBoundaryNodes);
		}

		private List<FiniteElement> GetElements(double[] x, double[] y, double[][] subDomains, List<int> missingNodes, int[][] areas)
		{
			List<FiniteElement> elements = new List<FiniteElement>();
			int[] NodesIndexes = new int[4];

			for (int j = 0; j < y.Length - 1; j++)
			{
				int jx0 = j * x.Length;
				int jx1 = (j + 1) * x.Length;

				for (int i = 0; i < x.Length - 1; i++)
				{
					var node = new Node((x[i] + x[i + 1]) / 2.0, (y[j] + y[j + 1]) / 2.0);
					int areaNumber = GetAreaNumber(subDomains, node);
					if (areaNumber != -1)
					{
						NodesIndexes = [jx0 + i, jx0 + i + 1, jx1 + i, jx1 + i + 1];

						for (int k = 0; k < NodesIndexes.Length; k++)
							NodesIndexes[k] -= missingNodes[NodesIndexes[k]];

						elements.Add(new FiniteElement(NodesIndexes, areas[areaNumber][4]));
					}
				}
			}
			return elements;
		}
	}
}