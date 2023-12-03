using FemProducer;

namespace ResearchPaper
{
	class Startup
	{
		static void Main(string[] args)
		{
			System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
			sw.Start();
			IProblemSolver problemSolver = new TimeProblemSolver();
			problemSolver.Solve();
			sw.Stop();
			Console.WriteLine(sw.ElapsedMilliseconds);
		}
	}
}