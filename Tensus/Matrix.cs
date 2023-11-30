using ReaserchPaper;

namespace ResearchPaper
{
	public class Matrix
	{
		private double[] _di;
		private double[] _al, _au;
		private int[] _ja, _ia;

		public Matrix(double[] di, double[] al, double[] au, int[] ja, int[] ia)
		{
			_di = di;
			_al = al;
			_au = au;
			_ja = ja;
			_ia = ia;
		}

		public int Size => _di.Length;

		public double[][] ConvertToDenseFormat()
		{
			double[][] matrix = new double[Size][];
			for (int i = 0; i < Size; i++)
			{
				matrix[i] = new double[Size];
			}

			for (int i = 0; i < Size; i++)
			{
				matrix[i][i] = _di[i];
				for (int j = _ia[i]; j < _ia[i + 1]; j++)
				{
					matrix[i][_ja[j]] = _al[j];
					matrix[_ja[j]][i] = _au[j];
				}
			}

			return matrix;
		}

		public static Vector operator *(Matrix matrix, Vector vector)
		{
			if (matrix.Size != vector.Length)
				throw new Exception("Sizes of the matrix and vector aren'T equals");

			Vector result = new Vector(vector.Length);

			for (int i = 0; i < vector.Length; i++)
				result[i] = matrix._di[i] * vector[i];


			for (int i = 0; i < vector.Length; i++)
			{
				for (int j = matrix._ia[i]; j < matrix._ia[i + 1]; j++)
				{
					result[i] += matrix._al[j] * vector[matrix._ja[j]];
					result[matrix._ja[j]] += matrix._au[j] * vector[i];
				}
			}

			return result;
		}

		public static Matrix operator *(Matrix matrix, double b)
		{
			Matrix result = new Matrix(matrix._di, matrix._al, matrix._au, matrix._ja, matrix._ia);

			for (int i = 0; i < matrix.Size; i++)
				result._di[i] = matrix._di[i] * b;

			for (int i = 0; i < matrix._au.Count(); i++)
				result._au[i] = matrix._au[i] * b;

			for (int i = 0; i < matrix._al.Count(); i++)
				result._al[i] = matrix._al[i] * b;

			return result;
		}

		public static Matrix operator /(Matrix matrix, double b)
		{
			Matrix result = new Matrix(matrix._di, matrix._al, matrix._au, matrix._ja, matrix._ia);

			for (int i = 0; i < matrix.Size; i++)
				result._di[i] = matrix._di[i] / b;

			for (int i = 0; i < matrix._au.Count(); i++)
				result._au[i] = matrix._au[i] / b;

			for (int i = 0; i < matrix._al.Count(); i++)
				result._al[i] = matrix._al[i] / b;

			return result;
		}

		public static Matrix operator +(Matrix a, Matrix b)
		{
			Matrix result = new Matrix(a._di, a._al, a._au, a._ja, a._ia);

			for (int i = 0; i < a.Size; i++)
				result._di[i] = a._di[i] + b._di[i];

			for (int i = 0; i < a._au.Count(); i++)
				result._au[i] = a._au[i] + b._au[i];

			for (int i = 0; i < a._al.Count(); i++)
				result._al[i] = a._al[i] + b._al[i];

			return result;
		}

		public static Matrix operator -(Matrix a, Matrix b)
		{
			Matrix result = new Matrix(a._di, a._al, a._au, a._ja, a._ia);

			for (int i = 0; i < a.Size; i++)
				result._di[i] = a._di[i] - b._di[i];

			for (int i = 0; i < a._au.Count(); i++)
				result._au[i] = a._au[i] - b._au[i];

			for (int i = 0; i < a._al.Count(); i++)
				result._al[i] = a._al[i] - b._al[i];

			return result;
		}

		public void Print()
		{
			double[][] matrix = this.ConvertToDenseFormat();
			for (int i = 0; i < matrix.Length; i++)
			{
				for (int j = 0; j < matrix.Length; j++)
				{
					if (matrix[i][j] >= 0)
						Console.Write(" ");
					Console.Write(matrix[i][j].ToString("E2") + " ");
				}
				Console.WriteLine();
			}
			Console.WriteLine("\n\n");
		}

		public void Reset()
		{
			for (int i = 0; i < Size; i++)
				_di[i] = 0;

			for (int i = 0; i < _au.Count(); i++)
				_au[i] = 0;

			for (int i = 0; i < _al.Count(); i++)
				_al[i] = 0;
		}
	}
}