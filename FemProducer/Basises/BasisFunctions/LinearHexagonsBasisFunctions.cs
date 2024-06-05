namespace FemProducer.Basises.BasisFunctions
{
    using MathModels.Models;

    using System.ComponentModel;

    using static BasisUnitFunctions;
    using static IntegerFuncitons3D;

    public class LinearHexagonsBasisFunctions
    {
        /// <summary>
        /// Basis function in unit coordinate system
        /// </summary>
        /// <param name="i">function number</param>
        /// <param name="ksi"></param>
        /// <param name="mu"></param>
        /// <param name="theta"></param>
        /// <returns></returns>
        public static double Fita(int i, double ksi, double mu, double theta) =>
            W[Mu(i)](ksi) * W[Nu(i)](mu) * W[Eps(i)](theta);//+

        public static double FitaDivKsi(int i, double mu, double theta) => WDiv[Mu(i)]() * W[Nu(i)](mu) * W[Eps(i)](theta);//+
        public static double FitaDivMu(int i, double ksi, double theta) => W[Mu(i)](ksi) * WDiv[Nu(i)]() * W[Eps(i)](theta);//+
        public static double FitaDivTheta(int i, double ksi, double mu) => W[Mu(i)](ksi) * W[Nu(i)](mu) * WDiv[Eps(i)]();//+

        public static Vector GradFita(int i, double ksi, double mu, double theta)
        {
            return new Vector([FitaDivKsi(i, mu, theta), FitaDivMu(i, ksi, theta), FitaDivTheta(i, ksi, mu)]);//+
        }

        /// <summary>
        /// Функция преобразования координат из единичной в декартову
        /// </summary>
        /// <param name="nums">значения коордитаны по которой происходит замена в узлах кэ</param>
        /// <param name="ksi"></param>
        /// <param name="mu"></param>
        /// <param name="theta"></param>
        /// <returns></returns>
        public static double ReverseCoordinateSubstitution(IList<double> nums, double ksi, double mu, double theta)//+
        {
            double res = 0;
            for (int i = 0; i < nums.Count; i++)
                res += nums[i] * Fita(i, ksi, mu, theta);
            return res;
        }

        /// <summary>
        /// Возвращает значение дифференциала по переменной в точке 
        /// </summary>
        /// <param name="nums">значение переменной в узлах кэ</param>
        /// <param name="a">первая компонента точки</param>
        /// <param name="b">вторая компонента точки</param>
        /// <param name="divVariable">переменная по которой необходимо продифференцировать</param>
        /// <returns></returns>
        public static double VariableDiv(IList<double> nums, double a, double b, DivVariable divVariable)//+
        {
            double res = 0;
            var div = VariableDivFunc(divVariable);
            for (int i = 0; i < nums.Count; i++)
                res += nums[i] * div(i, a, b);
            return res;
        }

        /// <summary>
        /// Возвращает продифференцированную базисную функцию по одной из переменных
        /// </summary>
        /// <param name="divVariable">переменная по которой происходит дифференизация</param>
        /// <returns></returns>
        private static Func<int, double, double, double> VariableDivFunc(DivVariable divVariable)
        {
            return divVariable switch
            {
                DivVariable.Ksi => FitaDivKsi,
                DivVariable.Mu => FitaDivMu,
                DivVariable.Theta => FitaDivTheta,
                _ => throw new InvalidEnumArgumentException()
            };
        }


        public enum DivVariable
        {
            Ksi,
            Mu,
            Theta
        }
    }
}