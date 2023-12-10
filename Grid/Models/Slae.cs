using Tensus;

namespace FemProducer.Models
{
	internal class Slae
	{
		public Matrix Matrix;
		public Vector Vector;

		public Slae(Matrix matrix, Vector vector)
		{
			Matrix = matrix;
			Vector = vector;
		}

		public int Size => Matrix.Size;
	}
}