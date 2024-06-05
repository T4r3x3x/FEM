using Grid.Models;

using System.ComponentModel;

namespace NumericsMethods
{
    public static class Integration
    {
        public static double GaussIntegration(Node limits, Func<double, double> func, PointsCount pointsCount)
        {
            double result, temp = 0;
            var hx = limits.Y - limits.X;
            GetCoeffs(pointsCount, out var q, out var x);

            for (int r = 0; r < q.Length; r++)
            {
                double u = (limits.X + limits.Y + hx * x[r]) / 2.0;
                temp += q[r] * func(u);
            }
            result = hx * temp / 2.0;

            return result;
        }

        //можно просто лимиты по х и у передавать
        public static double GaussIntegration(Node leftLowerPoint, Node rightUpperPoint, Func<double, double, double> func, PointsCount pointsCount)
        {
            double result = 0, temp;
            var hx = rightUpperPoint.X - leftLowerPoint.X;
            var hy = rightUpperPoint.Y - leftLowerPoint.Y;

            GetCoeffs(pointsCount, out var q, out var x);

            for (int l = 0; l < q.Length; l++)
            {
                temp = 0;
                for (int r = 0; r < q.Length; r++)
                {
                    double u = (leftLowerPoint.X + rightUpperPoint.X + hx * x[r]) / 2.0;
                    double v = (leftLowerPoint.Y + rightUpperPoint.Y + hy * x[l]) / 2.0;
                    temp += q[r] * func(u, v);
                }
                result += q[l] * temp;
            }
            result *= hx * hy / 4.0;

            return result;
        }

        public static double GaussIntegration(Node x1y1z1, Node x2y2z1, Node x2y2z2, Func<double, double, double, double> func, PointsCount pointsCount)
        {
            double result = 0, temp1, temp2;
            var hx = x2y2z1.X - x1y1z1.X;
            var hy = x2y2z1.Y - x1y1z1.Y;
            var hz = x2y2z2.Z - x2y2z1.Z;
            GetCoeffs(pointsCount, out var q, out var x);

            for (int l = 0; l < q.Length; l++)
            {
                temp1 = 0;
                for (int r = 0; r < q.Length; r++)
                {
                    temp2 = 0;
                    for (int k = 0; k < q.Length; k++)
                    {
                        double u = (x1y1z1.X + x2y2z1.X + hx * x[r]) / 2.0;
                        double v = (x1y1z1.Y + x2y2z1.Y + hy * x[l]) / 2.0;
                        double m = (x2y2z1.Z + x2y2z2.Z + hz * x[k]) / 2.0;
                        temp2 += q[k] * func(u, v, m);
                    }
                    temp1 += q[r] * temp2;
                }
                result += q[l] * temp1;
            }
            result *= hx * hy * hz / 8.0;

            return result;
        }

        private static void GetCoeffs(PointsCount pointsCount, out double[] q, out double[] x)
        {
            switch (pointsCount)
            {
                case PointsCount.Two:
                    q = [1, 1];
                    x = [-0.5773502692, 0.5773502692];
                    break;

                case PointsCount.Three:
                    q = [5 / 9.0, 8 / 9.0, 5 / 9.0];
                    x = [-0.77459666924148337, 0, 0.77459666924148337];
                    break;

                case PointsCount.Four:
                    q = [(18 - Math.Sqrt(30)) / 36, (18 + Math.Sqrt(30)) / 36, (18 + Math.Sqrt(30)) / 36, (18 - Math.Sqrt(30)) / 36];
                    x = [-Math.Sqrt((15 + 2 * Math.Sqrt(30)) / 35), -Math.Sqrt((15 - 2 * Math.Sqrt(30)) / 35), Math.Sqrt((15 - 2 * Math.Sqrt(30)) / 35), Math.Sqrt((15 + 2 * Math.Sqrt(30)) / 35)];
                    break;

                default:
                    throw new InvalidEnumArgumentException();
            }
        }

        public enum PointsCount
        {
            Two = 2,
            Three = 3,
            Four = 4,
        }
    }
}
