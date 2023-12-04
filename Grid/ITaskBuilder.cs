using FemProducer.DTO;

using ReaserchPaper.Solver;

namespace FemProducer
{
	internal interface ITaskBuilder
	{
		internal ProblemParametrs GetProblem();
		internal ISolver GetSolver();
		internal GridParameters GetGridParameters();
	}
}
