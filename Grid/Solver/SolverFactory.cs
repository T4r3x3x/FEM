using FemProducer.Models;

namespace FemProducer.Solver
{
	internal class SolverFactory
	{
		internal ISolver CreateSolver(SolverParameters solverParameters) => new LosLU(solverParameters.MaxIterCount, solverParameters.Epsilon);
	}
}
