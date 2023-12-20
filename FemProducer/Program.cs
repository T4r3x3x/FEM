using Tools;

namespace FemProducer
{
	class Program
	{
		const string ConfigureFile = "ConfigureTask.json";
		const string OutputFile = "output.txt";

		static void Main(string[] args)
		{
			System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();

			//	try
			//	{
			sw.Start();
			IProblemSolver problemSolver = new TimeProblemSolver();
			problemSolver.Solve(ConfigureFile, OutputFile);
			//}
			//catch (ValidationException ex)
			//{
			//	Messages.PrintErrorMessage(ex.Message);
			//}
			//catch (JsonReaderException ex)
			//{
			//	Messages.PrintErrorMessage(ex.Message);
			//}
			//catch (AggregateException exes)
			//{
			//	//Проблема в том, что ошибку кидает каждый из потоков из-за чего она дублируется, нет смысла обрабатывать по отдельности каждую
			//	var ex = exes.Flatten().InnerExceptions[0];

			//	if (ex is ArgumentException)
			//		Messages.PrintErrorMessage(ex.Message);
			//	else
			//		throw new Exception();
			//}
			//catch (ArgumentException ex)
			//{
			//	Messages.PrintErrorMessage(ex.Message);
			//}
			//catch
			//{
			//	Messages.PrintErrorMessage("Inner unknown error :(");
			//}
			sw.Stop();
			Messages.PrintSuccessMessage("program work time: " + sw.ElapsedMilliseconds);

			//Tools.Processes.OpenPythonScript("isolines.py");
		}
	}
}