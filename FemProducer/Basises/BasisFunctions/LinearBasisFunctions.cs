using System.Drawing;

using Grid.Models;

namespace FemProducer.Basises.BasisFunctions
{
	public static class LinearBasisFunctions
	{
		private const int NodesCount = 4;

		private static List<Func<double, double, double, double>> YFunctions = [
			(double y, double yUpper, double hy) => (yUpper - y) / hy,
			(double y, double yLower, double hy) => (y - yLower) / hy,
		];

		private static List<Func<double, double, double, double>> XFunctions = [

			(double X, double xRight, double hx) => (xRight - X) / hx,
			(double X, double xLeft, double hx) => (X - xLeft) / hx,
		];

		public static double GetBasisFunctionValue(int functionIndex, Node node, Point xLimits, Point yLimits)
		{
			double hx = xLimits.Y - xLimits.X;
			double hy = yLimits.Y - yLimits.X;

			return functionIndex switch
			{
				0 => XFunctions[0](node.X, xLimits.Y, hx) * YFunctions[0](node.Y, yLimits.Y, hy),
				1 => XFunctions[1](node.X, xLimits.X, hx) * YFunctions[0](node.Y, yLimits.Y, hy),
				2 => XFunctions[0](node.X, xLimits.Y, hx) * YFunctions[1](node.Y, yLimits.X, hy),
				3 => XFunctions[1](node.X, xLimits.X, hx) * YFunctions[1](node.Y, yLimits.X, hy),
				_ => throw new ArgumentException($"The argument i = {functionIndex} is an invalid value."),
			};
		}

		public static IList<double> GetLocalVector(IList<Node> nodes, Func<Node, double> func)
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
	}
}