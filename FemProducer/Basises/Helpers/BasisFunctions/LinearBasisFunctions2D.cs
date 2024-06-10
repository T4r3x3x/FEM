using Grid.Models;

namespace FemProducer.Basises.Helpers.BasisFunctions
{
    public static class LinearBasisFunctions2D
    {
        private const int NodesCount = 4;

        private static List<Func<double, double, double, double>> Functions = LinearBasisFunctions.s_Functions;
        private static List<Func<double, double>> FunctionDerivative = LinearBasisFunctions.s_FunctionDerivative;

        public static double XDerivativeBasisFunction(int functionIndex, Node node, Node xLimits, Node yLimits)
        {
            double hy = yLimits.Y - yLimits.X;
            double hx = xLimits.Y - xLimits.X;

            return functionIndex switch
            {
                0 => FunctionDerivative[0](hx) * Functions[0](node.Y, yLimits.Y, hy),
                1 => FunctionDerivative[1](hx) * Functions[0](node.Y, yLimits.Y, hy),
                2 => FunctionDerivative[0](hx) * Functions[1](node.Y, yLimits.X, hy),
                3 => FunctionDerivative[1](hx) * Functions[1](node.Y, yLimits.X, hy),
                _ => throw new ArgumentException($"The argument i = {functionIndex} is an invalid value."),
            };
        }

        public static double YDerivativeBasisFunction(int functionIndex, Node node, Node xLimits, Node yLimits)
        {
            double hx = xLimits.Y - xLimits.X;
            double hy = yLimits.Y - yLimits.X;

            return functionIndex switch
            {
                0 => Functions[0](node.X, xLimits.Y, hx) * FunctionDerivative[0](hy),
                1 => Functions[1](node.X, xLimits.X, hx) * FunctionDerivative[0](hy),
                2 => Functions[0](node.X, xLimits.Y, hx) * FunctionDerivative[1](hy),
                3 => Functions[1](node.X, xLimits.X, hx) * FunctionDerivative[1](hy),
                _ => throw new ArgumentException($"The argument i = {functionIndex} is an invalid value."),
            };
        }

        public static double GetBasisFunctionValue(int functionIndex, Node node, Node xLimits, Node yLimits)
        {
            double hx = xLimits.Y - xLimits.X;
            double hy = yLimits.Y - yLimits.X;

            return functionIndex switch
            {
                0 => Functions[0](node.X, xLimits.Y, hx) * Functions[0](node.Y, yLimits.Y, hy),
                1 => Functions[1](node.X, xLimits.X, hx) * Functions[0](node.Y, yLimits.Y, hy),
                2 => Functions[0](node.X, xLimits.Y, hx) * Functions[1](node.Y, yLimits.X, hy),
                3 => Functions[1](node.X, xLimits.X, hx) * Functions[1](node.Y, yLimits.X, hy),
                _ => throw new ArgumentException($"The argument i = {functionIndex} is an invalid value."),
            };
        }

        public static IList<double> GetLocalVector(IList<Node> nodes, Func<Node, int, double> func, int formulaNumber)
        {
            double[] result = new double[NodesCount];

            var hx = nodes[1].X - nodes[0].X;
            var hy = nodes[2].Y - nodes[0].Y;

            var funcValues = new double[NodesCount];
            for (int i = 0; i < NodesCount; i++)
                funcValues[i] = func(nodes[i], formulaNumber);

            result[0] = hx * hy / 36 * (4 * funcValues[0] + 2 * funcValues[1] + 2 * funcValues[2] + funcValues[3]);
            result[1] = hx * hy / 36 * (2 * funcValues[0] + 4 * funcValues[1] + funcValues[2] + 2 * funcValues[3]);
            result[2] = hx * hy / 36 * (2 * funcValues[0] + funcValues[1] + 4 * funcValues[2] + 2 * funcValues[3]);
            result[3] = hx * hy / 36 * (funcValues[0] + 2 * funcValues[1] + 2 * funcValues[2] + 4 * funcValues[3]);

            return result;
        }
    }
}
