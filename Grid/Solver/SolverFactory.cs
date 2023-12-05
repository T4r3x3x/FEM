using FemProducer.DTO;

using ResearchPaper;

namespace ReaserchPaper.Solver
{
	internal class SolverFactory
	{
		internal ISolver CreateSolver(SolverParameters solverParameters) => new LosLU(solverParameters.MaxIterCount, solverParameters.Epsilon);
	}
}
