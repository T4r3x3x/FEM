namespace FemProducer.Basises.Helpers.BasisFunctions
{
    public static class LinearBasisFunctions
    {
        public const int NodesCount = 4;

        public static List<Func<double, double, double, double>> s_Functions = [

            (X, xRight, hx) => (xRight - X) / hx,
            (X, xLeft, hx) => (X - xLeft) / hx,
        ];

        public static List<Func<double, double>> s_FunctionDerivative = [

            (h) => -1 / h,
            (h) => 1 / h,
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