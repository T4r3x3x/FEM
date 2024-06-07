using FemProducer.Basises.BasisFunctions;
using FemProducer.Services;

using Grid.Models;

using MathModels.Models;

namespace FemProducer.Basises
{
    public class LinearRectangularBasis : Abstract2DBasis
    {
        public const int NodesCount = 4;

        private double[,] G = LinearBasisFunctions.G;
        private double[,] M = LinearBasisFunctions.M;

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
                {
                    result[i][j] += M[mu(i), mu(j)] * M[nu(i), nu(j)];
                    result[i][j] *= hx / 6 * hy / 6;
                }

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

        public override IList<double> GetLocalVector(IList<Node> nodes, Func<Node, int, double> func, int formulaNumber)
        {
            double[] result = new double[NodesCount];

            (var hx, var hy) = GetSteps2D(nodes);

            var funcValues = new double[NodesCount];
            for (int i = 0; i < NodesCount; i++)
                funcValues[i] = func(nodes[i], formulaNumber);

            result[0] = hx * hy / 36 * (4 * funcValues[0] + 2 * funcValues[1] + 2 * funcValues[2] + funcValues[3]);
            result[1] = hx * hy / 36 * (2 * funcValues[0] + 4 * funcValues[1] + funcValues[2] + 2 * funcValues[3]);
            result[2] = hx * hy / 36 * (2 * funcValues[0] + funcValues[1] + 4 * funcValues[2] + 2 * funcValues[3]);
            result[3] = hx * hy / 36 * (funcValues[0] + 2 * funcValues[1] + 2 * funcValues[2] + 4 * funcValues[3]);

            return result;
        }

        public override Dictionary<string, IList<IList<double>>> GetLocalMatrixes(IList<Node> nodes)
        {
            return new()
            {
                { "G", GetStiffnessMatrix(nodes) },
                { "ColumnSize", GetMassMatrix(nodes) }
            };
        }

        public override IList<double> GetSecondBoundaryVector(IList<Node> nodes, Func<Node, int, double> func, int formulaNumber) => throw new NotImplementedException();
        public override (IList<IList<double>>, IList<double>) ConsiderThirdBoundaryCondition(Slae slae, IList<Node> nodes, IList<int> nodeIndexes, Func<Node, int, double> func, int formulaNumber) => throw new NotImplementedException();
    }
}