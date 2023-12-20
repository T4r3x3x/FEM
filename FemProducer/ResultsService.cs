using System.Text;

using FemProducer.Logger;

using Grid.Models;

using MathModels.Models;

namespace FemProducer
{
	/// <summary>
	/// Класс отвечает за запись результатов работы программы
	/// </summary>
	public class ResultsService<TLogger> where TLogger : ILogger
	{
		private readonly TLogger _logger;
		private readonly GridModel _grid;
		private readonly SolutionService _resultProducer;
		private readonly StringBuilder stringBuilder = new StringBuilder();
		private readonly ProblemService _problemParametrs;

		public ResultsService(TLogger logger, GridModel grid, SolutionService resultProducer, ProblemService problemParametrs)
		{
			_logger = logger;
			_grid = grid;
			_resultProducer = resultProducer;
			_problemParametrs = problemParametrs;
		}

		public void PrintTimeGrid()
		{
			stringBuilder.Clear();
			stringBuilder.AppendLine();

			for (int i = 0; i < _grid.T.Count; i++)
				stringBuilder.AppendLine("[" + _grid.T[i] + "] ");

			stringBuilder.AppendLine();

			_logger.Log(stringBuilder.ToString());
		}

		public void PrintSlae(Slae slae)
		{
			double[][] matrix = slae.Matrix.ConvertToDenseFormat();
			for (int i = 0; i < matrix.Length; i++)
			{
				for (int j = 0; j < matrix.Length; j++)
				{
					if (matrix[i][j] >= 0)
						Console.Write(" ");
					Console.Write(matrix[i][j].ToString("E2") + " ");
				}
				Console.Write(" " + slae.Vector[i].ToString("E2"));
				Console.WriteLine();
			}
			Console.WriteLine("\n\n");
		}


		public void PrintResult(int timeLayer, bool printSolution)
		{
			Vector exactSolution = _resultProducer.AnalyticsSolves[timeLayer];
			var solution = _resultProducer.NumericalSolves[timeLayer];

			if (printSolution)
			{
				Console.WriteLine("      Численное решение      |         точное решение        |      разница решений      ");
				Console.WriteLine("--------------------------------------------------------------------------------------");
			}

			if (printSolution)
				for (int i = 0; i < solution.Length; i++)
					Console.WriteLine("u{0} = {1:E16} | u*{0} = {2:E16} | {3:E16}", i + 1, solution[i], exactSolution[i], Math.Abs(exactSolution[i] - solution[i]));

			Console.WriteLine("Относительная погрешность: " + _resultProducer.GetSolveDifference(timeLayer));
		}

		public void WriteSolveWithNodes(string path, Vector solve)
		{
			using (var file = File.Open(path, FileMode.OpenOrCreate))
			using (StreamWriter sw = new StreamWriter(file))
			{
				sw.WriteLine(_grid.Subdomains.Count);
				foreach (var subDomain in _grid.Subdomains)
				{
					foreach (var value in subDomain)
					{
						sw.Write(value.ToString().Replace(',', '.') + " ");
					}
					sw.WriteLine();
				}

				sw.WriteLine(_grid.Nodes.Count);

				for (int i = 0; i < _grid.Nodes.Count; i++)
				{
					sw.Write(_grid.Nodes[i].ToString().Replace(",", ".") + " ");
					sw.Write(solve[i].ToString().Replace(",", ".") + "\n");
				}
			}
		}

		public void WriteSolve(string path, Vector solve)
		{
			using (var file = File.Open(path, FileMode.OpenOrCreate))
			using (StreamWriter sw = new StreamWriter(file))
				foreach (var value in solve)
					sw.WriteLine(value.ToString().Replace(",", "."));
		}

		public void WriteGrid(string path, GridParameters gridParameter)
		{
			using (var file = File.Open(path, FileMode.OpenOrCreate))
			using (StreamWriter sw = new StreamWriter(file))
			{
				var x = _grid.Nodes.Select(node => node.X).Order();
				var y = _grid.Nodes.Select(node => node.Y).Order();
				var xmin = x.First();
				var xmax = x.Last();
				var ymin = y.First();
				var ymax = y.Last();

				sw.Write(xmin + " ");
				sw.Write(xmax + " ");
				sw.Write(ymin + " ");
				sw.Write(ymax + " \n");

				sw.WriteLine(_grid.Elements.Count);
				foreach (var element in _grid.Elements)
				{
					sw.WriteLine(_grid.Nodes[element.NodesIndexes[0]]);
					sw.WriteLine(_grid.Nodes[element.NodesIndexes[1]]);
					sw.WriteLine(_grid.Nodes[element.NodesIndexes[3]]);
					sw.WriteLine(_grid.Nodes[element.NodesIndexes[2]]);
					sw.WriteLine();
				}
			}
		}
	}
}