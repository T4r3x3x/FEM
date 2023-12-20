namespace FemProducer.Basises.BasisFunctions
{
	public static class LinearBasisFunctions
	{
		public const int NodesCount = 4;

		public static List<Func<double, double, double, double>> s_Functions = [

			(double X, double xRight, double hx) => (xRight - X) / hx,
			(double X, double xLeft, double hx) => (X - xLeft) / hx,
		];

		public static List<Func<double, double>> s_FunctionDerivative = [

			(double h) => -1 / h,
			(double h) => 1 / h,
		];

		public static double[,] G = new double[,]
		{
			{1,-1 },
			{-1,1 },
		};

		public static double[,] M = new double[,]
		{
			 { 2, 1 },
			{ 1, 2 }
		};
	}
}