using Grid.Models;

using SlaeSolver.Models;

namespace FemProducer.AppBuilder
{
	public interface IAppBuilder
	{
		public ProblemService GetProblemParameters();
		public SolverParameters GetSolverParameters();
		public GridParameters GetGridParameters();
	}
}