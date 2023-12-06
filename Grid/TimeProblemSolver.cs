using FemProducer.Assemblier;
using FemProducer.Logger;

using ReaserchPaper;
using ReaserchPaper.Assemblier;
using ReaserchPaper.Grid;
using ReaserchPaper.Logger;
using ReaserchPaper.Solver;

using Tensus;

namespace FemProducer
{
	internal class TimeProblemSolver : IProblemSolver
	{
		void IProblemSolver.Solve(string configureFile, string outputFile)
		{
			ConsoleLogger consoleLogger = new();
			ITaskBuilder taskBuilder = new JsonTaskBuilder(configureFile, consoleLogger);

			var problemParameters = taskBuilder.GetProblemParameters();
			var solverParameters = taskBuilder.GetSolverParameters();
			var gridParameters = taskBuilder.GetGridParameters();

			GridFactory gridFactory = new GridFactory();
			SolverFactory solverFactory = new SolverFactory();

			ISolver solver = solverFactory.CreateSolver(solverParameters);
			Grid grid = gridFactory.GetGrid(gridParameters);

			consoleLogger.Log("The grid was built!");

			MatrixFactory matrixFactory = new(grid);

			CollectorBase collector = new(grid, matrixFactory, problemParameters);
			BasicCollector timeCollector = new BasicCollector(collector, grid, matrixFactory, problemParameters);

			ResultProducer resultProducer = new ResultProducer(problemParameters, grid);

			Outputer<TxtLogger> solvesOutputer = new(new TxtLogger("results"), grid, resultProducer, problemParameters);

			Vector solve = new(grid.NodesCount);

			Slae slae = timeCollector.Collect(-1);
			consoleLogger.Log(message: "The slae was collected!");

			//	solvesOutputer.PrintSlae(slae);
			solve = solver.Solve(slae);
			consoleLogger.Log(message: "The slae was solved!");
			resultProducer.NumericalSolves.Add(solve);

			solvesOutputer.PrintResult(-1, true);
			//consoleLogger.Log();
			//solvesOutputer.Show(outputFile);
		}
	}
}
