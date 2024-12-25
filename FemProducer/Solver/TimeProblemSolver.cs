using FemProducer.Collectors.Abstractions;
using FemProducer.Logger;
using FemProducer.Services;

using Grid.Models;
using Grid.Models.InputModels;

using MathModels.Models;

using SlaeSolver.Interfaces;

namespace FemProducer.Solver;

public class TimeProblemSolver(ISolver solver, SolutionService solutionService, AbstractCollector collector, ResultsService<TxtLogger> resultsService, GridInputParameters gridParameters, GridModel grid) : IProblemSolver
{
    void IProblemSolver.Solve(string configureFile, string outputFile)
    {
        // solutionService.NumericalSolves.Add(solutionService.AnalyticsSolves[0]);
        // solutionService.NumericalSolves.Add(solutionService.AnalyticsSolves[1]);
        //  resultsService.PrintResult(0, false);
        //  resultsService.PrintResult(1, false);

        for (var timeLayer = 0; timeLayer < grid.T.Count - 1; timeLayer++)
        {
            var slae = collector.Collect(timeLayer);
            //  resultsService.PrintSlaeByComponents("slae", slae);
            var solve = solver.Solve(slae);
            solutionService.NumericalSolves.Add(solve);
            //  resultsService.PrintSlae(slae);
            //    resultsService.PrintResult(timeLayer, false);

            Console.WriteLine();
            Console.WriteLine("///////////////////////////");
            Console.WriteLine("Slae size: {0}", slae.Size);
            Console.WriteLine("///////////////////////////");
            Console.WriteLine();
            //			Console.WriteLine(timeLayer + "/" + grid.T.Count);
        }
        resultsService.WriteSolve("solve.txt", solutionService.NumericalSolves);
        //	resultsService.PrintResult(0, true);

        resultsService.WriteSolveWithGrid("isolines.txt", solutionService.NumericalSolves[0]);
        //consoleLogger.Log();
        // solvesOutputer.Show(outputFile);
    }
}