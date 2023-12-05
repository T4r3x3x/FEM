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
		public static void MultiplyLocalMatrix(double[][] localMatrix, double coefficient)
		{
			for (int p = 0; p < 4; p++)
				for (int k = 0; k < 4; k++)
					localMatrix[p][k] *= coefficient;
		}
	}
}
