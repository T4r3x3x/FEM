using FemProducer.Services;

using Grid.Models;

using MathModels.Models;

namespace FemProducer.Basises
{
    public abstract class AbstractBasis
    {
        protected readonly ProblemService _problemService;

        protected AbstractBasis(ProblemService problemService) => _problemService = problemService;

        public abstract IList<IList<double>> GetMassMatrix(IList<Node> nodes);
        public abstract IList<IList<double>> GetStiffnessMatrix(IList<Node> nodes);
        public abstract Dictionary<string, IList<IList<double>>> GetLocalMatrixes(IList<Node> nodes);
        public abstract IList<double> GetLocalVector(IList<Node> nodes, Func<Node, int, double> func, int formulaNumber);

        public abstract IList<double> GetSecondBoundaryVector(IList<Node> nodes, Func<Node, int, double> func, int formulaNumber);

        public abstract (IList<IList<double>>, IList<double>) ConsiderThirdBoundaryCondition(Slae slae, IList<Node> nodes, IList<int> nodesIndexes, Func<Node, int, double> func, int formulaNumber);

        protected double GetXStep(IList<Node> nodes) => nodes[1].X - nodes[0].X;
        protected double GetYStep(IList<Node> nodes) => nodes[2].Y - nodes[0].Y;
        protected double GetZStep(IList<Node> nodes) => nodes[4].Z - nodes[0].Z;
    }
}
