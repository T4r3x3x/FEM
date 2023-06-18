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

        static double[,] G = new double[,]
        {
            {1,-1 },
            {-1,1 },
        };
        static double[,] M = new double[,]
        {
             { 2, 1 },
            { 1, 2 }
        };

        static int mu(int i) => ((i) % 2);
        static int nu(int i) => ((i) / 2);

        public static double[][] GetMassMatrix(double hx, double hy)// Grid.M - номер кэ 
        {
            // инициализация
            double[][] result = new double[M.LongLength][];
            for (int i = 0; i < result.Length; i++)
                result[i] = new double[result.Length];

            //матрица масс
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result.Length; j++)
                    result[i][j] += M[mu(i),mu(j)]*hx / 6 * M[nu(i),nu(j)] * hy / 6;

            return result;
        }
        public static double[][] GetStiffnessMatrix(double hx, double hy)// Grid.M - номер кэ 
        {
            // инициализация
            double[][] result = new double[G.LongLength][];
            for (int i = 0; i < result.Length; i++)
                result[i] = new double[result.Length];

            //матрица жесткости
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result.Length; j++)
                    result[i][j] = G[mu(i),mu(j)]/hx * M[nu(i),nu(j)]*hy/6 + M[mu(i),mu(j)] * hx / 6 * G[nu(i),nu(j)] / hy;

            return result;
        }

        //static double SolutionInPoint(int elemNumber, Point xBoundaries, Point yBoundaries, Point point, double hx, double hy)
        //{
        //    double result = 0;
        //    Master.Slau.p.Elements[elemNumber]*X1()* Y1(point.y, yBoundaries.y, hy)

        //    return result
        //}


        public static Point GetV(int elemNumber, Point xBoundaries, Point yBoundaries, Point point, double hx, double hy)
        {
            double _x, _y;
            _x = -Master.Slau.p.Elements[elemNumber] * Y1(point.y, yBoundaries.y, hy) / hx + Master.Slau.p.Elements[elemNumber + 1] * Y1(point.y, yBoundaries.y, hy) / hx
            - Master.Slau.p.Elements[elemNumber + Grid.N] * Y2(point.y, yBoundaries.x, hy) / hx + Master.Slau.p.Elements[elemNumber + Grid.N + 1] * Y2(point.y, yBoundaries.x, hy) / hx;
            _y = -Master.Slau.p.Elements[elemNumber] * X1(point.x, xBoundaries.y, hx) / hy - Master.Slau.p.Elements[elemNumber + 1] * X2(point.x, xBoundaries.x, hx) / hy
            + Master.Slau.p.Elements[elemNumber + Grid.N] * X1(point.x, xBoundaries.y, hx) / hy + Master.Slau.p.Elements[elemNumber + Grid.N + 1] * X2(point.x, xBoundaries.x, hx) / hy;
            return new Point(-_x, -_y);
        }

        public static double VGradP(int elemNumber, int i, int j, Point xBoundaries, Point yBoundaries, Point point, double hx, double hy)
        {
            double result = 0;
            Point V = GetV(elemNumber, xBoundaries,yBoundaries, point, hx, hy);
           // V.Print();
            switch (j)
            {
                case 0:
                    result += (-V.x * Y1(point.y, yBoundaries.y, hy) / hx - V.y * X1(point.x, xBoundaries.y, hx) / hy);
                    break;
                case 1:
                    result += (V.x * Y1(point.y, yBoundaries.y, hy) / hx - V.y * X2(point.x, xBoundaries.x, hx) / hy);
                    break;
                case 2:
                    result += (-V.x * Y2(point.y, yBoundaries.x, hy) / hx + V.y * X1(point.x, xBoundaries.y, hx) / hy);
                    break;
                case 3:
                    result += (V.x * Y2(point.y, yBoundaries.x, hy) / hx + V.y * X2(point.x, xBoundaries.x, hx) / hy);
                    break;
            }
            switch (i)
            {
                case 0:
                    result *= Y1(point.y, yBoundaries.y, hy) * X1(point.x, xBoundaries.y, hx);
                    break;
                case 1:
                    result *= Y1(point.y, yBoundaries.y, hy) * X2(point.x, xBoundaries.x, hx);
                    break;
                case 2:
                    result *= Y2(point.y, yBoundaries.x, hy) * X1(point.x, xBoundaries.y, hx);
                    break;
                case 3:
                    result *= Y2(point.y, yBoundaries.x, hy) * X2(point.x, xBoundaries.x, hx);
                    break;
            }
            return result;
        }
    }
}
