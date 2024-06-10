using FemProducer.Basises.Abstractions;
using FemProducer.Basises.Helpers.BasisFunctions;
using FemProducer.Services;

using Grid.Models;

using MathModels.Models;

using NumericsMethods;

namespace FemProducer.Basises.Implementations.TwoDimensional
{
    internal class LinearRectangularCylindricalBasis : AbstractBasis
    {
        protected override int _nodesCountInElement => 4;

        public LinearRectangularCylindricalBasis(ProblemService problemService) : base(problemService) { }

        public override Dictionary<string, IList<IList<double>>> GetLocalMatrixes(FiniteElement finiteElement) => throw new NotImplementedException();

        public override IList<IList<double>> GetStiffnessMatrix(FiniteElement finiteElement)
        {
            var stiffnessMatrix = InitializeMatrix(_nodesCountInElement);

            var xLimits = new Node(finiteElement.Nodes[0].X, finiteElement.Nodes[1].X);
            var yLimits = new Node(finiteElement.Nodes[0].Y, finiteElement.Nodes[2].Y);

            for (int i = 0; i < _nodesCountInElement; i++)
                for (int j = 0; j < _nodesCountInElement; j++)
                {
                    StiffnessIntegrationFuncClass intFunc = new StiffnessIntegrationFuncClass(i, j, xLimits, yLimits);
                    stiffnessMatrix[i][j] += Integration.GaussIntegration(finiteElement.Nodes[0], finiteElement.Nodes[3], intFunc.StiffnessIntegrationFunction, Integration.PointsCount.Four);
                }


            return stiffnessMatrix;
        }

        public override IList<IList<double>> GetMassMatrix(FiniteElement finiteElement)
        {
            var massMatrix = InitializeMatrix(_nodesCountInElement);

            var xLimits = new Node(finiteElement.Nodes[0].X, finiteElement.Nodes[1].X);
            var yLimits = new Node(finiteElement.Nodes[0].Y, finiteElement.Nodes[2].Y);

            for (int i = 0; i < _nodesCountInElement; i++)
                for (int j = 0; j < _nodesCountInElement; j++)
                {
                    MassIntegrationFuncClass intFunc = new(i, j, xLimits, yLimits);
                    massMatrix[i][j] += Integration.GaussIntegration(finiteElement.Nodes[0], finiteElement.Nodes[3], intFunc.MassIntegrationalFunction, Integration.PointsCount.Four);
                }

            return massMatrix;
        }

        public override IList<double> GetSecondBoundaryData(FiniteElement finiteElement, Func<Node, int, double> func, int formulaNumber) => throw new NotImplementedException();

        public double[][] GetGradTMatrix(IList<Node> nodes, Node v)
        {
            double[][] matrix = new double[4][];
            var xLimits = new Node(nodes[0].X, nodes[1].X);
            var yLimits = new Node(nodes[0].Y, nodes[2].Y);
            for (int i = 0; i < 4; i++)
            {
                matrix[i] = new double[4];
                for (int j = 0; j < 4; j++)
                {
                    HMatrixClass hMatrixClass = new(j, i, xLimits, yLimits, v);
                    matrix[i][j] = NumericsMethods.Integration.GaussIntegration(nodes[0], nodes[3], hMatrixClass.HMatrixFunc, Integration.PointsCount.Four);
                }
            }

            return matrix;
        }

        class HMatrixClass
        {
            private readonly int _i, _j;
            private readonly Node _xLimits, _yLimits;
            private readonly Node _v;

            public HMatrixClass(int i, int j, Node xLimits, Node yLimits, Node v)
            {
                _i = i;
                _j = j;
                _xLimits = xLimits;
                _yLimits = yLimits;
                _v = v;
            }

            public double HMatrixFunc(double r, double z)
            {
                Node node = new Node(r, z);
                double result = _v.X * LinearBasisFunctions2D.XDerivativeBasisFunction(_i, node, _xLimits, _yLimits) + _v.Y * LinearBasisFunctions2D.YDerivativeBasisFunction(_i, node, _xLimits, _yLimits);
                result *= LinearBasisFunctions2D.GetBasisFunctionValue(_j, node, _xLimits, _yLimits) * r;
                return result;

            }
        }

        public override (IList<IList<double>>, IList<double>) GetThirdBoundaryData(Slae slae, FiniteElement finiteElement, Func<Node, int, double> func, int formulaNumber)
        {
            double betta = 200;
            Node limits;
            double secondVariable;
            bool isR;
            if (finiteElement.Nodes[0].X - finiteElement.Nodes[1].X == 0)
            {
                limits = new Node(finiteElement.Nodes[0].Y, finiteElement.Nodes[1].Y);
                secondVariable = finiteElement.Nodes[0].X;
                isR = false;
            }
            else
            {
                limits = new Node(finiteElement.Nodes[0].X, finiteElement.Nodes[1].X);
                secondVariable = finiteElement.Nodes[0].Y;

                isR = true;
            }

            var localVector = new double[2];

            for (int i = 0; i < finiteElement.Nodes.Length; i++)
            {
                SecondBoundaryConditionClass intF = new SecondBoundaryConditionClass(limits, func, i, formulaNumber, isR, secondVariable);
                localVector[i] = Integration.GaussIntegration(limits, intF.SecondBoundaryConditionFunc, Integration.PointsCount.Two);
                localVector[i] *= betta;
            }

            var localMatrix = new double[finiteElement.Nodes.Length][];
            for (int i = 0; i < finiteElement.Nodes.Length; i++)
            {
                localMatrix[i] = new double[finiteElement.Nodes.Length];
                for (int j = 0; j < finiteElement.Nodes.Length; j++)
                {
                    ThirdBoundaryConditionMatrixClass intF = new ThirdBoundaryConditionMatrixClass(limits, func, i, j, formulaNumber, isR, secondVariable);
                    localMatrix[i][j] = Integration.GaussIntegration(limits, intF.ThirdBoundaryConditionMatrixFunc, Integration.PointsCount.Two);
                    localMatrix[i][j] *= betta;
                }
            }

            return (localMatrix, localVector);
        }

        class ThirdBoundaryConditionMatrixClass
        {
            private Node _Limits;
            private Func<Node, int, double> _func;
            private int _i, _j;
            private int _formulaNumber;
            private bool _isR;
            private double _secondVariable;

            public ThirdBoundaryConditionMatrixClass(Node Limits, Func<Node, int, double> func, int i, int j, int formulaNumber, bool isR, double secondVariable)
            {
                _Limits = Limits;
                _func = func;
                _i = i;
                _j = j;
                _formulaNumber = formulaNumber;
                _isR = isR;
                _secondVariable = secondVariable;
            }

            public double ThirdBoundaryConditionMatrixFunc(double r)
            {
                Node node;

                if (_isR)
                    node = new Node(r, _secondVariable);
                else
                    node = new Node(_secondVariable, r);

                double h = _Limits.Y - _Limits.X;
                double limit1 = _i == 0 ? _Limits.Y : _Limits.X;
                double limit2 = _j == 0 ? _Limits.Y : _Limits.X;
                double res = LinearBasisFunctions.s_Functions[_i](r, limit1, h) * LinearBasisFunctions.s_Functions[_j](r, limit2, h) * r;
                return res;
            }
        }

        class SecondBoundaryConditionClass
        {
            private Node _Limits;
            private Func<Node, int, double> _func;
            private int _i = 0;
            private int _formulaNumber;
            private bool _isR;
            private double _secondVariable;
            public SecondBoundaryConditionClass(Node Limits, Func<Node, int, double> func, int i, int formulaNumber, bool isR, double secondVariable)
            {
                _Limits = Limits;
                _func = func;
                _i = i;
                _formulaNumber = formulaNumber;
                _isR = isR;
                _secondVariable = secondVariable;
            }

            public double SecondBoundaryConditionFunc(double r)
            {
                Node node;

                if (!_isR)
                    node = new Node(r, _secondVariable);
                else
                    node = new Node(_secondVariable, r);

                double h = _Limits.Y - _Limits.X;
                double limit = _i == 0 ? _Limits.Y : _Limits.X;
                var res = _func(node, _formulaNumber) * LinearBasisFunctions.s_Functions[_i](r, limit, h) * r;
                return res;
            }
        }

        class LocalVectorIntegrationFuncClass
        {
            private Node _xLimits, _yLimits;
            private Func<Node, int, double> _func;
            private int _i = 0;
            private int _formulaNumber;
            public LocalVectorIntegrationFuncClass(Node xLimits, Node yLimits, Func<Node, int, double> func, int i, int formulaNumber)
            {
                _xLimits = xLimits;
                _yLimits = yLimits;
                _func = func;
                _i = i;
                _formulaNumber = formulaNumber;
            }

            public double LocalVectorIntegrationFunc(double r, double z)
            {
                var node = new Node(r, z);
                var res = _func(node, _formulaNumber) * LinearBasisFunctions2D.GetBasisFunctionValue(_i, node, _xLimits, _yLimits) * r;
                return res;
            }
        }

        class StiffnessIntegrationFuncClass
        {
            private readonly int _i, _j;
            private readonly Node _xLimits, _yLimits;

            public StiffnessIntegrationFuncClass(int i, int j, Node xLimits, Node yLimits)
            {
                _i = i;
                _j = j;
                _xLimits = xLimits;
                _yLimits = yLimits;
            }

            public double StiffnessIntegrationFunction(double r, double z)
            {
                Node node = new Node(r, z);
                var xW1 = LinearBasisFunctions2D.XDerivativeBasisFunction(_i, node, _xLimits, _yLimits);
                var xW2 = LinearBasisFunctions2D.XDerivativeBasisFunction(_j, node, _xLimits, _yLimits);
                var yW1 = LinearBasisFunctions2D.YDerivativeBasisFunction(_i, node, _xLimits, _yLimits);
                var yW2 = LinearBasisFunctions2D.YDerivativeBasisFunction(_j, node, _xLimits, _yLimits);
                var result = (xW1 * xW2 + yW1 * yW2);

                return result * r;
            }
        }

        class MassIntegrationFuncClass
        {
            private readonly int _i, _j;
            private readonly Node _xLimits, _yLimits;

            public MassIntegrationFuncClass(int i, int j, Node xLimits, Node yLimits)
            {
                _i = i;
                _j = j;
                _xLimits = xLimits;
                _yLimits = yLimits;
            }

            public double MassIntegrationalFunction(double r, double z)
            {
                Node node = new Node(r, z);
                return LinearBasisFunctions2D.GetBasisFunctionValue(_i, node, _xLimits, _yLimits) * LinearBasisFunctions2D.GetBasisFunctionValue(_j, node, _xLimits, _yLimits) * r;

            }
        }
    }
}