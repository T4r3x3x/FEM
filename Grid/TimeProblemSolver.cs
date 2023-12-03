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
		void IProblemSolver.Solve()
		{
			ITaskBuilder taskBuilder = new JsonTaskBuilder();

			ProblemParametrs problemParamters = taskBuilder.GetProblem();
			ISolver solver = taskBuilder.GetSolver();

			GridFactory gridFactory = new GridFactory();
			Grid grid = gridFactory.GetGrid(taskBuilder.GetGridParametrs());

			MatrixFactory matrixFactory = new(grid);

			Matrix matrix = matrixFactory.CreateMatrix();
			Vector vector = new Vector(grid.NodesCount);
			Slae slae = new Slae(matrix, vector);

			Collector collector = new(grid, matrixFactory);
			CollectorTimeDecorator timeCollector = new CollectorTimeDecorator(collector, grid);

			ResultProducer resultProducer = new ResultProducer(problemParamters, grid);
			var logger = new ConsoleLogger();
			Outputer<ConsoleLogger> programStateOutputer = new(logger, grid, resultProducer);
			Outputer<TxtLogger> solvesOutputer = new(new TxtLogger("results"), grid, resultProducer);

			Vector solve = new(grid.NodesCount);

			List<Vector> solves = new List<Vector>();
			for (int timeLayer = 0; timeLayer < grid.T.Length; timeLayer++)
			{
				slae = timeCollector.Collect(timeLayer);
				solve = solver.Solve(slae);
				solves.Add(solve);
			}
			solvesOutputer.Show();
		}
	}
}
