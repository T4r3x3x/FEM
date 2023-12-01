using ResearchPaper;

namespace ReaserchPaper
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

		public double Size => Matrix.Size;
	}
}