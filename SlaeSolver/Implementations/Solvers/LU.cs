using MathModels.Models;

using SlaeSolver.Interfaces;

namespace SlaeSolver.Implementations.Solvers
{
	public class LU : ISolver
	{
		double[] temp;
		double[][] L, U;
		int size;

		private void ForwardStep(Vector b)
		{
			int size = b.Length;
			temp = new double[size];

			for (int i = 0; i < size; i++)
			{
				temp[i] = b[i];
				for (int j = 0; j < i; j++)
				{
					temp[i] -= L[i][j] * temp[j];
				}
				temp[i] /= L[i][i];
			}
		}
		private Vector BackStep()
		{
			Vector result = new Vector(size);
			for (int i = size - 1; i >= 0; i--)
			{
				result[i] = temp[i];
				for (int j = size - 1; j > i; j--)
				{
					result[i] -= U[i][j] * result[j];
				}
				result[i] /= U[i][i];
			}
			return result;
		}
		private void ILU(double[][] A)
		{
			int size = A.Length;
			L = new double[size][];
			U = new double[size][];

			for (int i = 0; i < size; i++)
			{
				L[i] = new double[size];

				U[i] = new double[size];
				for (int j = 0; j < size; j++)
				{
					L[i][j] = 0;
					L[i][i] = 1;
					U[i][j] = 0;
				}
			}

			for (int i = 0; i < size; i++)
			{
				for (int j = 0; j < size; j++)
				{

					if (i > j)
					{
						L[i][j] = A[i][j];
						for (int k = 0; k < j; k++)
						{
							L[i][j] -= L[i][k] * U[k][j];
						}
						L[i][j] /= U[j][j];
					}
					else //i < j
					{
						U[i][j] = A[i][j];
						for (int k = 0; k < i; k++)
						{
							U[i][j] -= L[i][k] * U[k][j];
						}
					}
				}
			}
		}
		public Vector Solve(Slae slae)
		{
			var matrix = slae.Matrix.ConvertToDenseFormat();
			size = slae.Size;
			ILU(matrix);
			ForwardStep(slae.Vector);
			return BackStep();
		}
	}
}
