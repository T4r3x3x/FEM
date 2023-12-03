using ReaserchPaper;
using ReaserchPaper.Solver;

namespace ResearchPaper
{
	internal class LosLU : ISolver
	{
		private int maxItCount = 10000;
		private double a, eps = 1e-40, b;
		private Vector r, z, p;
		private Vector D, L, U;

		public Vector Solve(Slae slae)
		{
			return LOSLU(slae);
		}


		private Vector LOSLU(Slae slae)
		{
			var size = slae.Size;
			Vector solve = new Vector(size);
			r = new Vector(size);
			z = new Vector(size);
			p = new Vector(size);
			Vector Ur = new Vector(size);
			Vector temp = new Vector(size);
			Vector LAUr = new Vector(size);
			ILUsqModi(slae.Matrix);

			int k = 0;

			r = ForwardStepModi(slae.Matrix, slae.Vector); //+                      
			z = BackStepModi(slae.Matrix, r);
			p = ForwardStepModi(slae.Matrix, slae.Matrix * z);//+

			do
			{
				k++;

				a = GetCoefficent(p, r, p, p); //+
				solve += a * z;
				r -= a * p;

				Ur = BackStepModi(slae.Matrix, r);
				LAUr = ForwardStepModi(slae.Matrix, slae.Matrix * Ur);

				b = -GetCoefficent(p, LAUr, p, p);

				z = Ur + b * z;
				p = LAUr + b * p;

			} while (r * r > eps && k <= maxItCount);
			//   Console.WriteLine("iter counts: " + k);
			//   Console.WriteLine("disperancy: " + r * r);
			return solve;
		}

		private double GetCoefficent(Vector a1, Vector b1, Vector a2, Vector b2)
		{
			double result;
			result = a1 * b1 / (a2 * b2);
			//  Console.WriteLine(ScalarMultiplication(a1, b1) + " / " + ScalarMultiplication(a2, b2));
			return result;
		}

		private void ILUsqModi(Matrix matrix)
		{
			int i;

			D = new Vector(matrix.Size);
			L = new Vector(matrix.Al.Count());
			U = new Vector(matrix.Au.Count());



			for (i = 0; i < matrix.Al.Count(); i++)
				L[i] = matrix.Al[i];
			for (i = 0; i < matrix.Au.Count(); i++)
				U[i] = matrix.Au[i];
			for (i = 0; i < matrix.Di.Length; i++)
				D[i] = matrix.Di[i];


			for (i = 0; i < matrix.Size; i++)
			{
				double d = 0;
				int temp = matrix.Ia[i];
				for (int j = matrix.Ia[i]; j < matrix.Ia[i + 1]; j++)
				{
					double ls = 0;
					double us = 0;
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
		}

		private Vector ForwardStepModi(Matrix matrix, Vector rightSide)
		{
			Vector result = new Vector(matrix.Size);
			for (int i = 0; i < matrix.Size; i++)
			{
				double sum = 0;
				for (int j = matrix.Ia[i]; j < matrix.Ia[i + 1]; j++)
					sum += result[matrix.Ja[j]] * L[j];

				result[i] = (rightSide[i] - sum) / D[i];
			}
			return result;
		}

		private Vector BackStepModi(Matrix matrix, Vector rightSide)
		{
			Vector result = new Vector(matrix.Size);

			for (int i = 0; i < matrix.Size; i++)
				result[i] = rightSide[i];
			for (int i = matrix.Size - 1; i >= 0; i--)
			{
				for (int j = matrix.Ia[i]; j < matrix.Ia[i + 1]; j++)
					result[matrix.Ja[j]] -= result[i] * U[j];
			}

			return result;
		}


	}
}
