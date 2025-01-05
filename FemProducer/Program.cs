using FemProducer.Basises.Implementations.ThreeDimensional;
using FemProducer.Collectors.Abstractions;
using FemProducer.Collectors.CollectorBases.Implementations;
using FemProducer.Collectors.Implementations;
using FemProducer.ConfigureReader;
using FemProducer.Logger;
using FemProducer.Services;
using FemProducer.Solver;

using Grid.Factories.ElementFactory.Implemenations;
using Grid.Factories.GridFactories.Implementations;
using Grid.Factories.GridFactories.Interfaces;
using Grid.Factories.NodeFactory.Implementations;
using Grid.Models;

using MathModels.Models;

using SlaeSolver.Implementations.Factories;
using SlaeSolver.Interfaces;

using Tools;

namespace FemProducer;

internal class Program
{
    private const string ConfigureFile = "ConfigureTask.json";
    private const string OutputFile = "output.txt";

    private static void Main()

    {
        //  return;
        var sw = new System.Diagnostics.Stopwatch();

        IConfigureReader taskBuilder = new JsonConfigureReader(ConfigureFile);

        var problemParameters = taskBuilder.GetProblemParameters();
        var solverParameters = taskBuilder.GetSolverParameters();
        var gridParameters = taskBuilder.GetGridParameters();


        IGridFactory gridFactory = new GridFactory(new CubeElementFactory(), new QuadrilateralNodeFactory(), new());
        ISolverFactory solverFactory = new SolverFactory();

        var solver = solverFactory.CreateSolver(solverParameters);
        var grid = gridFactory.GetGrid(gridParameters);


        Messages.PrintSuccessMessage("The grid was built!");

        var problemService = new ProblemService(problemParameters);

        var solutionService = new SolutionService(problemService, grid);

        var matrixFactory = new MatrixFactory.MatrixFactory();

        var collectorBase = new CollectorBase(grid, matrixFactory, problemService, new LinearHexagonsBasis(problemService));

        // AbstractCollector timeCollector = new TimeCollector(solutionService, collectorBase, grid, matrixFactory);
        AbstractCollector timeCollector = new EllipticCollector(collectorBase, grid, matrixFactory);
        var resultsService = new ResultsService<TxtLogger>(new("results"), grid, solutionService, problemService);

        resultsService.WriteGrid("grid.txt", gridParameters);
        Processes.OpenPythonScript(@"PythonScripts\grid2d.py", "C:\\Users\\hardb\\source\\repos\\FEM\\FemProducer\\bin\\Debug\\net8.0\\grid.txt",
            "Oxy", "blue", "xy");
        resultsService.WriteGrid2("grid2.txt", gridParameters);
        Processes.OpenPythonScript(@"PythonScripts\grid2d.py", "C:\\Users\\hardb\\source\\repos\\FEM\\FemProducer\\bin\\Debug\\net8.0\\grid2.txt",
            "Oxz", "red", "xz");

        IProblemSolver problemSolver = new TimeProblemSolver(solver, solutionService, timeCollector, resultsService, gridParameters, grid);

        sw.Start();

        problemSolver.Solve(ConfigureFile, OutputFile);

        resultsService.PrintResult(0, true);

        GetVDifference(solutionService.NumericalSolves[0], grid);

        Messages.PrintSuccessMessage("program work time: " + sw.ElapsedMilliseconds);

        Processes.OpenPythonScript(@"PythonScripts\temperature.py");

        Console.ReadKey();
    }

    private static void GetVDifference(Vector solution, GridModel grid)
    {
        foreach (var scheme in grid.ReceivingLines)
            Console.WriteLine(Math.Abs(solution[scheme.NodesIndexes[0]] - solution[scheme.NodesIndexes[4]]));
    }
}