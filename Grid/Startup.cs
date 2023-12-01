using FemProducer;
using FemProducer.Assemblier;
using FemProducer.Logger;

using ReaserchPaper;
using ReaserchPaper.Assemblier;
using ReaserchPaper.Grid;
using ReaserchPaper.Logger;
using ReaserchPaper.Solver;

using Tensus;

namespace ResearchPaper
{
	class Startup
	{
		static void Main(string[] args)
		{
			System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
			sw.Start();

			ITaskBuilder taskBuilder = new JsonTaskBuilder();

			Problem problem = taskBuilder.GetProblem();
			ISolver solver = taskBuilder.GetSolver();

			GridFactory gridFactory = new GridFactory(taskBuilder.GetGridParametrs());
			Grid grid = gridFactory.GetGrid();

			MatrixFactory matrixFactory = new(grid);

			Matrix matrix = matrixFactory.CreateMatrix();
			Vector vector = new Vector(grid.NodesCount);
			Slae slae = new Slae(matrix, vector);

			Collector collector = new(grid.NodesCount);
			CollectorTimeDecorator timeCollector = new CollectorTimeDecorator(collector, grid);

			ResultProducer resultProducer = new ResultProducer();

			var logger = new ConsoleLogger();
			Outputer<ConsoleLogger> programStateOutputer = new(logger, grid, resultProducer);
			Outputer<TxtLogger> solvesOutputer = new(new TxtLogger("results"), grid, resultProducer);

			Vector solve = new(grid.NodesCount);


			for (int timeLayer = 0; timeLayer < grid.T.Length; timeLayer++)
			{
				slae = timeCollector.Collect(timeLayer);
				solve = solver.Solve(slae);
				resultProducer.AddSolve(solve);
			}

			sw.Stop();
			solvesOutputer.Show();
			Console.WriteLine(sw.ElapsedMilliseconds);
		}
	}
}