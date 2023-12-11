using SlaeSolver.Implementations.Solvers;
using SlaeSolver.Interfaces;
using SlaeSolver.Models;

namespace SlaeSolver.Implementations.Factories
{
	public class SolverFactory : ISolverFactory
	{
		public ISolver CreateSolver(SolverParameters solverParameters) => new LosLU(solverParameters.MaxIterCount, solverParameters.Epsilon);
	}
}
