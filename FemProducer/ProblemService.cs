using FemProducer.Models;

using Grid.Models;

namespace FemProducer
{
	public class ProblemService
	{
		private readonly ProblemParametrs _problemParametrs;

		public double Lamda(int area)
		{
			switch (area)
			{
				case 0: return 1;
				case 1: return 1;
				default: throw new ArgumentException("Попытка обратится к подобласти, которой не существует!");
			}
		}
		public double Gamma(int area)
		{
			switch (area)
			{
				case 0: return 1;
				case 1: return 1;
				default: throw new ArgumentException("Попытка обратится к подобласти, которой не существует!");
			}
		}

		public double Sigma(int area)
		{
			switch (area)
			{
				//    case 0: return 1;
				//  case 1: return 2;
				default: return 1;
			}
		}

		public double Function(Node node)
		{
			var x = node.X;
			var y = node.Y;

			return x + y;
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

		public double F1(Node node, int formulaIndex)
		{
			var x = node.X;
			var y = node.Y;

			return formulaIndex switch
			{
				0 => Function(node) - 1 / x,
				1 => Function(node) - 1 / x,
				_ => throw new ArgumentException("Попытка обратится к подобласти, которой не существует!"),
			};
		}


		//public double Func2(double x, double y, double t, int area)
		//{
		//	switch (area)
		//	{
		//		//    case 0: return -X * y * T;
		//		//    case 1: return X * y * T;
		//		default: return x + y + t;
		//	}
		//}
		//public double DivFuncX2(double x, double y, double t, int area)
		//{
		//	switch (area)
		//	{
		//		default:
		//			return y * t;
		//	}
		//}
		//public double DivFuncY2(double x, double y, double t, int area)
		//{
		//	switch (area)
		//	{
		//		default:
		//			return x * t;
		//	}
		//}
		//public double F2(double x, double y, double t, int area)
		//{
		//	switch (area)
		//	{
		//		//    case 0: return -1;
		//		// case 1: return 0;
		//		//       default: return 1 -6 * X - 6 * y  -9* X * X * X*X -9*y* y * y * y;
		//		default: return -1;
		//	}
		//}
	}
}
