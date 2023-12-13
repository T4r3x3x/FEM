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
			for (int p = 0; p < localMatrix.Length; p++)
				for (int k = 0; k < localMatrix.Length; k++)
					localMatrix[p][k] *= coefficient;
		}
	}
}
