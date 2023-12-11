using Grid.Models;

namespace FemProducer.Basises
{
	internal class LinearRectangularBasis : IBasis
	{
		public const int NodesCount = 4;

		private static double Y1(double y, double yUpper, double hy) => (yUpper - y) / hy;
		private static double Y2(double y, double yLower, double hy) => (y - yLower) / hy;
		private static double X1(double X, double xRight, double hx) => (xRight - X) / hx;
		private static double X2(double X, double xLeft, double hx) => (X - xLeft) / hx;

		private static double[,] G = new double[,]
		{
			{1,-1 },
			{-1,1 },
		};
		private static double[,] M = new double[,]
		{
			 { 2, 1 },
			{ 1, 2 }
		};

		private static int mu(int i) => i % 2;
		private static int nu(int i) => i / 2;

		public IList<IList<double>> GetMassMatrix(IList<Node> nodes)// Grid.M - номер кэ 
		{
			// инициализация
			double[][] result = new double[M.LongLength][];
			for (int i = 0; i < result.Length; i++)
				result[i] = new double[result.Length];

			//матрица масс
			for (int i = 0; i < result.Length; i++)
				for (int j = 0; j < result.Length; j++)
					result[i][j] += M[mu(i), mu(j)] * hx / 6 * M[nu(i), nu(j)] * hy / 6;

			return result;
		}

		public IList<IList<double>> GetStiffnessMatrix(IList<Node> nodes)// Grid.M - номер кэ 
		{
			// инициализация
			double[][] result = new double[G.LongLength][];
			for (int i = 0; i < result.Length; i++)
				result[i] = new double[result.Length];

			//матрица жесткости
			for (int i = 0; i < result.Length; i++)
				for (int j = 0; j < result.Length; j++)
					result[i][j] = G[mu(i), mu(j)] / hx * M[nu(i), nu(j)] * hy / 6 + M[mu(i), mu(j)] * hx / 6 * G[nu(i), nu(j)] / hy;

			return result;
		}

		public IList<double> GetLocalVector(IList<Node> nodes, Func<Node, double> func)
		{
			double[] result = new double[NodesCount];

			var hx = nodes[1].X - nodes[0].X;
			var hy = nodes[2].Y - nodes[0].Y;

			var funcValues = new double[NodesCount];
			for (int i = 0; i < NodesCount; i++)
				funcValues[i] = func(nodes[i]);

			result[0] = hx * hy / 36 * (4 * funcValues[0] + 2 * funcValues[1] + 2 * funcValues[2] + funcValues[3]);
			result[1] = hx * hy / 36 * (2 * funcValues[0] + 4 * funcValues[1] + funcValues[2] + 2 * funcValues[3]);
			result[2] = hx * hy / 36 * (2 * funcValues[0] + funcValues[1] + 4 * funcValues[2] + 2 * funcValues[3]);
			result[3] = hx * hy / 36 * (funcValues[0] + 2 * funcValues[1] + 2 * funcValues[2] + 4 * funcValues[3]);

			return result;
		}

		//public static double SolutionInPoint(Point point)
		//{
		//	double result = 0;
		//	Master.Slau.p.Elements[elemNumber] * X1() * Y1(point.y, yBoundaries.y, hy)


		//	return result;
		//}


		//public static Point GetV(FiniteElement element, Point point)
		//{
		//	double _x, _y;
		//	_x = -Master.Slau.p.Elements[elemNumber] * Y1(point.y, yBoundaries.y, hy) / hx + Master.Slau.p.Elements[elemNumber + 1] * Y1(point.y, yBoundaries.y, hy) / hx
		//	- Master.Slau.p.Elements[elemNumber + Grid.N] * Y2(point.y, yBoundaries.X, hy) / hx + Master.Slau.p.Elements[elemNumber + Grid.N + 1] * Y2(point.y, yBoundaries.X, hy) / hx;
		//	_y = -Master.Slau.p.Elements[elemNumber] * X1(point.X, xBoundaries.y, hx) / hy - Master.Slau.p.Elements[elemNumber + 1] * X2(point.X, xBoundaries.X, hx) / hy
		//	+ Master.Slau.p.Elements[elemNumber + Grid.N] * X1(point.X, xBoundaries.y, hx) / hy + Master.Slau.p.Elements[elemNumber + Grid.N + 1] * X2(point.X, xBoundaries.X, hx) / hy;
		//	return new Point(-_x, -_y);
		//}

		//public static double VGradP(FiniteElement element, Point point, int i, int j)
		//{
		//	double result = 0;
		//	var hy = element.hy;
		//	var hx = element.hx;
		//	var topBoundary = element.YBoundaries.Y;
		//	var bottomBoundary = element.YBoundaries.X;
		//	var leftBoudary = element.XBoundaries.X;
		//	var rightBoundary = element.XBoundaries.Y;
		//	Point V = GetV(element, point);

		//	switch (j)
		//	{
		//		case 0:
		//			result += (-V.X * Y1(point.Y, topBoundary, hy) / hx - V.Y * X1(point.X, rightBoundary, hx) / hy);
		//			break;
		//		case 1:
		//			result += (V.X * Y1(point.Y, topBoundary, hy) / hx - V.Y * X2(point.X, leftBoudary, hx) / hy);
		//			break;
		//		case 2:
		//			result += (-V.X * Y2(point.Y, bottomBoundary, hy) / hx + V.Y * X1(point.X, rightBoundary, hx) / hy);
		//			break;
		//		case 3:
		//			result += (V.X * Y2(point.Y, bottomBoundary, hy) / hx + V.Y * X2(point.X, leftBoudary, hx) / hy);
		//			break;
		//	}
		//	switch (i)
		//	{
		//		case 0:
		//			result *= Y1(point.Y, topBoundary, hy) * X1(point.X, rightBoundary, hx);
		//			break;
		//		case 1:
		//			result *= Y1(point.Y, topBoundary, hy) * X2(point.X, leftBoudary, hx);
		//			break;
		//		case 2:
		//			result *= Y2(point.Y, bottomBoundary, hy) * X1(point.X, rightBoundary, hx);
		//			break;
		//		case 3:
		//			result *= Y2(point.Y, bottomBoundary, hy) * X2(point.X, leftBoudary, hx);
		//			break;
		//	}
		//	return result;
		//}
	}
}