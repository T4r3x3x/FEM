using FemProducer.Models;
using Grid.Models.InputModels;
using SlaeSolver.Models;

namespace FemProducer.ConfigureReader
{
    public interface IConfigureReader
	{
		public ProblemParameters GetProblemParameters();
		public SolverParameters GetSolverParameters();
		public GridInputParameters GetGridParameters();
	}
}