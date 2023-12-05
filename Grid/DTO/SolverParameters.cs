using FemProducer.Solver;

namespace FemProducer.DTO
{
	internal record SolverParameters(SolverType SolverType, int MaxIterCount, double Epsilon);
}
