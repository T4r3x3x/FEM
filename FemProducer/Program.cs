namespace FemProducer
{
	class Program
	{
		const string configureFile = "Confige.json";
		const string outputFile = "output.txt";

		static void Main(string[] args)
		{


			System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
			sw.Start();
			IProblemSolver problemSolver = new TimeProblemSolver();
			problemSolver.Solve(configureFile, outputFile);
			sw.Stop();
			Console.WriteLine("program work time: " + sw.ElapsedMilliseconds);
		}
	}
}