namespace Grid.Implementations
{
	internal static class BaseMethods
	{
		internal static double[] GetPointsInAxis(double[] q, double[] W, IList<int> splitsCount)
		{
			var points = new List<double>();

			//points.Add(W[0]);

			for (int i = 0; i < q.Length; i++)
			{
				double firstStep = GetStep(q[i], W[i], W[i + 1], splitsCount[i]);
				points.AddRange(GetPointsInInterval(firstStep, q[i], W[i], splitsCount[i]));
			}
			points.Add(W[W.Length - 1]);

			return points.ToArray();
		}

		private static double GetStep(double q, double W0, double W1, int splitCount)
		{
			return q == 1 ? (W1 - W0) / (splitCount - 1) :
				(W1 - W0) * (q - 1) / (Math.Pow(q, splitCount - 1) - 1);
		}


		private static double[] GetPointsInInterval(double firstStep, double q, double startPoint, int pointsCount)//режим область на части и записываем в массив, _h - шаг,  j - номер подобласти
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
	}
}
