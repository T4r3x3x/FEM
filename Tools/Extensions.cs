using System.ComponentModel.DataAnnotations;

namespace Tools
{
    public static class Extensions
    {
        /// <summary>
        /// Умножает элементы матрицы на заданный коэффициент.
        /// </summary>
        /// <param name="localMatrix"></param>
        /// <param name="coefficient"></param>
        public static void MultiplyLocalMatrix(this IList<IList<double>> localMatrix, double coefficient)
        {
            for (int p = 0; p < localMatrix.Count; p++)
                for (int k = 0; k < localMatrix.Count; k++)
                    localMatrix[p][k] *= coefficient;
        }

        public static string ToCommonLine(this IEnumerable<ValidationResult> results)
        {
            string messages = string.Empty;

            foreach (var error in results)
                messages += error + "\n";

            return messages;
        }
    }
}
