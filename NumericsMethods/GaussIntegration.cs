using System.ComponentModel;

using Grid.Models;

using MathModels;

namespace NumericsMethods
{
	public static class Integration
	{
		public static double GaussIntegration(Node leftLowerPoint, Node rightUpperPoint, Func<double, double, double> func, PointsCount pointsCount)
		{
			double result = 0;
			Point point;
			var hx = rightUpperPoint.X - leftLowerPoint.X;
			var hy = rightUpperPoint.Y - leftLowerPoint.Y;
			double[] q;
			double[] x;

			switch (pointsCount)
			{
				case PointsCount.Two:
					q = [1, 1];
					x = [-0.5773502692, 0.5773502692];
					break;

				case PointsCount.Three:
					q = [8 / 9.0, 5 / 9.0, 5 / 9.0];
					x = [0, 0.77459666924148337, -0.77459666924148337];
					break;

				case PointsCount.Four:
					q = [(18 - Math.Sqrt(30)) / 36, (18 + Math.Sqrt(30)) / 36, (18 + Math.Sqrt(30)) / 36, (18 - Math.Sqrt(30)) / 36];
					x = [-Math.Sqrt((15 + 2 * Math.Sqrt(30)) / 35), -Math.Sqrt((15 - 2 * Math.Sqrt(30)) / 35), Math.Sqrt((15 - 2 * Math.Sqrt(30)) / 35), Math.Sqrt((15 + 2 * Math.Sqrt(30)) / 35)];
					break;

				default:
					throw new InvalidEnumArgumentException();
			}

			for (int l = 0; l < q.Length; l++)
			{
				double temp = 0;
				for (int r = 0; r < q.Length; r++)
				{
					double u = (leftLowerPoint.X + rightUpperPoint.X + hx * x[r]) / 2.0;
					double v = (leftLowerPoint.Y + rightUpperPoint.Y + hy * x[l]) / 2.0;
					point = new Point(u, v);
					temp += q[r] * func(u, v);
				}
				result += q[l] * temp;
			}

			result *= hx * hy / 4.0;

			return result;
		}

		public static double GaussIntegration(Point x1y1z1, Point x2y2z1, Point x_y_z2, Func<double, double, double, double> func, PointsCount pointsCount)
		{
			throw new NotImplementedException();
		}

		public enum PointsCount
		{
			Two = 2,
			Three = 3,
			Four = 4,
		}
	}
}
