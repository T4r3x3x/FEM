using Grid.Models;
using Grid.Models.InputModels;

using MathModels;

using Tools;

namespace Grid.Factories
{
    internal static class AxisFactory
    {
        internal static (SpatialCoordinates coordinates, double[] t) GetAxisesToPoints(GridInputParameters @params)
        {
            double[] x, y = null!, z = null!;
            (var xW, var yW) = GetSegmentsBoundaries(@params.LinesNodes);

            x = AxisToPoints(@params.XParams.Q, xW, @params.XParams.SplitsCount);

            if (@params.GridDimensional != Enum.GridDimensional.One)
                y = AxisToPoints(@params.YParams.Q, yW, @params.YParams.SplitsCount);
            if (@params.GridDimensional == Enum.GridDimensional.Three)
                z = AxisToPoints(@params.ZParams.Q, @params.ZW, @params.ZParams.SplitsCount);

            var t = AxisToPoints(@params.TParams.Q, @params.TW, @params.TParams.SplitsCount);
            return (new(x, y, z), t);
        }

        #region Segments
        private static (double[], double[]) GetSegmentsBoundaries(Point[][] lines) => (GetXSegmentBoundaries(lines), GetYSegmentBoundaries(lines));

        private static double[] GetYSegmentBoundaries(Point[][] lines)
        {
            HashSet<double> yw = [SearchingAlghoritms.GetMinValueInCollection(lines[0].Select(point => point.Y))];
            for (int i = 1; i < lines.Length - 1; i++)
                yw.Add(lines[i][0].Y);
            yw.Add(SearchingAlghoritms.GetMaxValueInCollection(lines[lines.Length - 1].Select(point => point.Y)));
            return yw.ToArray();
        }

        private static double[] GetXSegmentBoundaries(Point[][] lines)
        {
            HashSet<double> xw = new();
            var minX = SearchingAlghoritms.GetMinValueInCollection(lines[0].Select(x => x.X));
            xw.Add(minX);

            for (int i = 1; i < lines[0].Length - 1; i++)
                xw.Add(lines[0][i].X);

            var maxX = SearchingAlghoritms.GetMaxValueInCollection(lines[lines.Length - 1].Select(x => x.X));
            xw.Add(maxX);
            return xw.ToArray();
        }
        #endregion

        #region Axis
        internal static double[] AxisToPoints(double[] q, double[] W, IList<int> splitsCount)
        {
            var points = new List<double>();
            for (int i = 0; i < q.Length; i++)
            {
                double firstStep = GetStep(q[i], W[i], W[i + 1], splitsCount[i]);
                var pointsInInterval = GetPointsInInterval(firstStep, q[i], W[i], splitsCount[i]);
                points.AddRange(pointsInInterval);
            }
            points.Add(W[W.Length - 1]);
            return points.ToArray();
        }

        private static double GetStep(double q, double leftBoundary, double rightBoundary, int splitsCount) => q == 1 ?
                GetEvenStep(leftBoundary, rightBoundary, splitsCount) :
                GetUnevenStep(q, leftBoundary, rightBoundary, splitsCount);

        private static double GetEvenStep(double leftBoundary, double rightBoundary, int splitsCount) => (rightBoundary - leftBoundary) / (splitsCount - 1);
        private static double GetUnevenStep(double q, double leftBoundary, double rightBoundary, int splitsCount) => (rightBoundary - leftBoundary) * (q - 1) / (Math.Pow(q, splitsCount - 1) - 1);

        /// <summary>
        /// Режим данный интервал на точки
        /// </summary>
        /// <param name="firstStep"></param>
        /// <param name="q"></param>
        /// <param name="startPoint"></param>
        /// <param name="pointsCount"></param>
        /// <returns></returns>
        private static double[] GetPointsInInterval(double firstStep, double q, double startPoint, int pointsCount)
        {
            pointsCount--;
            var points = new double[pointsCount];
            var currentStep = firstStep;
            points[0] = startPoint;

            for (int i = 0; i < pointsCount - 1; i++)
            {
                points[i + 1] = points[i] + currentStep;
                currentStep *= q;
            }
            return points;
        }
        #endregion
    }
}