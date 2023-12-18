using FemProducer.AppBuilder;
using FemProducer.Collector;
using FemProducer.Logger;
using FemProducer.MatrixBuilding;

using Grid.Implementations.Factories;
using Grid.Interfaces;
using Grid.Models;

using MathModels.Models;

using SlaeSolver.Implementations.Factories;
using SlaeSolver.Interfaces;

using Tools;

namespace FemProducer
{
	internal class TimeProblemSolver : IProblemSolver
	{
		void IProblemSolver.Solve(string configureFile, string outputFile)
		{
			ConsoleLogger consoleLogger = new();
			IAppBuilder taskBuilder = new JsonAppBuilder(configureFile);

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

			CollectorBase collector = new(grid, matrixFactory, problemService, new Basises.LinearQuadrangularCartesianBasis(problemService));
			TimeCollector timeCollector = new TimeCollector(solutionService, collector, grid, matrixFactory);

			ResultsService<TxtLogger> solvesOutputer = new(new TxtLogger("results"), grid, solutionService, problemService);

			solutionService.NumericalSolves.Add(solutionService.AnalyticsSolves[0]);
			solutionService.NumericalSolves.Add(solutionService.AnalyticsSolves[1]);

			for (int timeLayer = 2; timeLayer < grid.T.Count; timeLayer++)
			{
				Slae slae = timeCollector.Collect(timeLayer);
				Vector solve = solver.Solve(slae);
				solutionService.NumericalSolves.Add(solve);
				solvesOutputer.PrintResult(timeLayer, true);
			}

			solvesOutputer.WriteSolve("solve.txt", solutionService.NumericalSolves[0]);
			solvesOutputer.WriteGrid("grid.txt");
			solvesOutputer.WriteSolveWithGrid("isolines.txt", solutionService.NumericalSolves[0]);
			//consoleLogger.Log();
			//solvesOutputer.Show(outputFile);
		}
	}
}
