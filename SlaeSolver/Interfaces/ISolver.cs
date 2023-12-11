using MathModels.Models;

namespace SlaeSolver.Interfaces
{
	public interface ISolver
	{
		public Vector Solve(Slae slae);
	}
}
