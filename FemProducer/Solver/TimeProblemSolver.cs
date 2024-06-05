using FemProducer.Collector;
using FemProducer.Logger;
using FemProducer.Services;

using Grid.Models;
using Grid.Models.InputModels;

using MathModels.Models;

using SlaeSolver.Interfaces;

namespace FemProducer.Solver
{
    public class TimeProblemSolver(ISolver solver, SolutionService solutionService, AbstractCollector collector, ResultsService<TxtLogger> resultsService, GridInputParameters gridParameters, GridModel grid) : IProblemSolver
    {
        void IProblemSolver.Solve(string configureFile, string outputFile)
        {
            // solutionService.NumericalSolves.Add(solutionService.AnalyticsSolves[0]);
            // solutionService.NumericalSolves.Add(solutionService.AnalyticsSolves[1]);
            //  resultsService.PrintResult(0, false);
            //  resultsService.PrintResult(1, false);

            for (int timeLayer = 0; timeLayer < grid.T.Count - 1; timeLayer++)
            {
                Slae slae = collector.Collect(timeLayer);
                resultsService.PrintSlaeByComponents("slae", slae);
                Vector solve = solver.Solve(slae);
                solutionService.NumericalSolves.Add(solve);
                //resultsService.PrintSlae(slae);
                resultsService.PrintResult(timeLayer, true);
                //			Console.WriteLine(timeLayer + "/" + grid.T.Count);
            }
            resultsService.WriteSolve("solve.txt", solutionService.NumericalSolves);
            //	resultsService.PrintResult(0, true);

            resultsService.WriteSolveWithGrid("isolines.txt", solutionService.NumericalSolves[0]);
            //consoleLogger.Log();
            // solvesOutputer.Show(outputFile);
        }
    }
}
