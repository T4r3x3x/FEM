using Autofac;

using FemProducer.Basises;
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

			ContainerBuilder container = new ContainerBuilder();
			container.RegisterType<JsonConfigureReader>().As<IConfigureReader>();
			container.RegisterType<GridFactory>().As<IGridFactory>();
			container.RegisterType<SolverFactory>().As<ISolverFactory>();
			container.RegisterType<ProblemService>();
			container.RegisterType<SolutionService>();
			container.RegisterType<MatrixFactory>().As<IMatrixFactory>();
			container.RegisterType<CollectorBase>().As<ICollectorBase>();
			container.RegisterType<EllipticCollector>().As<AbstractCollector>();
			container.RegisterType<LinearCubeBasis>().As<AbstractBasis>();
			container.RegisterType<ResultsService<TxtLogger>>();
			container.RegisterType<TimeProblemSolver>().As<IProblemSolver>();

			var build = container.Build();
			var reader = build.Resolve<IConfigureReader>(new NamedParameter("filePath", ConfigureFile));

			var problemParameters = reader.GetProblemParameters();
			var solverParameters = reader.GetSolverParameters();
			var gridParameters = reader.GetGridParameters();

			var gridFactory = build.Resolve<IGridFactory>();
			var solverFactory = build.Resolve<ISolverFactory>();

			//	SolverFactory solverFactory = new();
			//	GridFactory gridFactory = new();

			ISolver solver = solverFactory.CreateSolver(solverParameters);
			GridModel grid = gridFactory.GetGrid(gridParameters);

			Messages.PrintSuccessMessage("The grid was built!");

			ProblemService problemService = new ProblemService(problemParameters);
			SolutionService solutionService = new SolutionService(problemService, grid);

			MatrixFactory matrixFactory = new();

			CollectorBase collectorBase = new(grid, matrixFactory, problemService, new Basises.LinearCubeBasis(problemService));
			EllipticCollector timeCollector = new EllipticCollector(collectorBase, grid, matrixFactory);

			ResultsService<TxtLogger> resultsService = new(new TxtLogger("results"), grid, solutionService, problemService);


			var _problemSolver = build.Resolve<IProblemSolver>();
			IProblemSolver problemSolver = new TimeProblemSolver(solver, solutionService, timeCollector, resultsService, gridParameters, grid);

			//	try
			//	{
			sw.Start();

			Console.WriteLine(grid.ElementsCount);

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
			//Tools.Processes.OpenPythonScript(scriptPath: @"PythonScripts\grid2d.py");
			Tools.Processes.OpenPythonScript(@"PythonScripts\temperature.py");
		}
	}
}