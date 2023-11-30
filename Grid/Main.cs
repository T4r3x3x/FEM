using System.Diagnostics;

using ReaserchPaper;
using ReaserchPaper.Assemblier;
using ReaserchPaper.Grid;

namespace ResearchPaper
{
	class Master
	{
		static double ro, fita, K, nu, mu;

		public static double Lamda(int area)
		{
			switch (area)
			{
				//    case 0: return 1;
				//   case 1: return 10;
				default: return 1;
			}
		}
		public static double Gamma(int area)
		{
			switch (area)
			{
				// case 0: return 1;
				// case 1: return 2;
				default: return 5;
			}
		}

		public static double Sigma(int area)
		{
			switch (area)
			{
				//    case 0: return 1;
				//  case 1: return 2;
				default: return 1;
			}
		}
		public static Slae Slau;
		public static int[] boundaryConditions = new int[4] { 1, 1, 1, 1 };


		public static double Func1(double X, double y, int area)
		{
			switch (area)
			{
				//  case 0: return Math.Pow(Math.E,Math.PI*y)*Math.Sin(Math.PI*X);
				//  case 1: return Math.Pow(Math.E, Math.PI * y) * Math.Sin(Math.PI * X)/10;
				default: return X + y;
					//default: return X * X * X + y * y * y;
			}
		}

		public static double DivFuncX1(double X, double y, int area)
		{
			switch (area)
			{
				default: return 0;
			}
		}
		public static double DivFuncY1(double X, double y, int area)
		{
			switch (area)
			{
				default: return 0;
			}
		}
		public static double F1(double X, double y, int area)
		{
			switch (area)
			{
				//      case 0: return X + y;
				//        case 1: return 2 * X + 2 * y;
				default: return 5 * (X + y);
			}
		}


		public static double Func2(double X, double y, double T, int area)
		{
			switch (area)
			{
				//    case 0: return -X * y * T;
				//    case 1: return X * y * T;
				default: return X + y + T;
			}
		}
		public static double DivFuncX2(double X, double y, double T, int area)
		{
			switch (area)
			{
				default:
					return y * T;
			}
		}
		public static double DivFuncY2(double X, double y, double T, int area)
		{
			switch (area)
			{
				default:
					return X * T;
			}
		}
		public static double F2(double X, double y, double T, int area)
		{
			switch (area)
			{
				//    case 0: return -1;
				// case 1: return 0;
				//       default: return 1 -6 * X - 6 * y  -9* X * X * X*X -9*y* y * y * y;
				default: return -1;
			}
		}


		static void ExecuteCommand(string command)
		{
			ProcessStartInfo start = new ProcessStartInfo();
			start.FileName = "C:\\Python\\python.exe";
			start.Arguments = string.Format("C:\\Users\\hardb\\source\\repos\\Grid\\Grid\\bin\\Debug\\net6.0\\grid.py");
			start.UseShellExecute = false;
			start.RedirectStandardOutput = true;
			Process.Start(start);
			start.Arguments = string.Format("C:\\Users\\hardb\\source\\repos\\Grid\\Grid\\bin\\Debug\\net6.0\\pressure.py");
			Process.Start(start);
			start.Arguments = string.Format("C:\\Users\\hardb\\source\\repos\\Grid\\Grid\\bin\\Debug\\net6.0\\temperature.py");
			Process.Start(start);
		}

		static void Main(string[] args)
		{
			System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
			sw.Start();
			LosLU solver = new();
			Grid.ReadData();
			//  Grid.PrintPartialGrid();
			//   Grid.PrintTimeGrid();
			Grid.WriteGrid();
			Slau = new Slae(Grid.NodesCount, Grid.TimeLayersCount);
			Assemblier collector = new(Grid.NodesCount);
			collector.Collect();
			Slau.p = solver.Solve(Slau.A, Slau.b);
			//  Master.Slau.Print();

			Slau.PrintResult(-1, false);
			collector.GetMatrixH();
			collector.SwitchTask(solver);

			for (int i = 2; i < Grid.TimeLayersCount; i++)
			{
				collector.Collect(i);
				//  ;
				Slau.q[i] = solver.Solve(Slau.A, Slau.b);
				//Console.WriteLine("solving in proccess: {0} of {1} time layers...", i+1, Grid.TimeLayersCount);
				Slau.PrintResult(i, false);
			}
			//     Slau.PrintResult(Grid.TimeLayersCount - 1, true);
			sw.Stop();
			Slau.WriteSolves();
			Console.WriteLine(sw.ElapsedMilliseconds);
			ExecuteCommand("python func.py");
		}
	}
}