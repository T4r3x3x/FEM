using FemProducer.Solver;

namespace FemProducer.Models
{
	internal record SolverParameters(SolverType SolverType, int MaxIterCount, double Epsilon);
}
