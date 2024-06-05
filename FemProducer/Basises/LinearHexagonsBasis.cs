using FemProducer.Basises.BasisFunctions;
using FemProducer.Services;
using Grid.Models;

using MathModels.Models;

using NumericsMethods;

namespace FemProducer.Basises
{
    using static LinearHexagonsBasisFunctions;

    public class LinearHexagonsBasis : AbstractBasis
    {
        private readonly Node FirstNode = new(0, 0, 0), SecondNode = new(1, 1, 0), ThirdNode = new Node(1, 1, 1);
        private readonly Node singleSquareFirstPoint = new Node(0, 0);
        private readonly Node singleSquareFourthPoint = new Node(1, 1);
        private readonly LinearQuadrangularCartesianBasis _linearQuadrangularBasis;
        public const int NodesCount = 8;

        public LinearHexagonsBasis(ProblemService problemService) : base(problemService)
        {
            _linearQuadrangularBasis = new LinearQuadrangularCartesianBasis(problemService);
        }

        public override IList<IList<double>> GetMassMatrix(IList<Node> nodes)
        {
            double[][] result = new double[nodes.Count][];
            for (int i = 0; i < result.Length; i++)
                result[i] = new double[result.Length];

            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result.Length; j++)
                {
                    var integrFunc = (double ksi, double mu, double theta) =>
                    {
                        var jacobian = Jacobian(nodes, ksi, mu, theta);
                        return Fita(i, ksi, mu, theta) * Fita(j, ksi, mu, theta) * jacobian;
                    };
                    result[i][j] = Integration.GaussIntegration(
                        FirstNode,
                        SecondNode,
                        ThirdNode,
                        integrFunc,
                        Integration.PointsCount.Three);
                }
            return result;
        }

        public override IList<IList<double>> GetStiffnessMatrix(IList<Node> nodes)
        {
            double[][] result = new double[nodes.Count][];
            for (int i = 0; i < result.Length; i++)
                result[i] = new double[result.Length];

            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result.Length; j++)
                {
                    var integrFunc = (double ksi, double mu, double theta) =>
                    {
                        var jacobi = GetJacobiMatrix(nodes, ksi, mu, theta);
                        var detJ = jacobi.Determinant();
                        var inverseJacobi = jacobi.GetInverseMatrix();
                        var gradFitaI = GradFita(i, ksi, mu, theta);
                        var gradFitaJ = GradFita(j, ksi, mu, theta);
                        return (inverseJacobi * gradFitaI) * (inverseJacobi * gradFitaJ) * detJ;

                    };
                    result[i][j] = Integration.GaussIntegration(
                        FirstNode,
                        SecondNode,
                        ThirdNode,
                        integrFunc,
                        Integration.PointsCount.Three);
                }
            return result;
        }

        public override IList<double> GetLocalVector(IList<Node> nodes, Func<Node, int, double> func, int formulaNumber)
        {
            double[] localVector = new double[NodesCount];
            var fs = new double[NodesCount];

            for (int i = 0; i < NodesCount; i++)
                fs[i] = func(nodes[i], formulaNumber);

            var g = GetMassMatrix(nodes);

            for (int i = 0; i < NodesCount; i++)
            {
                localVector[i] = 0;
                for (int j = 0; j < NodesCount; j++)
                    localVector[i] += fs[j] * g[i][j];
            }

            return localVector;
        }

        public override Dictionary<string, IList<IList<double>>> GetLocalMatrixes(IList<Node> nodes)
        {
            throw new NotImplementedException();
        }

        public override IList<double> GetSecondBoundaryVector(IList<Node> nodes, Func<Node, int, double> func, int formulaNumber)
        {
            //   return _linearQuadrangularBasis.GetLocalVector(nodes, func, formulaNumber);


            IList<double>? result = _linearQuadrangularBasis.GetLocalVector(nodes, func, formulaNumber);
            return result;
        }

        public override (IList<IList<double>>, IList<double>) ConsiderThirdBoundaryCondition(Slae slae, IList<Node> nodes, IList<int> nodesIndexes, Func<Node, int, double> func, int formulaNumber)
        {
            var matrix = _linearQuadrangularBasis.GetMassMatrix(nodes);
            var vector = _linearQuadrangularBasis.GetLocalVector(nodes, func, formulaNumber);
            return (matrix, vector);
        }

        #region Support stuff
        private static void GetComponents(IList<Node> nodes, out double[] x, out double[] y, out double[] z)
        {
            x = nodes.Select(_ => _.X).ToArray();
            y = nodes.Select(_ => _.Y).ToArray();
            z = nodes.Select(_ => _.Z).ToArray();
        }

        /// <summary>
        /// Функция вычисления матрицы якобиана
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="ksi"></param>
        /// <param name="mu"></param>
        /// <param name="theta"></param>
        /// <returns>якобиан</returns>
        private DenseMatrix GetJacobiMatrix(IList<Node> nodes, double ksi, double mu, double theta)//разделить на формирование матрицы (понадобится при вычислении матрицы жесткости) и на нахождение якобиана
        {
            GetComponents(nodes, out var x, out var y, out var z);

            double[] xDivs = [VariableDiv(x, mu, theta, DivVariable.Ksi), VariableDiv(x, ksi, theta, DivVariable.Mu), VariableDiv(x, ksi, mu, DivVariable.Theta)];//1й столбец матрицы Якоби
            double[] yDivs = [VariableDiv(y, mu, theta, DivVariable.Ksi), VariableDiv(y, ksi, theta, DivVariable.Mu), VariableDiv(y, ksi, mu, DivVariable.Theta)];//2й столбец матрицы Якоби
            double[] zDivs = [VariableDiv(z, mu, theta, DivVariable.Ksi), VariableDiv(z, ksi, theta, DivVariable.Mu), VariableDiv(z, ksi, mu, DivVariable.Theta)];//3й столбец матрицы Якоби

            var elems = new double[3, 3] { { xDivs[0], yDivs[0], zDivs[0] }, { xDivs[1], yDivs[1], zDivs[1] }, { xDivs[2], yDivs[2], zDivs[2] } };
            return new(elems);
        }
        /// <summary>
        /// Функция вычисления якобиана. Захардкодил формулу ибо так проще и по-другому нет смысла делать. Вычисляется с помощью треугольников.
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="ksi"></param>
        /// <param name="mu"></param>
        /// <param name="theta"></param>
        /// <returns>якобиан</returns>
        private double Jacobian(IList<Node> nodes, double ksi, double mu, double theta)//разделить на формирование матрицы (понадобится при вычислении матрицы жесткости) и на нахождение якобиана
        {
            var jacobi = GetJacobiMatrix(nodes, ksi, mu, theta);
            return jacobi.Determinant();
        }


        private static Node GetNodeInCortesianCoordinateSystem(double ksi, double mu, double theta, IList<Node> nodes)
        {
            GetComponents(nodes, out var xComp, out var yComp, out var zComp);
            var x = ReverseCoordinateSubstitution(xComp, ksi, mu, theta);
            var y = ReverseCoordinateSubstitution(yComp, ksi, mu, theta);
            var z = ReverseCoordinateSubstitution(zComp, ksi, mu, theta);
            return new Node(x, y, z);
        }
        #endregion
    }
}