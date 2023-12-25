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

			return formulaIndex switch
			{
				_ => x + z + y,
				//_ => throw new ArgumentException($"Гамма для формулы задана!"),
			};
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
				default: return -Math.Sin(x + y + z);
			}
		}

		public double F(Node node, int formulaIndex)
		{
			var x = node.X;
			var y = node.Y;
			var z = node.Z;

			return formulaIndex switch
			{
				_ => Gamma(formulaIndex) * Function(node, formulaIndex),

				//_ => throw new ArgumentException(),
			};
		}
	}
}
