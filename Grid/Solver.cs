using ReaserchPaper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static ResearchPaper.Solver;

namespace ResearchPaper
{
    internal class Solver
    {
        static int maxItCount = 10000;
        static double a, eps = 1e-40, b;
        static Vector r, z, p;
        static Vector D, L, U;

        public enum SolverType
        {
            ILU, LOS,
        }


        public Vector Solve(Vector f, Matrix A)
        {
            return LOSLU(Master.Slau.b,Master.Slau.A);
        }

      
        static Vector LOSLU(Vector f, Matrix A)
        {
            Vector solve = new Vector(Grid.NodesCount);
            r = new Vector(Grid.NodesCount);
            z = new Vector(Grid.NodesCount);
            p = new Vector(Grid.NodesCount);
            Vector Ur = new Vector(Grid.NodesCount);
            Vector temp = new Vector(Grid.NodesCount);
            Vector LAUr = new Vector(Grid.NodesCount);
            ILUsqModi(A);


            int k = 0;

            r = ForwardStepModi(A,f); //+                      
            z = BackStepModi(A,r);
            p = ForwardStepModi(A,A * z);//+

            do
            {
                k++;

                a = GetCoefficent(p, r, p, p); //+
                solve += a * z;
                r -= a * p;
                
                Ur = BackStepModi(A, r);
                LAUr = ForwardStepModi(A, A * Ur);

                b = -GetCoefficent(p, LAUr, p, p);               

                z = Ur + b * z; 
                p = LAUr + b * p;
               
            } while (r * r > eps && k <= maxItCount);
            //   Console.WriteLine("iter counts: " + k);
            //   Console.WriteLine("disperancy: " + r * r);
            return solve;
        }
        static double GetCoefficent(Vector a1, Vector b1, Vector a2, Vector b2)
        {
            double result;
            result = a1 * b1 / (a2 * b2);
            //  Console.WriteLine(ScalarMultiplication(a1, b1) + " / " + ScalarMultiplication(a2, b2));
            return result;
        }

        static void ILUsqModi(Matrix A)
        {
            int i;
            int i0, i1, ki, kj;
            double suml, sumu, sumd;
         
            D = new Vector(A.Size);
            L = new Vector(A.al.Count());
            U = new Vector(A.au.Count());



            for (i = 0; i < A.al.Count(); i++)
                L.Elements[i] = A.al[i];
            for (i = 0; i < A.au.Count(); i++)
                U.Elements[i] = A.au[i];
            for (i = 0; i < A.di.Length; i++)
                D.Elements[i] = A.di[i];


            for (i = 0; i < A.Size; i++)
            {
                double d = 0;
                int temp = A.ia[i];
                for (int j = A.ia[i]; j < A.ia[i + 1]; j++)
                {
                    double ls = 0;
                    double us = 0;
                    for (int h = A.ia[A.ja[j]], k = temp; h < A.ia[A.ja[j] + 1] && k < j;)
                        if (A.ja[k] == A.ja[h])
                        {
                            ls += L.Elements[k] * U.Elements[h];
                            us += L.Elements[h++] * U.Elements[k++];
                        }
                        else
                        {
                            if (A.ja[k] < A.ja[h])
                                k++;
                            else
                                h++;
                        }
                    L.Elements[j] -= ls;
                    U.Elements[j] = (U.Elements[j] - us) / D.Elements[A.ja[j]];
                    d += L.Elements[j] * U.Elements[j];
                }
                D.Elements[i] -= d;
            }
        }

        static Vector ForwardStepModi(Matrix A,Vector rightSide)
        {
            Vector result = new Vector(A.Size);
            for (int i = 0; i < A.Size; i++)
            {
                double sum = 0;
                for (int j = A.ia[i]; j < A.ia[i + 1]; j++)
                    sum += result.Elements[A.ja[j]] * L.Elements[j];

                result.Elements[i] = (rightSide.Elements[i] - sum) / D.Elements[i];
            }
            return result;
        }

        static Vector BackStepModi(Matrix A, Vector rightSide)
        {
            Vector result = new Vector(A.Size);

            for (int i = 0; i < A.Size; i++)
                result.Elements[i] = rightSide.Elements[i];
            for (int i = A.Size - 1; i >= 0; i--)
            {
                for (int j = A.ia[i]; j < A.ia[i + 1]; j++)
                    result.Elements[A.ja[j]] -= result.Elements[i] * U.Elements[j];
            }

            return result;
        }       
    }
}
