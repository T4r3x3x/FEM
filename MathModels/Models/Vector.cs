using System.Collections;

namespace MathModels.Models
{
    public class Vector : IEnumerable<double>
    {
        private double[] Elements;

        public Vector(int length)
        {
            this.Elements = new double[length];
        }

        public Vector(double[] elements)
        {
            Elements = elements;
        }

        public double this[int index]
        {
            get => Elements[index];
            set => Elements[index] = value;
        }

        public int Length => Elements.Length;

        public double GetNorm()
        {
            double result = 0;

            for (int i = 0; i < Length; i++)
                result += Elements[i] * Elements[i];

            return Math.Sqrt(result);
        }

        public static double operator *(Vector a, Vector b)
        {
            if (a.Length != b.Length)
                throw new Exception("Sizes of the vectors aren'T equals");

            double result = 0;

            for (int i = 0; i < a.Elements.Length; i++)
                result += a.Elements[i] * b.Elements[i];

            return result;
        }

        public static Vector operator +(Vector a, Vector b)
        {
            if (a.Length != b.Length)
                throw new Exception("Sizes of the vectors aren'T equals");

            Vector result = new Vector(a.Length);

            for (int i = 0; i < a.Elements.Length; i++)
                result.Elements[i] = a.Elements[i] + b.Elements[i];

            return result;
        }

        public static Vector operator -(Vector a, Vector b)
        {
            if (a.Length != b.Length)
                throw new Exception("Sizes of the vectors aren'T equals");

            Vector result = new Vector(a.Length);

            for (int i = 0; i < a.Elements.Length; i++)
                result.Elements[i] = a.Elements[i] - b.Elements[i];

            return result;
        }

        public static Vector operator *(double a, Vector b)
        {
            Vector result = new Vector(b.Length);

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

        public void Reset()
        {
            for (int i = 0; i < Length; i++)
            {
                Elements[i] = 0;
            }
        }

        public static implicit operator Vector(double[] nums) => new Vector(nums);

        public IEnumerator<double> GetEnumerator() => ((IEnumerable<double>) Elements).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => Elements.GetEnumerator();
    }
}