using System.Text;

using FemProducer.Grid;
using FemProducer.Logger;
using FemProducer.Models;

using Tensus;

namespace FemProducer
{
	/// <summary>
	/// Класс отвечает за запись результата работы программы
	/// </summary>
	internal class SolutionService<TLogger> where TLogger : ILogger
	{
		private readonly TLogger _logger;
		private readonly GridModel _grid;
		private readonly ResultService _resultProducer;
		private readonly StringBuilder stringBuilder = new StringBuilder();
		private readonly ProblemService _problemParametrs;

		public SolutionService(TLogger logger, GridModel grid, ResultService resultProducer, ProblemService problemParametrs)
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

			for (int i = 0; i < _grid.T.Length; i++)
				stringBuilder.AppendLine("[" + _grid.T[i] + "] ");

			stringBuilder.AppendLine();

			_logger.Log(stringBuilder.ToString());
		}
		public void PrintPartialGrid()
		{
			stringBuilder.Clear();
			for (int j = 0; j < _grid.M; j++)
			{
				for (int i = 0; i < _grid.N; i++)
				{
					var line = string.Format(" [{0}, {1}] ", _grid.X[i].ToString("e2"), _grid.Y[j].ToString("e2"));
					stringBuilder.Append(line);
				}
				stringBuilder.AppendLine();
			}
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


		public void PrintResult(int timeLayer, bool isPrint)
		{
			Vector exactSolution = _resultProducer.AnalyticsSolves[0];
			var solution = _resultProducer.NumericalSolves[0];

			if (isPrint)
			{
				Console.WriteLine("      Численное решение      |         точное решение        |      разница решений      ");
				Console.WriteLine("--------------------------------------------------------------------------------------");

			}
			if (timeLayer == -1)
			{
				for (int i = 0; i < _grid.M; i++)
					for (int j = 0; j < _grid.N; j++)
						exactSolution[i * _grid.N + j] = _problemParametrs.Func1(_grid.X[j], _grid.Y[i], _grid.GetAreaNumber(j, i));

				if (isPrint)
					for (int i = 0; i < solution.Length; i++)
						Console.WriteLine("u{0} = {1:E16} | u*{0} = {2:E16} | {3:E16}", i + 1, solution[i], exactSolution[i], Math.Abs(exactSolution[i] - solution[i]));

				Console.WriteLine("Относительная погрешность: " + _resultProducer.GetSolveDifference(0));
			}
			//else
			//{
			//	for (int i = 0; i < _grid.M; i++)
			//		for (int j = 0; j < _grid.N; j++)
			//			exactSolution.Elements[i * _grid.N + j] = Master.Func2(_grid.X[j], _grid.y[i], _grid.T[timeLayer], _grid.GetAreaNumber(j, i));

			//	if (isPrint)
			//		for (int i = 0; i < p.Length; i++)
			//			Console.WriteLine("u{0} = {1:E16} | u*{0} = {2:E16} | {3:E16}", i + 1, q[timeLayer].Elements[i], exactSolution.Elements[i], Math.Abs(exactSolution.Elements[i] - q[timeLayer].Elements[i]));
			//	Console.WriteLine("--------------------------------------------------------------------------------------");
			//	Console.WriteLine("Относительная погрешность: " + GetSolveDifference(q[timeLayer], exactSolution));
			//}

		}

		private void WriteSolve(string path, Vector solve)
		{
			using (StreamWriter sw = new StreamWriter(path))
				for (int j = 0; j < _grid.M; j++)
					for (int i = 0; i < _grid.N; i++)
						sw.WriteLine(_grid.X[i].ToString().Replace(",", ".") + " " + _grid.Y[j].ToString().Replace(",", ".") +
							 " " + solve[j * _grid.N + i].ToString().Replace(",", "."));

			//using (StreamWriter sw = new StreamWriter(@"output\temperature.txt"))
			//{
			//	sw.WriteLine(_grid.Y);
			//	sw.WriteLine(_grid.TimeLayersCount);
			//	for (int k = 0; k < _grid.TimeLayersCount; k++)
			//		for (int j = 0; j < _grid.M; j++)
			//			for (int i = 0; i < _grid.N; i++)
			//				sw.WriteLine(_grid.X[i].ToString().Replace(",", ".") + " " + _grid.Y[j].ToString().Replace(",", ".") +
			//					  " " + q[k].Elements[j * _grid.N + i].ToString().Replace(",", "."));
			//}
		}
		//private void WriteGrid()
		//{
		//	using (StreamWriter sw = new StreamWriter(@"output\grid.txt"))
		//	{
		//		//	sw.WriteLine(_boreholes.Length);
		//		//for (int i = 0; i < _boreholes.Length; i++)
		//		//	{
		//		//		sw.WriteLine("{0} {1} {2} {3}", _x[_boreholes[i][0]].ToString().Replace(",", "."), _x[_boreholes[i][0] + 1].ToString().Replace(",", "."),
		//		//		_y[_boreholes[i][1]].ToString().Replace(",", "."), _y[_boreholes[i][1] + 1].ToString().Replace(",", "."));
		//		//	}
		//		sw.WriteLine("0 0 0 0");
		//		sw.WriteLine("{0} {1} {2} {3}", _grid.X[0].ToString().Replace(",", "."), _grid.X[_grid.X.Count() - 1].ToString().Replace(",", "."),
		//				 _grid.Y[0].ToString().Replace(",", "."), _grid.Y[_grid.Y.Count() - 1].ToString().Replace(",", "."));

		//		sw.WriteLine(_x.Count());
		//		sw.WriteLine(_y.Count());
		//		//  sw.WriteLine("Hello World!!");
		//		for (int i = 0; i < _x.Count(); i++)
		//		{
		//			sw.WriteLine(_x[i].ToString().Replace(",", "."));
		//		}
		//		for (int i = 0; i < _y.Count(); i++)
		//		{
		//			sw.WriteLine(_y[i].ToString().Replace(",", "."));
		//		}
		//		sw.WriteLine(_areas.Length);
		//		foreach (var area in _areas)
		//			sw.WriteLine("{0} {1} {2} {3}", _x[_IX[area[0]]], _x[_IX[area[1]]], _y[_IY[area[2]]], _y[_IY[area[3]]]);
		//		sw.Close();
		//	}
		//}
	}
}
