using FemProducer.Models;

using Grid.Models;

using SlaeSolver.Models;

namespace FemProducer.AppBuilder
{
	public interface IAppBuilder
	{
		public ProblemParameters GetProblemParameters();
		public SolverParameters GetSolverParameters();
		public GridParameters GetGridParameters();
	}
}