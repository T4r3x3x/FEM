using FemProducer.Models;

using Grid.Models;

using SlaeSolver.Models;

namespace FemProducer.ConfigureReader
{
	public interface IConfigureReader
	{
		public ProblemParameters GetProblemParameters();
		public SolverParameters GetSolverParameters();
		public GridParameters GetGridParameters();
	}
}