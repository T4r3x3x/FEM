using MathModels.Models;

namespace MathModels.Extensions
{
	public static class Operations
	{

		public static void Add(this Vector a, Vector b)
		{
			if (a.Length != b.Length)
				throw new Exception("Sizes of the vectors aren'T equals");

			for (int i = 0; i < a.Length; i++)
				a[i] += b[i];
		}

		public static void Subtract(this Vector a, Vector b)
		{
			if (a.Length != b.Length)
				throw new Exception("Sizes of the vectors aren'T equals");

			for (int i = 0; i < a.Length; i++)
				a[i] -= b[i];
		}

		public static void Multiply(this Vector vector, double number)
		{
			for (int i = 0; i < vector.Length; i++)
				vector[i] *= number;
		}
	}
}