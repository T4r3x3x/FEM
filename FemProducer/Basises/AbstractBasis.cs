using FemProducer.Services;

using Grid.Models;

using MathModels.Models;

namespace FemProducer.Basises
{
    public abstract class AbstractBasis
    {
        protected readonly ProblemService _problemService;

        protected AbstractBasis(ProblemService problemService) => _problemService = problemService;

        public abstract IList<IList<double>> GetMassMatrix(FiniteElement finiteElement);
        public abstract IList<IList<double>> GetStiffnessMatrix(FiniteElement finiteElement);
        public abstract Dictionary<string, IList<IList<double>>> GetLocalMatrixes(FiniteElement finiteElement);
        public abstract IList<double> GetLocalVector(FiniteElement finiteElement, Func<Node, int, double> func, int formulaNumber);
        public abstract IList<double> GetSecondBoundaryVector(FiniteElement finiteElement, Func<Node, int, double> func, int formulaNumber);
        public abstract (IList<IList<double>>, IList<double>) ConsiderThirdBoundaryCondition(Slae slae, FiniteElement finiteElement, Func<Node, int, double> func, int formulaNumber);
    }
}
