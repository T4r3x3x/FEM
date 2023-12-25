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

		public GridModel GetGrid(GridParameters gridParameters)
		{
			(var XW, var YW) = GetSegmentsBoundaries(gridParameters.LinesNodes);

			var ZW = gridParameters.ZW;

			double[][] subDomains = new double[gridParameters.Areas.Length][];

			var lines = gridParameters.LinesNodes;
			var areas = gridParameters.Areas;

			for (int i = 0; i < areas.Length; i++)
			{
				subDomains[i] = [lines[areas[i][2]][areas[i][0]].X,
					lines[areas[i][3]][areas[i][1]].X,
					lines[areas[i][2]][areas[i][1]].Y,
					lines[areas[i][3]][areas[i][1]].Y,
					ZW[areas[i][4]],
					ZW[areas[i][5]]];

			}
			//for (int i = 0; i < subDomains.Length; i++)
			//{
			//	subDomains[i] = GetRectangleArea([lines[areas[i][2]][areas[i][0]],
			//		lines[areas[i][2]][areas[i][1]],
			//		lines[areas[i][3]][areas[i][0]],
			//		lines[areas[i][3]][areas[i][1]]]);
			//}

			var x = BaseMethods.GetPointsInAxis(gridParameters.Qx, XW, gridParameters.XSplitsCount);
			var y = BaseMethods.GetPointsInAxis(gridParameters.Qy, YW, gridParameters.YSplitsCount);
			var z = BaseMethods.GetPointsInAxis(gridParameters.Qz, ZW, gridParameters.ZSplitsCount);
			var t = BaseMethods.GetPointsInAxis([gridParameters.Qt], gridParameters.TLimits, [gridParameters.TSplitsCount]);

			(var nodes, var missingNodesIndexes) = GetNodes(gridParameters.Areas, subDomains, gridParameters.LinesNodes, x, y, z);

			var elements = GetElements(x, y, z, subDomains, missingNodesIndexes, gridParameters.Areas);

			(var firstBoundaryNodes, var second, var third) = GetBoundaryNodes(gridParameters.BoundaryConditions, x, y, gridParameters.XSplitsCount, gridParameters.YSplitsCount, gridParameters.ZSplitsCount, missingNodesIndexes, subDomains, areas);


			//	var realSubDomains = GetRealSubdomians(lines, gridParameters.Areas);

			return new GridModel(elements, nodes, firstBoundaryNodes, second, third, x.Length, y.Length, subDomains, 8, x, y, z, t);
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
				if (subDomains[i][4] <= node.Z && node.Z < subDomains[i][5])
					if (subDomains[i][2] <= node.Y && node.Y <= subDomains[i][3])
						if (subDomains[i][0] <= node.X && node.X <= subDomains[i][1])
							return i;
			return -1;
		}

		private (List<Node>, List<int>) GetNodes(int[][] areas, double[][] subDomains, Point[][] lines, double[] x, double[] y, double[] z)
		{
			List<Node> nodes = new List<Node>();
			List<int> missingNodes = new List<int>();
			int missingNodesCount = 0;
			for (int k = 0; k < z.Length; k++)
				for (int i = 0; i < y.Length; i++)
					for (int j = 0; j < x.Length; j++)
					{
						Node node = new Node(x[j], y[i], z[k]);
						var area = GetAreaNumber(subDomains, node);
						//if (area != -1)
						//{
						//	var points = GetQuadrilateralPoints(lines, area, areas);

						//	var limits = GetRectangleArea(points);
						//	var xLimits = new Point(limits[0], limits[1]);//вершины прямоугольной подобласти
						//	var yLimits = new Point(limits[2], limits[3]);

						//	node = TransformRectangleToQuadrilateral(node, xLimits, yLimits, points);
						nodes.Add(node);
						//}
						//else
						//	missingNodesCount++;

						//missingNodes.Add(missingNodesCount);
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
		private IList<int> GetAreaLimitIndexes(IList<int> limits, List<int> xSplitsCount, List<int> ySplitsCount, List<int> zSplitsCount)
		{
			var xLimits = GetAxisLimitIndexes(limits[0], limits[1], xSplitsCount);
			var yLimits = GetAxisLimitIndexes(limits[2], limits[3], ySplitsCount);
			var zLimits = GetAxisLimitIndexes(limits[4], limits[5], zSplitsCount);

			return [xLimits.Item1, xLimits.Item2, yLimits.Item1, yLimits.Item2, zLimits.Item1, zLimits.Item2];
		}

		private IList<IList<int>> GetBoundaryLimitsIndexes(IList<IList<int>> boundaryConditionsLimits, List<int> xSplitsCount, List<int> ySplitsCount, List<int> zSplitsCount)
		{
			IList<IList<int>> boundaryLimitsIndexes = new List<IList<int>>();

			for (int i = 0; i < boundaryConditionsLimits.Count; i++)
				boundaryLimitsIndexes.Add(GetAreaLimitIndexes(boundaryConditionsLimits[i], xSplitsCount, ySplitsCount, zSplitsCount));

			return boundaryLimitsIndexes;
		}

		private (HashSet<int>, IList<IList<int>>, IList<IList<int>>) GetBoundaryNodes(IList<IList<int>> boundaryIndexes, IList<double> x, IList<double> y, List<int> xSplitsCount, List<int> ySplitsCount, List<int> zSplitsCount, IList<int> missingNodesIndexes, double[][] subDomains,
			int[][] areas)
		{
			HashSet<int> boundaryNodes = new HashSet<int>();
			HashSet<int> secondBoundaryNodes = new HashSet<int>();
			List<List<int>> secondBoundaryIndexes = new();
			var limits = GetBoundaryLimitsIndexes(boundaryIndexes, xSplitsCount, ySplitsCount, zSplitsCount);

			for (int t = 0; t < boundaryIndexes.Count; t++)
			{
				switch (boundaryIndexes[t][6])
				{
					case 0:
						{
							for (int k = limits[t][4]; k <= limits[t][5]; k++)
								for (int i = limits[t][2]; i <= limits[t][3]; i++)
									for (int j = limits[t][0]; j <= limits[t][1]; j++)
									{
										var index = k * x.Count * y.Count + i * x.Count + j;
										var realIndex = index;// - missingNodesIndexes[index];
										boundaryNodes.Add(realIndex);
									}
						}
						break;
					case 1:
						{
							for (int k = limits[t][4]; k <= limits[t][5]; k++)
								for (int i = limits[t][2]; i <= limits[t][3]; i++)
									for (int j = limits[t][0]; j <= limits[t][1]; j++)
									{
										var index = k * x.Count * y.Count + i * x.Count + j;
										var realIndex = index;// - missingNodesIndexes[index];
										secondBoundaryNodes.Add(realIndex);
									}

							for (int j = 0; j < y.Count - 1; j++)
							{
								int jx0 = j * x.Count;
								int jx1 = (j + 1) * x.Count;

								for (int i = 0; i < x.Count - 1; i++)
								{
									var node = new Node((x[i] + x[i + 1]) / 2.0, (y[j] + y[j + 1]) / 2.0);
									int areaNumber = GetAreaNumber(subDomains, node);

									List<int> NodesIndexes = [jx0 + i,
										jx0 + i + 1,
										jx1 + i,
										jx1 + i + 1,
										areas[areaNumber][6]];

									//for (int k = 0; k < NodesIndexes.Length; k++)
									//	NodesIndexes[k] -= missingNodes[NodesIndexes[k]];

									secondBoundaryIndexes.Add(NodesIndexes);

								}
							}
							break;
						}
				}
			}

			return (boundaryNodes, secondBoundaryIndexes.ToArray(), null);
		}

		private List<FiniteElement> GetElements(double[] x, double[] y, double[] z, double[][] subDomains, List<int> missingNodes, int[][] areas)
		{
			List<FiniteElement> elements = new List<FiniteElement>();
			int[] NodesIndexes = new int[8];
			for (int k = 0; k < z.Length - 1; k++)
			{
				int kxy0 = k * x.Length * y.Length;
				int kxy1 = (k + 1) * x.Length * y.Length;

				for (int j = 0; j < y.Length - 1; j++)
				{
					int jx0 = j * x.Length;
					int jx1 = (j + 1) * x.Length;

					for (int i = 0; i < x.Length - 1; i++)
					{
						var node = new Node((x[i] + x[i + 1]) / 2.0, (y[j] + y[j + 1]) / 2.0, (z[k] + z[k + 1]) / 2.0);
						int areaNumber = GetAreaNumber(subDomains, node);
						if (areaNumber != -1)
						{
							NodesIndexes = [kxy0 + jx0 + i,
								kxy0 + jx0 + i + 1,
								kxy0 + jx1 + i,
								kxy0 + jx1 + i + 1,
								kxy1 + jx0 + i,
								kxy1 + jx0 + i + 1,
								kxy1 + jx1 + i,
								kxy1 + jx1 + i + 1];

							//for (int k = 0; k < NodesIndexes.Length; k++)
							//	NodesIndexes[k] -= missingNodes[NodesIndexes[k]];

							elements.Add(new FiniteElement(NodesIndexes, areas[areaNumber][6]));
						}
					}
				}
			}
			return elements;
		}
	}
}