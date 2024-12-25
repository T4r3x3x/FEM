using FemProducer.Basises.Abstractions;
using FemProducer.Services;

using Grid.Models;

using MathModels.Models;

using NumericsMethods;

using static FemProducer.Basises.Helpers.BasisFunctions.LinearQuarangularBasisFunctions;

namespace FemProducer.Basises.Implementations.TwoDimensional
{
    public partial class LinearQuadrangularCartesianBasis : AbstractBasis
    {
        protected override int NodesCountInElement => 4;

        private readonly Node singleSquareFirstPoint = new Node(0, 0);
        private readonly Node singleSquareFourthPoint = new Node(1, 1);

        private double[][][] M = [
            [[4, 2, 2, 1], [2, 4, 1, 2], [2, 1, 4, 2], [1, 2, 2, 4]],

            [[2, 2, 1, 1], [2, 6, 1, 3], [1, 1, 2, 2], [1, 3, 2, 6]],

            [[2, 1, 2, 1], [1, 2, 1, 2], [2, 1, 6, 3], [1, 2, 3, 6]]
            ];

        public LinearQuadrangularCartesianBasis(ProblemService problemService) : base(problemService) { }

        private double Jacobian(double ksi, double nu, LinearQuadrangularCartesianBasisCoefficients coefficients) => coefficients.a0 + ksi * coefficients.a1 + nu * coefficients.a2;

        public override IList<IList<double>> GetMassMatrix(FiniteElement finiteElement)
        {
            var massMatrix = InitializeMatrix(NodesCountInElement);

            var coefficents = new LinearQuadrangularCartesianBasisCoefficients(finiteElement);
            for (int i = 0; i < NodesCountInElement; i++)
                for (int j = 0; j < NodesCountInElement; j++)
                    massMatrix[i][j] += (coefficents.a0 / 36 * M[0][i][j] + coefficents.a1 / 72 * M[1][i][j] + coefficents.a2 / 72 * M[2][i][j]) * Math.Sign(coefficents.a0);
            return massMatrix;
        }

        public override IList<IList<double>> GetStiffnessMatrix(FiniteElement finiteElement)
        {
            var stiffnessMatrix = InitializeMatrix(NodesCountInElement);

            var coefficients = new LinearQuadrangularCartesianBasisCoefficients(finiteElement);
            for (int i = 0; i < finiteElement.Nodes.Length; i++)
                for (int j = 0; j < finiteElement.Nodes.Length; j++)
                {
                    var integrFunc = GetStiffnessIntegrFunc(i, j, coefficients);
                    // var intFunc = new StiffnessIntegrationFuncClass(coefficients, i, j, Jacobian);
                    stiffnessMatrix[i][j] = Integration.GaussIntegration(singleSquareFirstPoint, singleSquareFourthPoint,
                        integrFunc, Integration.PointsCount.Three);
                }
            return stiffnessMatrix;
        }

        public override Dictionary<string, IList<IList<double>>> GetLocalMatrixes(FiniteElement finiteElement)
        {
            return new()
            {
                { "ColumnSize", GetMassMatrix(finiteElement)},
                { "_G", GetStiffnessMatrix(finiteElement)}
            };
        }

        private Func<double, double, double> GetStiffnessIntegrFunc(int i, int j, LinearQuadrangularCartesianBasisCoefficients coefficients)
        {
            return (double ksi, double nu) =>
            {
                var IfitasKsi = FitasKsi[i](ksi, nu);
                var JfitasKsi = FitasKsi[j](ksi, nu);
                var IfitasNu = FitasNu[i](ksi, nu);
                var JfitasNu = FitasNu[j](ksi, nu);
                var J = Jacobian(ksi, nu, coefficients);

                var a = IfitasKsi * (coefficients.b6 * ksi + coefficients.b3) - IfitasNu * (coefficients.b6 * nu + coefficients.b4);
                var b = JfitasKsi * (coefficients.b6 * ksi + coefficients.b3) - JfitasNu * (coefficients.b6 * nu + coefficients.b4);
                var c = IfitasNu * (coefficients.b5 * nu + coefficients.b2) - IfitasKsi * (coefficients.b5 * ksi + coefficients.b1);
                var d = JfitasNu * (coefficients.b5 * nu + coefficients.b2) - JfitasKsi * (coefficients.b5 * ksi + coefficients.b1);

                return Math.Sign(coefficients.a0) * ((a * b / J) + (c * d / J));
            };
        }

        public override IList<double> GetSecondBoundaryData(FiniteElement finiteElement, Func<Node, int, double> func, int formulaNumber) => throw new NotImplementedException();
        public override (IList<IList<double>>, IList<double>) GetThirdBoundaryData(Slae slae, FiniteElement finiteElement, Func<Node, int, double> func, int formulaNumber) => throw new NotImplementedException();
    }
}