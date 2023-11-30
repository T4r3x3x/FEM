using ReaserchPaper.Logger;

namespace FemProducer
{
	/// <summary>
	/// Класс отвечает за запись результата работы программы
	/// </summary>
	internal class Outputer
	{
		private readonly ILogger _logger;


		public void PrintTimeGrid()
		{
			Console.WriteLine();
			for (int i = 0; i < T.Length; i++)
			{
				Console.Write("[" + T[i] + "] ");
			}
			Console.WriteLine();
		}
		public void PrintPartialGrid()
		{
			for (int j = 0; j < _m; j++)
			{
				for (int i = 0; i < _n; i++)
				{

					Console.Write(" [{0}, {1}] ", X[i].ToString("e2"), Y[j].ToString("e2"));
				}
				Console.WriteLine("\n");
			}
		}
		public void WriteGrid()
		{
			using (StreamWriter sw = new StreamWriter(@"output\grid.txt"))
			{
				sw.WriteLine(_boreholes.Length);
				for (int i = 0; i < _boreholes.Length; i++)
				{
					sw.WriteLine("{0} {1} {2} {3}", _x[_boreholes[i][0]].ToString().Replace(",", "."), _x[_boreholes[i][0] + 1].ToString().Replace(",", "."),
					_y[_boreholes[i][1]].ToString().Replace(",", "."), _y[_boreholes[i][1] + 1].ToString().Replace(",", "."));
				}

				sw.WriteLine("{0} {1} {2} {3}", _x[0].ToString().Replace(",", "."), _x[_x.Count() - 1].ToString().Replace(",", "."),
						 _y[0].ToString().Replace(",", "."), _y[_y.Count() - 1].ToString().Replace(",", "."));

				sw.WriteLine(_x.Count());
				sw.WriteLine(_y.Count());
				//  sw.WriteLine("Hello World!!");
				for (int i = 0; i < _x.Count(); i++)
				{
					sw.WriteLine(_x[i].ToString().Replace(",", "."));
				}
				for (int i = 0; i < _y.Count(); i++)
				{
					sw.WriteLine(_y[i].ToString().Replace(",", "."));
				}
				sw.WriteLine(_areas.Length);
				foreach (var area in _areas)
					sw.WriteLine("{0} {1} {2} {3}", _x[_IX[area[0]]], _x[_IX[area[1]]], _y[_IY[area[2]]], _y[_IY[area[3]]]);
				sw.Close();
			}
		}
	}
}
