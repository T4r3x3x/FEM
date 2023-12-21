using FemProducer.Collector;
using FemProducer.Logger;

using Grid.Models;

using MathModels.Models;

using SlaeSolver.Interfaces;

namespace FemProducer
{
	public class TimeProblemSolver(ISolver solver, SolutionService solutionService, AbstractCollector collector, ResultsService<TxtLogger> resultsService, GridParameters gridParameters, GridModel grid) : IProblemSolver
	{
		void IProblemSolver.Solve(string configureFile, string outputFile)
		{
			//	solutionService.NumericalSolves.Add(solutionService.AnalyticsSolves[0]);
			//	solutionService.NumericalSolves.Add(solutionService.AnalyticsSolves[1]);

			//	for (int timeLayer = 2; timeLayer < grid.T.Count; timeLayer++)
			//		{
			Slae slae = collector.Collect(0);
			Vector solve = solver.Solve(slae);
			solutionService.NumericalSolves.Add(solve);
			resultsService.PrintResult(0, false);
			//	}

			resultsService.WriteSolve("solve.txt", solutionService.NumericalSolves[0]);
			resultsService.WriteGrid("grid.txt", gridParameters);
			//	resultsService.WriteSolveWithNodes("isolines.txt", solutionService.NumericalSolves[0]);
			//consoleLogger.Log();
			//solvesOutputer.Show(outputFile);
		}
	}
}
