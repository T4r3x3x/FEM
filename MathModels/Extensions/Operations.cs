using MathModels.Models;

namespace MathModels.Extensions
{
	public static class Operations
	{

		public static Vector Add(this Vector a, Vector b)
		{
			if (a.Length != b.Length)
				throw new Exception("Sizes of the vectors aren'T equals");

			for (int i = 0; i < a.Length; i++)
				a[i] += b[i];

			return a;
		}

		public static Vector Subtract(this Vector a, Vector b)
		{
			if (a.Length != b.Length)
				throw new Exception("Sizes of the vectors aren'T equals");

			for (int i = 0; i < a.Length; i++)
				a[i] -= b[i];
			return a;
		}

		public static Vector Multiply(this Vector vector, double number)
		{
			for (int i = 0; i < vector.Length; i++)
				vector[i] *= number;
			return vector;
		}
	}
}