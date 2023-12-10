using FemProducer.Models;

using Tensus;

namespace FemProducer.Solver
{
	internal interface ISolver
	{
		public Vector Solve(Slae slae);
	}
}
