using FemProducer.Basises.BasisFunctions;

using Grid.Models;

using MathModels.Models;

namespace FemProducer.Basises
{
	public class LinearCubeBasis : AbstractBasis
	{
		public const int NodesCount = 8;

		private double[,] G = LinearBasisFunctions.G;
		private double[,] M = LinearBasisFunctions.M;
		private readonly LinearRectangularBasis _linearRectangularBasis = new(null);

		public LinearCubeBasis(ProblemService problemService) : base(problemService) { }

		private static int mu(int i) => (i) % 2;
		private static int nu(int i) => ((i) / 2) % 2;
		private static int vi(int i) => ((i) / 4);

		public override IList<IList<double>> GetMassMatrix(IList<Node> nodes)// Grid.M - номер кэ 
		{
			var hx = nodes[1].X - nodes[0].X;
			var hy = nodes[2].Y - nodes[0].Y;
			var hz = nodes[4].Z - nodes[0].Z;

			// инициализация
			double[][] result = new double[nodes.Count][];
			for (int i = 0; i < result.Length; i++)
				result[i] = new double[result.Length];

			//матрица масс
			for (int i = 0; i < result.Length; i++)
				for (int j = 0; j < result.Length; j++)
				{
					result[i][j] = M[mu(i), mu(j)] * M[nu(i), nu(j)] * M[vi(i), vi(j)] * hx / 6 * hy / 6 * hz / 6;
				}

			return result;
		}

		public override IList<IList<double>> GetStiffnessMatrix(IList<Node> nodes)// Grid.M - номер кэ 
		{
			var hx = nodes[1].X - nodes[0].X;
			var hy = nodes[2].Y - nodes[0].Y;
			var hz = nodes[4].Z - nodes[0].Z;

			// инициализация
			double[][] result = new double[nodes.Count][];
			for (int i = 0; i < result.Length; i++)
				result[i] = new double[result.Length];

			//матрица жесткости
			for (int i = 0; i < result.Length; i++)
				for (int j = 0; j < result.Length; j++)
					result[i][j] = G[mu(i), mu(j)] / hx * M[nu(i), nu(j)] * hy / 6 * M[vi(i), vi(j)] * hz / 6
						+ M[mu(i), mu(j)] * hx / 6 * G[nu(i), nu(j)] / hy * M[vi(i), vi(j)] * hz / 6
						+ M[mu(i), mu(j)] * hx / 6 * M[nu(i), nu(j)] * hy / 6 * G[vi(i), vi(j)] / hz;

			return result;
		}

		public override IList<double> GetLocalVector(IList<Node> nodes, Func<Node, int, double> func, int formulaNumber)
		{
			double[] localVector = new double[NodesCount];

			var massMatrix = GetMassMatrix(nodes);

			var funcValues = new double[NodesCount];
			for (int i = 0; i < NodesCount; i++)
				funcValues[i] = func(nodes[i], formulaNumber);

			for (int i = 0; i < NodesCount; i++)
				for (int j = 0; j < NodesCount; j++)
					localVector[i] += funcValues[j] * massMatrix[i][j];

			return localVector;
		}

		public override Dictionary<string, IList<IList<double>>> GetLocalMatrixes(IList<Node> nodes)
		{
			return new()
			{
				{ "G", GetStiffnessMatrix(nodes) },
				{ "M", GetMassMatrix(nodes) }
			};
		}

		public override IList<double> ConsiderSecondBoundaryCondition(Slae slae, IList<Node> nodes, int nodeIndex)
		{
			return _linearRectangularBasis.GetLocalVector(nodes, _problemService.DivFuncZ, nodeIndex);
		}

		public override (IList<IList<double>>, IList<double>) ConsiderThirdBoundaryCondition(Slae slae, IList<Node> nodes, int nodeIndex) => throw new NotImplementedException();

	}
}