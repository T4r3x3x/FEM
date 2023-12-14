using FemProducer.Basises.BasisFunctions;

using Grid.Models;

using MathModels;

using NumericsMethods;


namespace FemProducer.Basises
{
	public class LinearQuadrangularCartesianBasis : IBasis
	{
		public const int NodesCount = 4;


		private readonly Point singleSquareFirstPoint = new Point(0, 0);
		private readonly Point singleSquareFourthPoint = new Point(1, 1);





		private double[][][] M = [
			[[4, 2, 2, 1], [2, 4, 1, 2], [2, 1, 4, 2], [1, 2, 2, 4]],

			[[2, 2, 1, 1], [2, 6, 1, 3], [1, 1, 2, 2], [1, 3, 2, 6]],

			[[2, 1, 2, 1], [1, 2, 1, 2], [2, 1, 6, 3], [1, 2, 3, 6]]
			];

		private double Jacobian(double ksi, double nu, Coefficients coefficients) => coefficients.a0 + ksi * coefficients.a1 + nu * coefficients.a2;

		public IList<IList<double>> GetMassMatrix(IList<Node> nodes)
		{
			// инициализация
			double[][] result = new double[NodesCount][];
			for (int i = 0; i < result.Length; i++)
				result[i] = new double[result.Length];

			var coefficents = GetCoefficients(nodes);

			var a = M[0][0][0];

			//матрица масс
			for (int i = 0; i < result.Length; i++)
				for (int j = 0; j < result.Length; j++)
					result[i][j] += (coefficents.a0 / 36 * M[0][i][j] + coefficents.a1 / 72 * M[1][i][j] + coefficents.a2 / 72 * M[2][i][j]) * Math.Sign(coefficents.a0);

			return result;
		}

		public IList<IList<double>> GetStiffnessMatrix(IList<Node> nodes)
		{
			double[][] result = new double[nodes.Count][];
			var coefficents = GetCoefficients(nodes);
			for (int i = 0; i < nodes.Count; i++)
			{
				result[i] = new double[nodes.Count];
				for (int j = 0; j < nodes.Count; j++)
				{
					var intFunc = new StiffnessIntegrationFuncClass(coefficents, i, j, Jacobian);
					result[i][j] = Integration.GaussIntegration(singleSquareFirstPoint, singleSquareFourthPoint, intFunc.StiffnessIntegrationFunc, Integration.PointsCount.Three);
				}
			}
			return result;
		}

		private Coefficients GetCoefficients(IList<Node> nodes)
		{
			var p1 = nodes[0];
			var p2 = nodes[1];
			var p3 = nodes[2];
			var p4 = nodes[3];

			var b1 = p3.X - p1.X;
			var b2 = p2.X - p1.X;
			var b3 = p3.Y - p1.Y;
			var b4 = p2.Y - p1.Y;
			var b5 = p1.X - p2.X - p3.X + p4.X;
			var b6 = p1.Y - p2.Y - p3.Y + p4.Y;

			var a0 = (p2.X - p1.X) * (p3.Y - p1.Y) - (p2.Y - p1.Y) * (p3.X - p1.X);
			var a1 = (p2.X - p1.X) * (p4.Y - p3.Y) - (p2.Y - p1.Y) * (p4.X - p3.X);
			var a2 = (p3.Y - p1.Y) * (p4.X - p2.X) - (p3.X - p1.X) * (p4.Y - p2.Y);

			return new Coefficients(b1, b2, b3, b4, b5, b6, a0, a1, a2);
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
			var coefficents = GetCoefficients(nodes);
			for (int i = 0; i < NodesCount; i++)
			{
				LocalVectorIntegrationFuncClass intF = new LocalVectorIntegrationFuncClass(func, Jacobian, i, nodes, coefficents);
				localVector[i] += Integration.GaussIntegration(singleSquareFirstPoint, singleSquareFourthPoint, intF.LocalVectorIntegrationFunc, Integration.PointsCount.Three);
			}

			return localVector;
		}


		class Coefficients
		{
			public double b1, b2, b3, b4, b5, b6, a0, a1, a2;

			public Coefficients(double b1, double b2, double b3, double b4, double b5, double b6, double a0, double a1, double a2)
			{
				this.b1 = b1;
				this.b2 = b2;
				this.b3 = b3;
				this.b4 = b4;
				this.b5 = b5;
				this.b6 = b6;
				this.a0 = a0;
				this.a1 = a1;
				this.a2 = a2;
			}
		}


		/// <summary>
		/// костыль нужный для частичной передачи параметров, а именно для передачи функции правой части уравнения, так как GaussIntegration принимает только функции со сигнатурой <double, (double,double)>.
		/// </summary>
		class StiffnessIntegrationFuncClass
		{
			private Coefficients _coefficients;
			private int _i, _j;
			private Func<double, double, Coefficients, double> _jacobian;

			public StiffnessIntegrationFuncClass(Coefficients coefficients, int i, int j, Func<double, double, Coefficients, double> jacobian)
			{
				_coefficients = coefficients;
				_i = i;
				_j = j;
				_jacobian = jacobian;
			}

			public double StiffnessIntegrationFunc(double ksi, double nu)
			{
				var IfitasKsi = LinearQuarangularBasisFunctions.fitasKsi[_i](ksi, nu);
				var JfitasKsi = LinearQuarangularBasisFunctions.fitasKsi[_j](ksi, nu);
				var IfitasNu = LinearQuarangularBasisFunctions.fitasNu[_i](ksi, nu);
				var JfitasNu = LinearQuarangularBasisFunctions.fitasNu[_j](ksi, nu);
				var J = _jacobian(ksi, nu, _coefficients);


				var a = IfitasKsi * (_coefficients.b6 * ksi + _coefficients.b3) - IfitasNu * (_coefficients.b6 * nu + _coefficients.b4);
				var b = JfitasKsi * (_coefficients.b6 * ksi + _coefficients.b3) - JfitasNu * (_coefficients.b6 * nu + _coefficients.b4);
				var c = IfitasNu * (_coefficients.b5 * nu + _coefficients.b2) - IfitasKsi * (_coefficients.b5 * ksi + _coefficients.b1);
				var d = JfitasNu * (_coefficients.b5 * nu + _coefficients.b2) - JfitasKsi * (_coefficients.b5 * ksi + _coefficients.b1);

				return Math.Sign(_coefficients.a0) * ((a * b / J) + (c * d / J));
			}
		}

		/// <summary>
		/// костыль нужный для частичной передачи параметров, а именно для передачи функции правой части уравнения, так как GaussIntegration принимает только функции со сигнатурой <double, (double,double)>.
		/// </summary>
		class LocalVectorIntegrationFuncClass
		{
			private IList<Node> _nodes;
			private Func<Node, double> _func;
			private Func<double, double, Coefficients, double> _jacobian;
			private int _i = 0;
			private Coefficients _coefficients;

			public LocalVectorIntegrationFuncClass(Func<Node, double> func, Func<double, double, Coefficients, double> jacobian, int i, IList<Node> nodes, Coefficients coefficients)
			{
				_func = func;
				_jacobian = jacobian;
				_i = i;
				_nodes = nodes;
				_coefficients = coefficients;
			}

			private (double, double) ReverseCoordinateSubstitution(double ksi, double nu)
			{
				var x = (1 - ksi) * (1 - nu) * _nodes[0].X + ksi * (1 - nu) * _nodes[1].X + (1 - ksi) * nu * _nodes[2].X + ksi * nu * _nodes[3].X;
				var y = (1 - ksi) * (1 - nu) * _nodes[0].Y + ksi * (1 - nu) * _nodes[1].Y + (1 - ksi) * nu * _nodes[2].Y + ksi * nu * _nodes[3].Y;

				return (x, y);
			}

			public double LocalVectorIntegrationFunc(double ksi, double nu)
			{
				var xy = ReverseCoordinateSubstitution(ksi, nu);
				var res = _func(new Node(xy.Item1, xy.Item2)) * _jacobian(ksi, nu, _coefficients) * LinearQuarangularBasisFunctions.Fita[_i](ksi, nu);
				return res;
			}
		}
	}
}
