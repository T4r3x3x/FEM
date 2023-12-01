using FemProducer.DTO;

using ReaserchPaper.Solver;

namespace FemProducer
{
	internal interface ITaskBuilder
	{
		internal Problem GetProblem();
		internal ISolver GetSolver();
		internal GridParametrs GetGridParametrs();
	}
}
