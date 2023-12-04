using FemProducer.Assemblier;
using FemProducer.Logger;

using ReaserchPaper;
using ReaserchPaper.Assemblier;
using ReaserchPaper.Grid;
using ReaserchPaper.Logger;
using ReaserchPaper.Solver;

using ResearchPaper;

using Tensus;

namespace FemProducer
{
	internal class TimeProblemSolver : IProblemSolver
	{
		void IProblemSolver.Solve(string configureFile, string outputFile)
		{
			ConsoleLogger consoleLogger = new();
			ITaskBuilder taskBuilder = new JsonTaskBuilder(configureFile, consoleLogger);

			taskBuilder.GetGridParameters();
			ProblemParametrs problemParameters = taskBuilder.GetProblem();
			ISolver solver = taskBuilder.GetSolver();

			GridFactory gridFactory = new GridFactory();
			Grid grid = gridFactory.GetGrid(taskBuilder.GetGridParameters());

			MatrixFactory matrixFactory = new(grid);

			Matrix matrix = matrixFactory.CreateMatrix();
			Vector vector = new Vector(grid.NodesCount);

			CollectorBase collector = new(grid, matrixFactory, problemParameters);
			BasicCollector timeCollector = new BasicCollector(collector, grid, matrixFactory, problemParameters);

			ResultProducer resultProducer = new ResultProducer(problemParameters, grid);
			var logger = new ConsoleLogger();

			Outputer<TxtLogger> solvesOutputer = new(new TxtLogger("results"), grid, resultProducer, problemParameters);

			Vector solve = new(grid.NodesCount);

			Slae slae;
			List<Vector> solves = new List<Vector>();
			for (int timeLayer = 0; timeLayer < grid.T.Length; timeLayer++)
			{
				slae = timeCollector.Collect(timeLayer);
				solvesOutputer.PrintSlae(slae);
				solve = solver.Solve(slae);
				resultProducer.NumericalSolves.Add(solve);
			}
			solvesOutputer.PrintResult(-1, true);
			//consoleLogger.Log();
			solvesOutputer.Show(outputFile);
		}
	}
}
