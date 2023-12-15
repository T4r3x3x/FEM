namespace FemProducer
{
	class Program
	{
		const string configureFile = "Confige.json";
		const string outputFile = "output.txt";

		static void Main(string[] args)
		{
			System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
			try
			{
				sw.Start();
				IProblemSolver problemSolver = new TimeProblemSolver();
				problemSolver.Solve(configureFile, outputFile);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message, Console.BackgroundColor = ConsoleColor.DarkRed);

				Console.BackgroundColor = ConsoleColor.Gray;
			}
			sw.Stop();
			Console.WriteLine("program work time: " + sw.ElapsedMilliseconds);
		}
	}
}