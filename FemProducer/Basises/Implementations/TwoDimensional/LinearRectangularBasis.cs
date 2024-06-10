using FemProducer.Basises.Abstractions;
using FemProducer.Basises.Helpers.BasisFunctions;
using FemProducer.Services;

using Grid.Models;

using MathModels.Models;

using static FemProducer.Basises.Helpers.IndexFunctions.IndexFunctions2D;

namespace FemProducer.Basises.Implementations.TwoDimensional
{
    public class LinearRectangularBasis : Abstract2DBasis
    {
        protected override int _nodesCountInElement => 4;

        private double[,] _G = LinearBasisFunctions.G;
        private double[,] _M = LinearBasisFunctions.M;

        public LinearRectangularBasis(ProblemService problemService) : base(problemService) { }

        public override IList<IList<double>> GetMassMatrix(FiniteElement finiteElement)
        {
            var massMatrix = InitializeMatrix(_nodesCountInElement);

            (var hx, var hy) = finiteElement.GetSteps2D();

            for (int i = 0; i < _nodesCountInElement; i++)
                for (int j = 0; j < _nodesCountInElement; j++)
                {
                    massMatrix[i][j] += _M[Mu(i), Mu(j)] * _M[Nu(i), Nu(j)];
                    massMatrix[i][j] *= hx / 6 * hy / 6;
                }
            return massMatrix;
        }

        public override IList<IList<double>> GetStiffnessMatrix(FiniteElement finiteElement)
        {
            var stiffnessMatrix = InitializeMatrix(_nodesCountInElement);

            (var hx, var hy) = finiteElement.GetSteps2D();

            for (int i = 0; i < _nodesCountInElement; i++)
                for (int j = 0; j < _nodesCountInElement; j++)
                    stiffnessMatrix[i][j] = _G[Mu(i), Mu(j)] / hx * _M[Nu(i), Nu(j)] * hy / 6 + _M[Mu(i), Mu(j)] * hx / 6 * _G[Nu(i), Nu(j)] / hy;

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

        public override IList<double> GetSecondBoundaryData(FiniteElement finiteElement, Func<Node, int, double> func, int formulaNumber) => throw new NotImplementedException();
        public override (IList<IList<double>>, IList<double>) GetThirdBoundaryData(Slae slae, FiniteElement finiteElement, Func<Node, int, double> func, int formulaNumber) => throw new NotImplementedException();
    }
}