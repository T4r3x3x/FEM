using FemProducer.Models;

using Grid.Models;

namespace FemProducer
{
	public class ProblemService
	{
		private readonly ProblemParameters _problemParameters;

		public ProblemService(ProblemParameters problemParameters) => _problemParameters = problemParameters;

		public double Lambda(int formulaNumber)
		{
			if (formulaNumber > _problemParameters.Lambda.Count - 1)
				throw new ArgumentException($"Лямбда для формулы {formulaNumber} не задана!");

			return _problemParameters.Lambda[formulaNumber];
		}

		public double Gamma(int formulaNumber)
		{
			if (formulaNumber > _problemParameters.Gamma.Count - 1)
				throw new ArgumentException($"Гамма для формулы {formulaNumber} не задана!");

			return _problemParameters.Gamma[formulaNumber];
		}

		public double Function(Node node, int area)
		{
			var x = node.X;
			var y = node.Y;

			return area switch
			{
				0 => 25,
				1 => 450,
				_ => 25,
				//_ => throw new ArgumentException(),
			};
		}

		public double DivFuncX1(double x, double y, int area)
		{
			switch (area)
			{
				default: return 0;
			}
		}
		public double DivFuncY1(double x, double y, int area)
		{
			switch (area)
			{
				default: return 0;
			}
		}

		public double FBetta(Node node, int formulaIndex)
		{
			var x = node.X;
			var y = node.Y;

			return formulaIndex switch
			{
				_ => 25,
				//_ => throw new ArgumentException(),
			};
		}

		public double F(Node node, int formulaIndex)
		{
			var x = node.X;
			var y = node.Y;

			return formulaIndex switch
			{
				_ => 0,
				//_ => throw new ArgumentException(),
			};
		}
	}
}
