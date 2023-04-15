using ResearchPaper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReaserchPaper
{
    internal class Vector
    {
        public int Length => Elements.Length;

        public double[] Elements;

        public Vector(int length, double[] Elements = null)
        {
            this.Elements = Elements ?? new double[length];
        }

        public double GetNorm()
        {
            double result = 0;

            for (int i = 0; i < Length; i++)
                result += Elements[i] * Elements[i];

            return Math.Sqrt(result);
        }

        public static double operator *(Vector a, Vector b)
        {
            if (a.Length == b.Length)
            {
                double result = 0;

                for (int i = 0; i < a.Elements.Length; i++)
                    result += a.Elements[i] * b.Elements[i];

                return result;
            }
            else
                throw new Exception("Sizes of the vectors aren't equals");
        }

        public static Vector operator +(Vector a, Vector b)
        {
            if (a.Length == b.Length)
            {
                Vector result = new Vector(a.Length, null);

                for (int i = 0; i < a.Elements.Length; i++)
                    result.Elements[i] = a.Elements[i] +b.Elements[i];

                return result;
            }
            else
                throw new Exception("Sizes of the vectors aren't equals");
        }

        public static Vector operator -(Vector a, Vector b)
        {
            if (a.Length == b.Length)
            {
                Vector result = new Vector(a.Length, null);

                for (int i = 0; i < a.Elements.Length; i++)
                    result.Elements[i] = a.Elements[i] - b.Elements[i];

                return result;
            }
            else
                throw new Exception("Sizes of the vectors aren't equals");
        }

        public static Vector operator *(double a, Vector b)
        {
            Vector result = new Vector(Master.Slau.b.Elements.Length, null);

            for (int i = 0; i < b.Length; i++)
                result.Elements[i] = a * b.Elements[i];

            return result;
        }

        public void Print()
        {
            foreach (var element in Elements)
            {
                Console.Write(element + " ");
            }
            Console.WriteLine("\n\n");
        }

        internal void Reset()
        {
            for (int i = 0; i < Length; i++)
            {
                Elements[i] = 0;
            }
        }
    }
}