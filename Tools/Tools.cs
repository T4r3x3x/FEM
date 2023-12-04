namespace FemProducer
{
	public static class Tools
	{
		public static int BinarySearch(IList<int> list, int value, int l, int r)
		{
			while (l != r)
			{
				int mid = (l + r) / 2 + 1;

				if (list[mid] > value)
					r = mid - 1;
				else
					l = mid;
			}

			return list[l] == value ? l : -1;
		}
		/// <summary>
		/// Умножает элементы матрицы на заданный коэффициент.
		/// </summary>
		/// <param name="localMatrix"></param>
		/// <param name="coefficient"></param>
		public static double[][] MultiplyLocalMatrix(double[][] localMatrix, double coefficient)
		{
			var result = new double[localMatrix.Length][];
			for (int p = 0; p < 4; p++)
			{
				result[p] = new double[localMatrix.Length];
				for (int k = 0; k < 4; k++)
					result[p][k] = localMatrix[p][k] * coefficient;
			}
			return result;
		}
	}
}
