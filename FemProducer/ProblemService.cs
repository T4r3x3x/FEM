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

		public double Function(Node node)
		{
			var x = node.X;
			var y = node.Y;

			return (x + y);
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

		public double F(Node node, int formulaIndex)
		{
			var x = node.X;
			var y = node.Y;

			return formulaIndex switch
			{
				0 => Gamma(formulaIndex) * Function(node),
				1 => Gamma(formulaIndex) * Function(node),
				_ => throw new ArgumentException("Попытка обратится к подобласти, которой не существует!"),
			};
		}
	}
}
