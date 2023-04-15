using ResearchPaper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReaserchPaper
{
    internal class FEM
    {
        static double Y1(double y, double yUpper, double hy) => (yUpper - y) / hy;
        static double Y2(double y, double yLower, double hy) => (y - yLower) / hy;
        static double X1(double x, double xRight, double hx) => (xRight - x) / hx;
        static double X2(double x, double xLeft, double hx) => (x - xLeft) / hx;



        public static Point GetV(int elemNumber, double xLeft, double xRight, double yLower, double yUpper, double x, double y, double hx, double hy)
        {
            double _x, _y;
            _x = -Master.Slau.p.Elements[elemNumber] * Y1(y, yUpper, hy) / hx + Master.Slau.p.Elements[elemNumber + 1] * Y1(y, yUpper, hy) / hx
            - Master.Slau.p.Elements[elemNumber + Grid.N] * Y2(y, yLower, hy) / hx + Master.Slau.p.Elements[elemNumber + Grid.N + 1] * Y2(y, yLower, hy) / hx;
            _y = -Master.Slau.p.Elements[elemNumber] * X1(x, xRight, hx) / hy - Master.Slau.p.Elements[elemNumber + 1] * X2(x, xLeft, hx) / hy
            + Master.Slau.p.Elements[elemNumber + Grid.N] * X1(x, xRight, hx) / hy + Master.Slau.p.Elements[elemNumber + Grid.N + 1] * X2(x, xLeft, hx) / hy;
            return new Point(-_x, -_y);
        }

        public static double VGradP(int elemNumber, int i, int j, double xLeft, double xRight, double yLower, double yUpper, double x, double y, double hx, double hy)
        {
            double result = 0;
            Point V = GetV(elemNumber, xLeft, xRight, yLower, yUpper, x, y, hx, hy);
            switch (j)
            {
                case 0:
                    result += (-V.x * Y1(y, yUpper, hy) / hx - V.y * X1(x, xRight, hx) / hy);
                    break;
                case 1:
                    result += (V.x * Y1(y, yUpper, hy) / hx - V.y * X2(x, xLeft, hx) / hy);
                    break;
                case 2:
                    result += (-V.x * Y2(y, yLower, hy) / hx + V.y * X1(x, xRight, hx) / hy);
                    break;
                case 3:
                    result += (V.x * Y2(y, yLower, hy) / hx + V.y * X2(x, xLeft, hx) / hy);
                    break;
            }
            switch (i)
            {
                case 0:
                    result *= Y1(y, yUpper, hy) * X1(x, xRight, hx);
                    break;
                case 1:
                    result *= Y1(y, yUpper, hy) * X2(x, xLeft, hx);
                    break;
                case 2:
                    result *= Y2(y, yLower, hy) * X1(x, xRight, hx);
                    break;
                case 3:
                    result *= Y2(y, yLower, hy) * X2(x, xLeft, hx);
                    break;
            }
            return result;
        }
    }
}
