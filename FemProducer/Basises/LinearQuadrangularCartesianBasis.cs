using FemProducer.Basises.BasisFunctions;

using Grid.Models;

using MathModels;

using NumericsMethods;

namespace FemProducer.Basises
{
	public class LinearQuadrangularCartesianBasis : IBasis
	{
		List<Func<double, double, double>> fitasKsi = [
			(ksi, nu) => nu - 1,
			(ksi, nu) => 1 - nu,
			(ksi, nu) => -nu,
			(ksi, nu) => nu,
		];

		List<Func<double, double, double>> fitasNu = [
			(ksi, nu) => ksi - 1,
			(ksi, nu) => -ksi,
			(ksi, nu) => 1 - ksi,
			(ksi, nu) => ksi,
		];

		private int i, j;
		private double b1, b2, b3, b4, b5, b6;
		private double a0, a1, a2;

		private readonly Point singleSquareFirstPoint = new Point(0, 0);
		private readonly Point singleSquareFourthPoint = new Point(1, 1);

		private double J(double ksi, double nu) => a0 + ksi * a1 + nu * a2;

		private double IntegrationFunc(double ksi, double nu)
		{
			var a = fitasKsi[i](ksi, nu) * (b6 * ksi + b3) - fitasNu[i](ksi, nu) * (b6 * nu + b4);
			var b = fitasKsi[j](ksi, nu) * (b6 * ksi + b3) - fitasNu[j](ksi, nu) * (b6 * nu + b4);
			var c = fitasNu[i](ksi, nu) * (b5 * nu + b2) - fitasKsi[i](ksi, nu) * (b5 * ksi + b1);
			var d = fitasNu[j](ksi, nu) * (b5 * nu + b2) - fitasKsi[j](ksi, nu) * (b5 * ksi + b1);
			return Math.Sign(a0) * ((a * b / J(ksi, nu)) + (c * d) / J(ksi, nu));
		}

		private double[][][] M = [
			[[4, 2, 2, 1], [2, 4, 1, 2], [2, 1, 4, 2], [1, 2, 2, 4]],

			[[2, 2, 1, 1], [2, 6, 1, 3], [1, 1, 2, 2], [1, 3, 2, 6]],

			[[2, 1, 2, 1], [1, 2, 1, 2], [2, 1, 6, 3], [1, 2, 3, 6]]
			];

		public IList<double> GetLocalVector(IList<Node> nodes, Func<Node, double> func) => LinearBasisFunctions.GetLocalVector(nodes, func);
		public IList<IList<double>> GetMassMatrix(IList<Node> nodes)
		{
			var hx = nodes[1].X - nodes[0].X;
			var hy = nodes[2].Y - nodes[0].Y;

			// инициализация
			double[][] result = new double[M.LongLength][];
			for (int i = 0; i < result.Length; i++)
				result[i] = new double[result.Length];

			//матрица масс
			for (int i = 0; i < result.Length; i++)
				for (int j = 0; j < result.Length; j++)
					result[i][j] += (a0 / 36 * M[0][i][j] + a1 / 72 * M[1][i][j] + a2 / 72 * M[2][i][j]) * Math.Sign(a0);

			return result;
		}

		public IList<IList<double>> GetStiffnessMatrix(IList<Node> nodes)
		{
			double[][] result = new double[nodes.Count][];
			CalcultaeVariables(nodes);

			for (i = 0; i < nodes.Count; i++)
				for (j = 0; j < nodes.Count; j++)
					Integration.GaussIntegration(singleSquareFirstPoint, singleSquareFourthPoint, IntegrationFunc, Integration.PointsCount.Three);

			return result;
		}

		private void CalcultaeVariables(IList<Node> nodes)
		{
			var p1 = nodes[0];
			var p2 = nodes[1];
			var p3 = nodes[2];
			var p4 = nodes[3];

			b1 = p3.X - p1.X;
			b2 = p2.X - p1.X;
			b3 = p3.Y - p1.Y;
			b4 = p2.Y - p1.Y;
			b5 = p1.X - p2.X - p3.X + p4.X;
			b6 = p1.Y - p2.Y - p3.Y + p4.Y;

			a0 = (p2.X - p1.X) * (p3.Y - p1.Y) - (p2.Y - p1.Y) * (p3.X - p1.X);
			a1 = (p2.X - p1.X) * (p4.Y - p3.Y) - (p2.Y - p1.Y) * (p4.X - p3.X);
			a2 = (p3.Y - p1.Y) * (p4.X - p2.X) - (p3.X - p1.X) * (p4.Y - p2.Y);
		}

		public Dictionary<string, IList<IList<double>>> GetLocalMatrixes(IList<Node> nodes)
		{
			return new()
			{
				{ "M", GetMassMatrix(nodes)},
				{ "G", GetStiffnessMatrix(nodes)}
			};
		}
	}
}
