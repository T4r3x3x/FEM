namespace Tools
{
	public static class Extensions
	{
		/// <summary>
		/// Умножает элементы матрицы на заданный коэффициент.
		/// </summary>
		/// <param name="localMatrix"></param>
		/// <param name="coefficient"></param>
		public static void MultiplyLocalMatrix(this double[][] localMatrix, double coefficient)
		{
			for (int p = 0; p < 4; p++)
				for (int k = 0; k < 4; k++)
					localMatrix[p][k] *= coefficient;
		}
	}
}
