using Grid.Models;

using MathModels.Models;

namespace FemProducer.Basises
{
	public abstract class AbstractBasis
	{
		private readonly ProblemService _problemService;

		protected AbstractBasis(ProblemService problemService) => _problemService = problemService;

		public abstract IList<IList<double>> GetMassMatrix(IList<Node> nodes);
		public abstract IList<IList<double>> GetStiffnessMatrix(IList<Node> nodes);
		public abstract Dictionary<string, IList<IList<double>>> GetLocalMatrixes(IList<Node> nodes);
		public abstract IList<double> GetLocalVector(IList<Node> nodes, Func<Node, int, double> func, int formulaNumber);

		public virtual void ConsiderFirstBoundaryCondition(Slae slae, Node node, int nodeIndex, int area)
		{
			slae.Matrix.ZeroingRow(nodeIndex);
			slae.Matrix.Di[nodeIndex] = 1;
			slae.Vector[nodeIndex] = _problemService.Function(node, area);
		}

		public abstract void ConsiderSecondBoundaryCondition(Slae slae, Node node, int nodeIndex);

		public abstract void ConsiderThirdBoundaryCondition(Slae slae, Node node, int nodeIndex);
	}
}
