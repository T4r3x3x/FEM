using Grid.Models;

using MathModels;

namespace FemProducer
{
	public class FEM
	{
		static double Y1(double y, double yUpper, double hy) => (yUpper - y) / hy;
		static double Y2(double y, double yLower, double hy) => (y - yLower) / hy;
		static double X1(double X, double xRight, double hx) => (xRight - X) / hx;
		static double X2(double X, double xLeft, double hx) => (X - xLeft) / hx;

		public static double Psi(int i, Node node, Point xLimits, Point yLimits)
		{
			double hx = xLimits.Y - xLimits.X;
			double hy = yLimits.Y - yLimits.X;

			switch (i)
			{
				case 0:
					return X1(node.X, xLimits.Y, hx) * Y1(node.Y, yLimits.Y, hy);
				case 1:
					return X2(node.X, xLimits.X, hx) * Y1(node.Y, yLimits.Y, hy);
				case 2:
					return X1(node.X, xLimits.Y, hx) * Y2(node.Y, yLimits.X, hy);
				case 3:
					return X2(node.X, xLimits.X, hx) * Y2(node.Y, yLimits.X, hy);
				default:
					throw new Exception("Ne verniy i!");
			}
		}

		static double[][] G =
		[
			[1, -1],
			[-1, 1],
		];
		static double[][] M =
		[
			[2, 1],
			[1, 2]
		];

		static int mu(int i) => ((i) % 2);
		static int nu(int i) => ((i) / 2);

		public static double[][] GetMassMatrix(double hx, double hy)// Grid.M - номер кэ 
		{
			// инициализация
			double[][] result = new double[M.LongLength][];
			for (int i = 0; i < result.Length; i++)
				result[i] = new double[result.Length];

			//матрица масс
			for (int i = 0; i < result.Length; i++)
				for (int j = 0; j < result.Length; j++)
					result[i][j] += M[mu(i)][mu(j)] * hx / 6 * M[nu(i)][nu(j)] * hy / 6;

			return result;
		}
		public static double[][] GetStiffnessMatrix(double hx, double hy)// Grid.M - номер кэ 
		{
			// инициализация
			double[][] result = new double[G.LongLength][];
			for (int i = 0; i < result.Length; i++)
				result[i] = new double[result.Length];

			//матрица жесткости
			for (int i = 0; i < result.Length; i++)
				for (int j = 0; j < result.Length; j++)
					result[i][j] = G[mu(i)][mu(j)] / hx * M[nu(i)][nu(j)] * hy / 6 + M[mu(i)][mu(j)] * hx / 6 * G[nu(i)][nu(j)] / hy;

			return result;
		}

		//public static double SolutionInPoint(FiniteElement element, Point point)
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
