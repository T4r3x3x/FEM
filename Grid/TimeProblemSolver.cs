using FemProducer.AppBuilder;
using FemProducer.Collector;
using FemProducer.Grid;
using FemProducer.Logger;
using FemProducer.Models;
using FemProducer.Solver;

using Tensus;

namespace FemProducer
{
	internal class TimeProblemSolver : IProblemSolver
	{
		void IProblemSolver.Solve(string configureFile, string outputFile)
		{
			ConsoleLogger consoleLogger = new();
			IAppBuilder taskBuilder = new JsonAppBuilder(configureFile, consoleLogger);

			var problemParameters = taskBuilder.GetProblemParameters();
			var solverParameters = taskBuilder.GetSolverParameters();
			var gridParameters = taskBuilder.GetGridParameters();

			IGridFactory gridFactory = new GridFactory();
			SolverFactory solverFactory = new SolverFactory();

			ISolver solver = solverFactory.CreateSolver(solverParameters);
			GridModel grid = gridFactory.GetGrid(gridParameters);

			consoleLogger.Log("The grid was built!");

			MatrixFactory matrixFactory = new(grid);

			CollectorBase collector = new(grid, matrixFactory, problemParameters);
			SimpleCollector timeCollector = new SimpleCollector(collector, grid, matrixFactory, problemParameters);

			ResultService resultProducer = new ResultService(problemParameters, grid);

			SolutionService<TxtLogger> solvesOutputer = new(new TxtLogger("results"), grid, resultProducer, problemParameters);

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
