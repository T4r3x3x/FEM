using FemProducer.Collector;
using FemProducer.ConfigureReader;
using FemProducer.Logger;
using FemProducer.MatrixBuilding;

using Grid.Implementations.Factories;
using Grid.Interfaces;
using Grid.Models;

using SlaeSolver.Implementations.Factories;
using SlaeSolver.Interfaces;

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

			IConfigureReader taskBuilder = new JsonConfigureReader(ConfigureFile);

			var problemParameters = taskBuilder.GetProblemParameters();
			var solverParameters = taskBuilder.GetSolverParameters();
			var gridParameters = taskBuilder.GetGridParameters();

			IGridFactory gridFactory = new GridFactory();
			ISolverFactory solverFactory = new SolverFactory();

			ISolver solver = solverFactory.CreateSolver(solverParameters);
			GridModel grid = gridFactory.GetGrid(gridParameters);

			Messages.PrintSuccessMessage("The grid was built!");

			ProblemService problemService = new ProblemService(problemParameters);
			SolutionService solutionService = new SolutionService(problemService, grid);

			MatrixFactory matrixFactory = new();

			CollectorBase collectorBase = new(grid, matrixFactory, problemService, new Basises.LinearRectangularCylindricalBasis(problemService));
			EllipticCollector timeCollector = new EllipticCollector(collectorBase, grid, matrixFactory);

			ResultsService<TxtLogger> resultsService = new(new TxtLogger("results"), grid, solutionService, problemService);



			IProblemSolver problemSolver = new TimeProblemSolver(solver, solutionService, timeCollector, resultsService, gridParameters, grid);

			//	try
			//	{
			sw.Start();

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
			Tools.Processes.OpenPythonScript(scriptPath: @"PythonScripts\grid2d.py");
			Tools.Processes.OpenPythonScript(@"PythonScripts\temperature.py");
		}
	}
}