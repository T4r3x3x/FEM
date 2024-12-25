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

using SlaeSolver.Implementations.Factories;
using SlaeSolver.Interfaces;

using Tools;

namespace FemProducer;

internal class Program
{
    private const string ConfigureFile = "ConfigureTask.json";
    private const string ConfigureFileAdditionalField = "ConfigureTaskAdditional.json";
    private const string OutputFile = "output.txt";

    private static void Main()

    {
        //  return;
        var sw = new System.Diagnostics.Stopwatch();

        IConfigureReader taskBuilder = new JsonConfigureReader(ConfigureFile);
        IConfigureReader additionalTaskBuilder = new JsonConfigureReader(ConfigureFileAdditionalField);

        var problemParameters = taskBuilder.GetProblemParameters();
        var solverParameters = taskBuilder.GetSolverParameters();
        var gridParameters = taskBuilder.GetGridParameters();

        var additionalGridParameters = additionalTaskBuilder.GetGridParameters();

        IGridFactory gridFactory = new GridFactory(new CubeElementFactory(), new QuadrilateralNodeFactory(), new());
        ISolverFactory solverFactory = new SolverFactory();

        var solver = solverFactory.CreateSolver(solverParameters);
        var grid = gridFactory.GetGrid(gridParameters);
        var additionalGrid = gridFactory.GetGrid(additionalGridParameters);


        Messages.PrintSuccessMessage("The grid was built!");

        var problemService = new ProblemService(problemParameters);
        var additionalFieldProblemService = new AdditionalFieldProblemService(problemParameters);
        additionalFieldProblemService.NodesIndexes = grid.NodesIndexes;

        var solutionService = new SolutionService(problemService, grid);
        var additionalSolutionService = new SolutionService(additionalFieldProblemService, additionalGrid);

        var matrixFactory = new MatrixFactory.MatrixFactory();

        var collectorBase = new CollectorBase(grid, matrixFactory, problemService, new LinearCubeBasis(problemService));
        var additionalFieldCollectorBase = new CollectorBase(additionalGrid, matrixFactory, additionalFieldProblemService, new LinearCubeBasis(problemService));

        // AbstractCollector timeCollector = new TimeCollector(solutionService, collectorBase, grid, matrixFactory);
        AbstractCollector timeCollector = new EllipticCollector(collectorBase, grid, matrixFactory);
        AbstractCollector additionalCollector = new EllipticCollector(additionalFieldCollectorBase, additionalGrid, matrixFactory);
        var resultsService = new ResultsService<TxtLogger>(new("results"), grid, solutionService, problemService);
        var additionalResultsService = new ResultsService<TxtLogger>(new("results"), additionalGrid, additionalSolutionService, additionalFieldProblemService);

        resultsService.WriteGrid("grid.txt", gridParameters);
        Processes.OpenPythonScript(@"PythonScripts\grid2d.py", "C:\\Users\\hardb\\source\\repos\\FEM\\FemProducer\\bin\\Debug\\net8.0\\grid.txt",
            "Oxy", "blue", "xy");
        resultsService.WriteGrid2("grid2.txt", gridParameters);
        Processes.OpenPythonScript(@"PythonScripts\grid2d.py", "C:\\Users\\hardb\\source\\repos\\FEM\\FemProducer\\bin\\Debug\\net8.0\\grid2.txt",
            "Oxz", "red", "xz");

        IProblemSolver problemSolver = new TimeProblemSolver(solver, solutionService, timeCollector, resultsService, gridParameters, grid);
        IProblemSolver additionalProblemSolver = new TimeProblemSolver(solver, additionalSolutionService, additionalCollector, additionalResultsService, additionalGridParameters, additionalGrid);

        sw.Start();

        problemSolver.Solve(ConfigureFile, OutputFile);

        additionalFieldProblemService.Q = solutionService.NumericalSolves[0];

        additionalProblemSolver.Solve(ConfigureFile, OutputFile);
        var center = additionalGrid.NodesIndexes.Where(x => x.Key.Z == -15 && x.Key.Y >= 60 && x.Key.Y <= 90 && x.Key.X >= 60 && x.Key.X <= 90);

        foreach (var _center in center)
        {
            Console.WriteLine(solutionService.NumericalSolves[0][_center.Value]);
        }

        resultsService.PrintResult(0, false);

        Messages.PrintSuccessMessage("program work time: " + sw.ElapsedMilliseconds);

        Processes.OpenPythonScript(@"PythonScripts\temperature.py");

        Console.ReadKey();
    }
}