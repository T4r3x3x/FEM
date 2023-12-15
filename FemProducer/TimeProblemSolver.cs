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

			consoleLogger.Log("The grid was built!");

			MatrixFactory matrixFactory = new();

			CollectorBase collector = new(grid, matrixFactory, problemParameters, new Basises.LinearRectangularCylindricalBasis());
			SimpleCollector timeCollector = new SimpleCollector(collector, grid, matrixFactory, problemParameters);

			SolutionService resultProducer = new SolutionService(problemParameters, grid);

			ProgramResultsService<TxtLogger> solvesOutputer = new(new TxtLogger("results"), grid, resultProducer, problemParameters);


			Slae slae = timeCollector.Collect(-1);
			consoleLogger.Log(message: "The slae was collected!");

			//	solvesOutputer.PrintSlae(slae);
			Vector solve = solver.Solve(slae);
			consoleLogger.Log(message: "The slae was solved!");
			resultProducer.NumericalSolves.Add(solve);

			solvesOutputer.PrintResult(-1, true);
			//consoleLogger.Log();
			//solvesOutputer.Show(outputFile);
		}
	}
}
