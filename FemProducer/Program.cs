using FemProducer.Collector;
using FemProducer.ConfigureReader;
using FemProducer.Logger;
using FemProducer.MatrixBuilding;
using FemProducer.Services;
using FemProducer.Solver;

using Grid.Factories;
using Grid.Factories.ElementFactory.Implemenations;
using Grid.Factories.GridFactory.Implementations;
using Grid.Factories.GridFactory.Interfaces;
using Grid.Factories.NodeFactory.Implementations;
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

            IConfigureReader taskBuilder = new JsonConfigureReader(ConfigureFile);

            var problemParameters = taskBuilder.GetProblemParameters();
            var solverParameters = taskBuilder.GetSolverParameters();
            var gridParameters = taskBuilder.GetGridParameters();

            IGridFactory gridFactory = new GridFactory(8, new CubeElementFactory(), new QuadrilateralNodeFactory(), new BoundaryFactory());
            ISolverFactory solverFactory = new SolverFactory();

            ISolver solver = solverFactory.CreateSolver(solverParameters);
            GridModel grid = gridFactory.GetGrid(gridParameters);

            Messages.PrintSuccessMessage("The grid was built!");


            ProblemService problemService = new ProblemService(problemParameters);
            SolutionService solutionService = new SolutionService(problemService, grid);

            MatrixFactory matrixFactory = new();

            CollectorBase collectorBase = new(grid, matrixFactory, problemService, new Basises.LinearCubeBasis(problemService));
            // AbstractCollector timeCollector = new TimeCollector(solutionService, collectorBase, grid, matrixFactory);
            AbstractCollector timeCollector = new EllipticCollector(collectorBase, grid, matrixFactory);
            ResultsService<TxtLogger> resultsService = new(new TxtLogger("results"), grid, solutionService, problemService);

            resultsService.WriteGrid("grid.txt", gridParameters);
            //    resultsService.WriteGrid2("grid2.txt", gridParameters);
            Tools.Processes.OpenPythonScript(scriptPath: @"PythonScripts\grid2d.py");


            IProblemSolver problemSolver = new TimeProblemSolver(solver, solutionService, timeCollector, resultsService, gridParameters, grid);

            //	try
            //	{
            sw.Start();

            problemSolver.Solve(ConfigureFile, OutputFile);


            //resultsService.PrintSlae();
            //       resultsService.PrintResult(1, false);
            //      resultsService.WriteSolve("sovle");
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
            Tools.Processes.OpenPythonScript(@"PythonScripts\grid2d2.py");
            Tools.Processes.OpenPythonScript(@"PythonScripts\temperature.py");
        }
    }
}