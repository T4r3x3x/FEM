using FemProducer.Basises.BasisFunctions;

using Grid.Models;

using MathModels.Models;

namespace FemProducer.Basises
{
	internal class LinearRectangularBasis : AbstractBasis
	{
		public const int NodesCount = 4;


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

		public LinearRectangularBasis(ProblemService problemService) : base(problemService) { }

		private static int mu(int i) => i % 2;
		private static int nu(int i) => i / 2;

		public override IList<IList<double>> GetMassMatrix(IList<Node> nodes)// Grid.M - номер кэ 
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
					result[i][j] += M[mu(i), mu(j)] * hx / 6 * M[nu(i), nu(j)] * hy / 6;

			return result;
		}

		public override IList<IList<double>> GetStiffnessMatrix(IList<Node> nodes)// Grid.M - номер кэ 
		{
			var hx = nodes[1].X - nodes[0].X;
			var hy = nodes[2].Y - nodes[0].Y;

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

		public override IList<double> GetLocalVector(IList<Node> nodes, Func<Node, int, double> func, int formulaNumber) => LinearBasisFunctions.GetLocalVector(nodes, func, formulaNumber);

		public override Dictionary<string, IList<IList<double>>> GetLocalMatrixes(IList<Node> nodes)
		{
			return new()
			{
				{ "G", GetStiffnessMatrix(nodes) },
				{ "M", GetMassMatrix(nodes) }
			};
		}

		public override void ConsiderSecondBoundaryCondition(Slae slae, Node node, int nodeIndex) => throw new NotImplementedException();
		public override void ConsiderThirdBoundaryCondition(Slae slae, Node node, int nodeIndex) => throw new NotImplementedException();
	}
}