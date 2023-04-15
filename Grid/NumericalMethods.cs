using ResearchPaper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace ReaserchPaper
{
    delegate double Function(Point point);

    internal class NumericalMethods
    {
        public static double FiniteDifferentiation(Point point, Function function, double h)
        {
            double result = 0;
            result = 1 / 12.0 / h * (function(point));
            return result;
        }
        public static double GaussIntegration(int elemNumber, int i, int j, double xLeft, double xRight, double yLower, double yUpper, double hx, double hy)
        {
            double result = 0;

            double[] q = new double[4] { (18 - Math.Sqrt(30)) / 36, (18 + Math.Sqrt(30)) / 36, (18 + Math.Sqrt(30)) / 36, (18 - Math.Sqrt(30)) / 36 };
            double[] x = new double[4] { -Math.Sqrt((15 + 2 * Math.Sqrt(30)) / 35), -Math.Sqrt((15 - 2 * Math.Sqrt(30)) / 35), Math.Sqrt((15 - 2 * Math.Sqrt(30)) / 35), Math.Sqrt((15 + 2 * Math.Sqrt(30)) / 35) };

            for (int l = 0; l < 4; l++)
            {
                double temp = 0;
                for (int r = 0; r < 4; r++)
                {
                    double u = (yLower + yUpper + hy * x[l]) / 2.0; // y
                    double v = (xLeft + xRight + hx * x[r]) / 2.0; // x
                    temp += q[r] * FEM.VGradP(elemNumber, i, j, xLeft, xRight, yLower, yUpper, v, u, hx, hy);
                }
                result += q[l] * temp;
            }

            result *= hx * hy / 4.0;

            return result;
        }
    }
}
