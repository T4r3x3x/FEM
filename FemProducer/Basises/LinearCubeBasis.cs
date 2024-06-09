using FemProducer.Basises.BasisFunctions;
using FemProducer.Services;

using Grid.Models;

using MathModels.Models;

namespace FemProducer.Basises
{
    using static IntegerFuncitons3D;

    public class LinearCubeBasis : AbstractBasis
    {
        public const int NodesCount = 8;

        private double[,] G = LinearBasisFunctions.G;
        private double[,] M = LinearBasisFunctions.M;

        public LinearCubeBasis(ProblemService problemService) : base(problemService) { }

        public override IList<IList<double>> GetMassMatrix(FiniteElement finiteElement)// Grid.M - номер кэ 
        {
            (var hx, var hy, var hz) = finiteElement.GetSteps3D();

            // инициализация
            double[][] result = new double[finiteElement.Nodes.Length][];
            for (int i = 0; i < result.Length; i++)
                result[i] = new double[result.Length];

            //матрица масс
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result.Length; j++)
                {
                    result[i][j] = M[Mu(i), Mu(j)] * M[Nu(i), Nu(j)] * M[Eps(i), Eps(j)] * hx / 6 * hy / 6 * hz / 6;
                }

            return result;
        }

        public override IList<IList<double>> GetStiffnessMatrix(FiniteElement finiteElement)// Grid.M - номер кэ 
        {
            (var hx, var hy, var hz) = finiteElement.GetSteps3D();

            // инициализация
            double[][] result = new double[finiteElement.Nodes.Length][];
            for (int i = 0; i < result.Length; i++)
                result[i] = new double[result.Length];

            //матрица жесткости
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result.Length; j++)
                    result[i][j] = G[Mu(i), Mu(j)] / hx * M[Nu(i), Nu(j)] * hy / 6 * M[Eps(i), Eps(j)] * hz / 6
                        + M[Mu(i), Mu(j)] * hx / 6 * G[Nu(i), Nu(j)] / hy * M[Eps(i), Eps(j)] * hz / 6
                        + M[Mu(i), Mu(j)] * hx / 6 * M[Nu(i), Nu(j)] * hy / 6 * G[Eps(i), Eps(j)] / hz;

            return result;
        }

        public override IList<double> GetLocalVector(FiniteElement finiteElement, Func<Node, int, double> func, int formulaNumber)
        {
            double[] localVector = new double[NodesCount];

            var massMatrix = GetMassMatrix(finiteElement);

            var funcValues = new double[NodesCount];
            for (int i = 0; i < NodesCount; i++)
                funcValues[i] = func(finiteElement.Nodes[i], formulaNumber);

            for (int i = 0; i < NodesCount; i++)
                for (int j = 0; j < NodesCount; j++)
                    localVector[i] += funcValues[j] * massMatrix[i][j];

            return localVector;
        }

        public override Dictionary<string, IList<IList<double>>> GetLocalMatrixes(FiniteElement finiteElement)
        {
            return new()
            {
                { "G", GetStiffnessMatrix(finiteElement) },
                { "ColumnSize", GetMassMatrix(finiteElement) }
            };
        }

        public override IList<double> GetSecondBoundaryVector(FiniteElement finiteElement, Func<Node, int, double> func, int formulaNumber)
        {
            var rectBasis = new LinearRectangularBasis(_problemService);
            return rectBasis.GetLocalVector(finiteElement, func, formulaNumber);
        }

        public override (IList<IList<double>>, IList<double>) ConsiderThirdBoundaryCondition(Slae slae, FiniteElement finiteElement, Func<Node, int, double> func, int formulaNumber) => throw new NotImplementedException();
    }
}