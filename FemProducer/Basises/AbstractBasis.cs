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

        public virtual void ConsiderFirstBoundaryCondition(Slae slae, Node node, int nodeIndex, int area)//это не должно быть в базисе
        {
            slae.Matrix.ZeroingRow(nodeIndex);
            slae.Matrix.Di[nodeIndex] = 1;
            slae.Vector[nodeIndex] = _problemService.Function(node, area);
        }

        public abstract IList<double> GetSecondBoundaryVector(IList<Node> nodes, Func<Node, int, double> func, int formulaNumber);

        public abstract (IList<IList<double>>, IList<double>) ConsiderThirdBoundaryCondition(Slae slae, IList<Node> nodes, IList<int> nodesIndexes, Func<Node, int, double> func, int formulaNumber);
    }
}
