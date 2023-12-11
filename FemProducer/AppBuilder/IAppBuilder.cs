using FemProducer.Models;

namespace FemProducer.AppBuilder
{
	internal interface IAppBuilder
	{
		internal ProblemService GetProblemParameters();
		internal SolverParameters GetSolverParameters();
		internal GridParameters GetGridParameters();
	}
}