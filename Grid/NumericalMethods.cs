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
        public static double GaussIntegration(int elemNumber, int i, int j, Point xBoundaries, Point yBoundaries, double hx, double hy)
        {
            double result = 0;
            Point point;
          //  double[] q = new double[2] { 1,1};
           // double[] x = new double[2] { -0.5773502692, 0.5773502692 };
            double[] q = new double[4] { (18 - Math.Sqrt(30)) / 36, (18 + Math.Sqrt(30)) / 36, (18 + Math.Sqrt(30)) / 36, (18 - Math.Sqrt(30)) / 36 };
            double[] x = new double[4] { -Math.Sqrt((15 + 2 * Math.Sqrt(30)) / 35), -Math.Sqrt((15 - 2 * Math.Sqrt(30)) / 35), Math.Sqrt((15 - 2 * Math.Sqrt(30)) / 35), Math.Sqrt((15 + 2 * Math.Sqrt(30)) / 35) };

            for (int l = 0; l < q.Length; l++)
            {
                double temp = 0;
                for (int r = 0; r < q.Length; r++)
                {
                    double u = (xBoundaries.x + xBoundaries.y + hx * x[r]) / 2.0; // x
                    double v = (yBoundaries.x + yBoundaries.y + hy * x[l]) / 2.0; // y                  
                    point = new Point(u, v);
                    temp += q[r] * FEM.VGradP(elemNumber, i, j, xBoundaries,yBoundaries, point, hx, hy);
                }
                result += q[l] * temp;
            }

            result *= hx * hy / 4.0;

            return result;
        }
    }
}
