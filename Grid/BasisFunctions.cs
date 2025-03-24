using Grid.Models;

using MathModels;
using MathModels.Models;

namespace Grid
{
    public class BasisFunctions
    {
        static double Y1(double y, double yUpper, double hy) => (yUpper - y) / hy;
        static double Y2(double y, double yLower, double hy) => (y - yLower) / hy;
        static double X1(double X, double xRight, double hx) => (xRight - X) / hx;
        static double X2(double X, double xLeft, double hx) => (X - xLeft) / hx;

        public static double Psi(int i, Node node, Point xLimits, Point yLimits)
        {
            double hx = xLimits.Y - xLimits.X;
            double hy = yLimits.Y - yLimits.X;
            return i switch
            {
                0 => X1(node.X, xLimits.Y, hx) * Y1(node.Y, yLimits.Y, hy),
                1 => X2(node.X, xLimits.X, hx) * Y1(node.Y, yLimits.Y, hy),
                2 => X1(node.X, xLimits.Y, hx) * Y2(node.Y, yLimits.X, hy),
                3 => X2(node.X, xLimits.X, hx) * Y2(node.Y, yLimits.X, hy),
                _ => throw new ArgumentException($"Wrong index -- {i}"),
            };
        }
    }
}
