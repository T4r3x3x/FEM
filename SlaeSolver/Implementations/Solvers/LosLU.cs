using MathModels.Extensions;
using MathModels.Models;

using SlaeSolver.Interfaces;

namespace SlaeSolver.Implementations.Solvers
{
	public class LosLU : ISolver
	{
		private readonly int maxItCount;
		private readonly double eps;

		public LosLU(int maxItCount, double eps)
		{
			this.maxItCount = maxItCount;
			this.eps = eps;
		}

		public Vector Solve(Slae slae)
		{
			var vectors = ILUsqModi(slae.Matrix);
			var D = vectors.Item1;
			var L = vectors.Item2;
			var U = vectors.Item3;
			return LOSLU(slae, D, L, U);
		}


		private Vector LOSLU(Slae slae, Vector D, Vector L, Vector U)
		{
			var size = slae.Size;
			double a, b;
			Vector solve = new Vector(size);
			Vector r, z, p, Ur, LAUr;

			int k = 0;

			r = ForwardStepModi(slae.Matrix, slae.Vector, D, L);
			z = BackStepModi(slae.Matrix, r, U);
			p = ForwardStepModi(slae.Matrix, slae.Matrix * z, D, L);

			do
			{
				k++;

				a = GetCoefficent(p, r, p, p);
				solve += a * z;
				r -= a * p;

				Ur = BackStepModi(slae.Matrix, r, U);
				LAUr = ForwardStepModi(slae.Matrix, slae.Matrix * Ur, D, L);

				b = -GetCoefficent(p, LAUr, p, p);

				z = Ur.Add(b * z);//+

				p = LAUr.Add(b * p);//+

			} while (r * r > eps && k <= maxItCount);

			return solve;
		}

		private double GetCoefficent(Vector a1, Vector b1, Vector a2, Vector b2)
		{
			double result;
			result = a1 * b1 / (a2 * b2);
			//  Console.WriteLine(ScalarMultiplication(a1, b1) + " / " + ScalarMultiplication(a2, b2));
			return result;
		}

		private (Vector, Vector, Vector) ILUsqModi(Matrix matrix)
		{
			int i;

			var D = new Vector(matrix.Size);
			var L = new Vector(matrix.Al.Count());
			var U = new Vector(matrix.Au.Count());

			for (i = 0; i < matrix.Al.Count(); i++)
				L[i] = matrix.Al[i];
			for (i = 0; i < matrix.Au.Count(); i++)
				U[i] = matrix.Au[i];
			for (i = 0; i < matrix.Di.Length; i++)
				D[i] = matrix.Di[i];


			for (i = 0; i < matrix.Size; i++)
			{
				double ls, us, d = 0;
				int temp = matrix.Ia[i];
				for (int j = matrix.Ia[i]; j < matrix.Ia[i + 1]; j++)
				{
					ls = 0;
					us = 0;
					for (int h = matrix.Ia[matrix.Ja[j]], k = temp; h < matrix.Ia[matrix.Ja[j] + 1] && k < j;)
						if (matrix.Ja[k] == matrix.Ja[h])
						{
							ls += L[k] * U[h];
							us += L[h++] * U[k++];
						}
						else
						{
							if (matrix.Ja[k] < matrix.Ja[h])
								k++;
							else
								h++;
						}
					L[j] -= ls;
					U[j] = (U[j] - us) / D[matrix.Ja[j]];
					d += L[j] * U[j];
				}
				D[i] -= d;
			}
			return (D, L, U);
		}

		private Vector ForwardStepModi(Matrix matrix, Vector rightSide, Vector D, Vector L)
		{
			Vector result = new Vector(matrix.Size);
			double sum;

			for (int i = 0; i < matrix.Size; i++)
			{
				sum = 0;
				for (int j = matrix.Ia[i]; j < matrix.Ia[i + 1]; j++)
					sum += result[matrix.Ja[j]] * L[j];

				result[i] = (rightSide[i] - sum) / D[i];
			}
			return result;
		}

		private Vector BackStepModi(Matrix matrix, Vector rightSide, Vector U)
		{
			Vector result = new Vector(matrix.Size);

			for (int i = 0; i < matrix.Size; i++)
				result[i] = rightSide[i];

			for (int i = matrix.Size - 1; i >= 0; i--)
				for (int j = matrix.Ia[i]; j < matrix.Ia[i + 1]; j++)
					result[matrix.Ja[j]] -= result[i] * U[j];

			return result;
		}
	}
}