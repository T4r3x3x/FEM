using FemProducer.Basises.Abstractions;
using FemProducer.Basises.Helpers.BasisFunctions;
using FemProducer.Services;

using Grid.Models;

using MathModels.Models;

using static FemProducer.Basises.Helpers.IndexFunctions.IndexFunctions2D;

namespace FemProducer.Basises.Implementations.TwoDimensional
{
    public class LinearRectangularBasis(ProblemService problemService) : Abstract2DBasis(problemService)
    {
        protected override int NodesCountInElement => 4;

        private readonly double[,] _g = LinearBasisFunctions.G;
        private readonly double[,] _m = LinearBasisFunctions.M;

        public override IList<IList<double>> GetMassMatrix(FiniteElement finiteElement)
        {
            var massMatrix = InitializeMatrix(NodesCountInElement);

            (var hx, var hy) = finiteElement.GetSteps2D();

            for (int i = 0; i < NodesCountInElement; i++)
                for (int j = 0; j < NodesCountInElement; j++)
                {
                    massMatrix[i][j] += _m[Mu(i), Mu(j)] * _m[Nu(i), Nu(j)];
                    massMatrix[i][j] *= hx / 6 * hy / 6;
                }
            return massMatrix;
        }

        public override IList<IList<double>> GetStiffnessMatrix(FiniteElement finiteElement)
        {
            var stiffnessMatrix = InitializeMatrix(NodesCountInElement);

            (var hx, var hy) = finiteElement.GetSteps2D();

            for (int i = 0; i < NodesCountInElement; i++)
                for (int j = 0; j < NodesCountInElement; j++)
                    stiffnessMatrix[i][j] = _g[Mu(i), Mu(j)] / hx * _m[Nu(i), Nu(j)] * hy / 6 + _m[Mu(i), Mu(j)] * hx / 6 * _g[Nu(i), Nu(j)] / hy;

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