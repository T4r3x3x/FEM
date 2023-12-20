using SlaeSolver.Models;

namespace SlaeSolver.Interfaces
{
	public interface ISolverFactory
	{
		ISolver CreateSolver(SolverParameters solverParameters);
	}
}
