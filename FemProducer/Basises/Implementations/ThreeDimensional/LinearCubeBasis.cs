using FemProducer.Basises.Abstractions;
using FemProducer.Basises.Helpers.BasisFunctions;
using FemProducer.Basises.Helpers.IndexFunctions;
using FemProducer.Basises.Implementations.TwoDimensional;
using FemProducer.Services;

using Grid.Models;

using MathModels.Models;

namespace FemProducer.Basises.Implementations.ThreeDimensional
{
    using static IndexFunctions3D;

    public class LinearCubeBasis : AbstractBasis
    {
        private LinearRectangularBasis _boundaryBasis;

        protected override int _nodesCountInElement => 8;

        private double[,] G = LinearBasisFunctions.G;
        private double[,] M = LinearBasisFunctions.M;

        public LinearCubeBasis(ProblemService problemService) : base(problemService)
        {
            _boundaryBasis = new(problemService);
        }

        public override IList<IList<double>> GetMassMatrix(FiniteElement finiteElement)
        {
            var massMatrix = InitializeMatrix(_nodesCountInElement);

            (var hx, var hy, var hz) = finiteElement.GetSteps3D();

            for (int i = 0; i < _nodesCountInElement; i++)
                for (int j = 0; j < _nodesCountInElement; j++)
                    massMatrix[i][j] = M[Mu(i), Mu(j)] * M[Nu(i), Nu(j)] * M[Eps(i), Eps(j)] * hx / 6 * hy / 6 * hz / 6;
            return massMatrix;
        }

        public override IList<IList<double>> GetStiffnessMatrix(FiniteElement finiteElement)
        {
            var stiffnessMatrix = InitializeMatrix(_nodesCountInElement);

            (var hx, var hy, var hz) = finiteElement.GetSteps3D();

            for (int i = 0; i < _nodesCountInElement; i++)
                for (int j = 0; j < _nodesCountInElement; j++)
                    stiffnessMatrix[i][j] = G[Mu(i), Mu(j)] / hx * M[Nu(i), Nu(j)] * hy / 6 * M[Eps(i), Eps(j)] * hz / 6
                        + M[Mu(i), Mu(j)] * hx / 6 * G[Nu(i), Nu(j)] / hy * M[Eps(i), Eps(j)] * hz / 6
                        + M[Mu(i), Mu(j)] * hx / 6 * M[Nu(i), Nu(j)] * hy / 6 * G[Eps(i), Eps(j)] / hz;
            return stiffnessMatrix;
        }

        public override Dictionary<string, IList<IList<double>>> GetLocalMatrixes(FiniteElement finiteElement)
        {
            return new()
            {
                { "_G", GetStiffnessMatrix(finiteElement) },
                { "ColumnSize", GetMassMatrix(finiteElement) }
            };
        }

        public override IList<double> GetSecondBoundaryData(FiniteElement finiteElement, Func<Node, int, double> func, int formulaNumber) =>
            _boundaryBasis.GetLocalVector(finiteElement, func, formulaNumber);

        public override (IList<IList<double>>, IList<double>) GetThirdBoundaryData(Slae slae, FiniteElement finiteElement, Func<Node, int, double> func, int formulaNumber)
        {
            var matrix = _boundaryBasis.GetMassMatrix(finiteElement);
            var vector = _boundaryBasis.GetLocalVector(finiteElement, func, formulaNumber);
            return (matrix, vector);
        }
    }
}