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
				//    case 0: return 1;
				//   case 1: return 10;
				default: return 1;
			}
		}
		public double Gamma(int area)
		{
			switch (area)
			{
				// case 0: return 1;
				// case 1: return 2;
				default: return 5;
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

		public int[] boundaryConditions = new int[4] { 1, 1, 1, 1 };


		public double Function(Node node, int nodeIndex)
		{
			var x = node.X;
			var y = node.Y;

			return nodeIndex switch
			{
				//  case 0: return Math.Pow(Math.E,Math.PI*y)*Math.Sin(Math.PI*X);
				//  case 1: return Math.Pow(Math.E, Math.PI * y) * Math.Sin(Math.PI * X)/10;
				_ => x + y
				//default: return X * X * X + y * y * y;
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
		public double F1(double x, double y, int area)
		{
			//switch (area)
			//{
			//      case 0: return X + y;
			//        case 1: return 2 * X + 2 * y;
			/*default:*/
			return 5 * (x + y);
			//}
		}
		public double F1(Node node)
		{
			//switch (area)
			//{
			//      case 0: return X + y;
			//        case 1: return 2 * X + 2 * y;
			/*default:*/
			return 5 * (node.X + node.Y);
			//}
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
