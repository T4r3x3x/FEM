using ResearchPaper;

namespace ReaserchPaper.Logger
{
	internal class ConsoleLogger : ILogger
	{



		public void Log(string message) => throw new NotImplementedException();

		public void PrintResult(int timeLayer, bool isPrint)
		{
			Vector exactSolution = new Vector(q[0].Length);

			if (isPrint)
			{
				Console.WriteLine("      Численное решение      |         точное решение        |      разница решений      ");
				Console.WriteLine("--------------------------------------------------------------------------------------");

			}
			if (timeLayer == -1)
			{
				for (int i = 0; i < Grid.M; i++)
					for (int j = 0; j < Grid.N; j++)
						exactSolution.Elements[i * Grid.N + j] = Master.Func1(Grid.X[j], Grid.y[i], Grid.GetAreaNumber(j, i));

				if (isPrint)
					for (int i = 0; i < p.Length; i++)
						Console.WriteLine("u{0} = {1:E16} | u*{0} = {2:E16} | {3:E16}", i + 1, p.Elements[i], exactSolution.Elements[i], Math.Abs(exactSolution.Elements[i] - p.Elements[i]));

				Console.WriteLine("Относительная погрешность: " + GetSolveDifference(p, exactSolution));
			}
			else
			{
				for (int i = 0; i < Grid.M; i++)
					for (int j = 0; j < Grid.N; j++)
						exactSolution.Elements[i * Grid.N + j] = Master.Func2(Grid.X[j], Grid.y[i], Grid.T[timeLayer], Grid.GetAreaNumber(j, i));

				if (isPrint)
					for (int i = 0; i < p.Length; i++)
						Console.WriteLine("u{0} = {1:E16} | u*{0} = {2:E16} | {3:E16}", i + 1, q[timeLayer].Elements[i], exactSolution.Elements[i], Math.Abs(exactSolution.Elements[i] - q[timeLayer].Elements[i]));
				Console.WriteLine("--------------------------------------------------------------------------------------");
				Console.WriteLine("Относительная погрешность: " + GetSolveDifference(q[timeLayer], exactSolution));
			}

		}


		public void WriteSolves()
		{
			using (StreamWriter sw = new StreamWriter(@"output\pressure.txt"))
				for (int j = 0; j < Grid.M; j++)
					for (int i = 0; i < Grid.N; i++)
						sw.WriteLine(Grid.X[i].ToString().Replace(",", ".") + " " + Grid.y[j].ToString().Replace(",", ".") +
							 " " + p.Elements[j * Grid.N + i].ToString().Replace(",", "."));

			using (StreamWriter sw = new StreamWriter(@"output\temperature.txt"))
			{
				sw.WriteLine(p.Length);
				sw.WriteLine(Grid.TimeLayersCount);
				for (int k = 0; k < Grid.TimeLayersCount; k++)
					for (int j = 0; j < Grid.M; j++)
						for (int i = 0; i < Grid.N; i++)
							sw.WriteLine(Grid.X[i].ToString().Replace(",", ".") + " " + Grid.y[j].ToString().Replace(",", ".") +
								  " " + q[k].Elements[j * Grid.N + i].ToString().Replace(",", "."));
			}
		}
	}
}
