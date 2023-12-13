using FemProducer.Basises.BasisFunctions;

using Grid.Models;

using MathModels;

using NumericsMethods;


namespace FemProducer.Basises
{
	public class LinearQuadrangularCartesianBasis : IBasis
	{
		public const int NodesCount = 4;

		private int i, j;
		private double b1, b2, b3, b4, b5, b6;
		private double a0, a1, a2;

		private readonly Point singleSquareFirstPoint = new Point(0, 0);
		private readonly Point singleSquareFourthPoint = new Point(1, 1);

		private double Jacobian(double ksi, double nu) => a0 + ksi * a1 + nu * a2;

		public double StiffnessIntegrationFunc(double ksi, double nu)
		{
			var IfitasKsi = LinearQuarangularBasisFunctions.fitasKsi[i](ksi, nu);
			var JfitasKsi = LinearQuarangularBasisFunctions.fitasKsi[j](ksi, nu);
			var IfitasNu = LinearQuarangularBasisFunctions.fitasNu[i](ksi, nu);
			var JfitasNu = LinearQuarangularBasisFunctions.fitasNu[j](ksi, nu);
			var J = Jacobian(ksi, nu);


			var a = IfitasKsi * (b6 * ksi + b3) - IfitasNu * (b6 * nu + b4);
			var b = JfitasKsi * (b6 * ksi + b3) - JfitasNu * (b6 * nu + b4);
			var c = IfitasNu * (b5 * nu + b2) - IfitasKsi * (b5 * ksi + b1);
			var d = JfitasNu * (b5 * nu + b2) - JfitasKsi * (b5 * ksi + b1);

			return Math.Sign(a0) * ((a * b / J) + (c * d / J));
		}



		private double[][][] M = [
			[[4, 2, 2, 1], [2, 4, 1, 2], [2, 1, 4, 2], [1, 2, 2, 4]],

			[[2, 2, 1, 1], [2, 6, 1, 3], [1, 1, 2, 2], [1, 3, 2, 6]],

			[[2, 1, 2, 1], [1, 2, 1, 2], [2, 1, 6, 3], [1, 2, 3, 6]]
			];

		public IList<IList<double>> GetMassMatrix(IList<Node> nodes)
		{
			// инициализация
			double[][] result = new double[NodesCount][];
			for (int i = 0; i < result.Length; i++)
				result[i] = new double[result.Length];

			CalcultaeVariables(nodes);

			var a = M[0][0][0];

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
			{
				result[i] = new double[nodes.Count];
				for (j = 0; j < nodes.Count; j++)
					result[i][j] = Integration.GaussIntegration(singleSquareFirstPoint, singleSquareFourthPoint, StiffnessIntegrationFunc, Integration.PointsCount.Three);
			}
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

		public IList<double> GetLocalVector(IList<Node> nodes, Func<Node, double> func)
		{
			double[] localVector = new double[NodesCount];
			CalcultaeVariables(nodes);
			for (int i = 0; i < NodesCount; i++)
			{
				LocalVectorIntegrationFuncClass intF = new(func, Jacobian, i);
				localVector[i] += Integration.GaussIntegration(singleSquareFirstPoint, singleSquareFourthPoint, intF.LocalVectorIntegrationFunc, Integration.PointsCount.Three);
			}

			return localVector;
		}
		/// <summary>
		/// костыль нужный для частичной передачи параметров, а именно для передачи функции правой части уравнения, так как GaussIntegration принимает только функции со сигнатурой <double, (double,double)>.
		/// </summary>
		class LocalVectorIntegrationFuncClass
		{
			private Func<Node, double> _func;
			private Func<double, double, double> _jacobian;
			private int _i = 0;

			public LocalVectorIntegrationFuncClass(Func<Node, double> func, Func<double, double, double> jacobian, int i)
			{
				_func = func;
				_jacobian = jacobian;
				_i = i;
			}

			public double LocalVectorIntegrationFunc(double ksi, double nu)
			{
				var res = _func(new Node(ksi, nu)) * _jacobian(ksi, nu) * LinearQuarangularBasisFunctions.Fita[_i](ksi, nu);
				return res;
			}
		}
	}
}
