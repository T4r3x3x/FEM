using ResearchPaper;

namespace ReaserchPaper
{
	public class Slae
	{
		public Matrix Matrix;
		public Vector Vector;

		public Slae(Matrix matrix, Vector vector)
		{
			Matrix = matrix;
			Vector = vector;
		}

		public double Size => Matrix.Size;

		public void Print()
		{
			double[][] matrix = Matrix.ConvertToDenseFormat();
			for (int i = 0; i < matrix.Length; i++)
			{
				for (int j = 0; j < matrix.Length; j++)
				{
					if (matrix[i][j] >= 0)
						Console.Write(" ");
					Console.Write(matrix[i][j].ToString("E2") + " ");
				}
				Console.Write(" " + Vector[i].ToString("E2"));
				Console.WriteLine();
			}
			Console.WriteLine("\n\n");
		}
	}
}