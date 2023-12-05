using FemProducer.DTO;

namespace FemProducer
{
	internal interface ITaskBuilder
	{
		internal ProblemParametrs GetProblemParameters();
		internal SolverParameters GetSolverParameters();
		internal GridParameters GetGridParameters();
	}
}
