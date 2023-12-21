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

		public double Function(Node node, int formulaIndex)
		{
			var x = node.X;
			var y = node.Y;
			var z = node.Z;

			return 5 + 0.2 * x + y + 30 * z + 0.5 * x * y + x * z + 10 * y * z + x * y * z;
		}

		public double DivFuncX(Node node, int area)
		{
			switch (area)
			{
				default: return 0;
			}
		}
		public double DivFuncY(Node node, int area)
		{
			switch (area)
			{
				default: return 0;
			}
		}
		public double DivFuncZ(Node node, int area)
		{
			var x = node.X;
			var y = node.Y;
			var z = node.Z;
			switch (area)
			{
				default: return x * y;
			}
		}

		public double F(Node node, int formulaIndex)
		{
			var x = node.X;
			var y = node.Y;
			var z = node.Z;

			return formulaIndex switch
			{
				_ => 0.4 * (5 + 0.2 * x + y + 30 * z + 0.5 * x * y + x * z + 10 * y * z + x * y * z),
				//_ => throw new ArgumentException(),
			};
		}
	}
}
