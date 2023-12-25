using FemProducer.Basises.BasisFunctions;

using Grid.Models;

using MathModels.Models;

using NumericsMethods;

namespace FemProducer.Basises
{
	internal class LinearRectangularCylindricalBasis : AbstractBasis
	{
		private const int NodesCount = 4;

		public LinearRectangularCylindricalBasis(ProblemService problemService) : base(problemService)
		{
		}

		public override Dictionary<string, IList<IList<double>>> GetLocalMatrixes(IList<Node> nodes) => throw new NotImplementedException();
		public override IList<double> GetLocalVector(IList<Node> nodes, Func<Node, int, double> func, int formulaNumber)
		{
			double[] localVector = new double[NodesCount];
			Node xLimits = new Node(nodes[0].X, nodes[1].X);
			Node yLimits = new Node(nodes[0].Y, nodes[2].Y);

			for (int i = 0; i < NodesCount; i++)
			{
				LocalVectorIntegrationFuncClass intF = new LocalVectorIntegrationFuncClass(xLimits, yLimits, func, i, formulaNumber);
				localVector[i] += Integration.GaussIntegration(nodes[0], nodes[3], intF.LocalVectorIntegrationFunc, Integration.PointsCount.Three);
			}

			return localVector;
		}

		public override IList<IList<double>> GetStiffnessMatrix(IList<Node> nodes)
		{
			double[][] stiffnessMatrix = new double[NodesCount][];
			var xLimits = new Node(nodes[0].X, nodes[1].X);
			var yLimits = new Node(nodes[0].Y, nodes[2].Y);

			for (int i = 0; i < NodesCount; i++)
			{
				stiffnessMatrix[i] = new double[NodesCount];
				for (int j = 0; j < NodesCount; j++)
				{
					StiffnessIntegrationFuncClass intFunc = new StiffnessIntegrationFuncClass(i, j, xLimits, yLimits);
					stiffnessMatrix[i][j] += Integration.GaussIntegration(nodes[0], nodes[3], intFunc.StiffnessIntegrationFunction, Integration.PointsCount.Three);
				}
			}

			return stiffnessMatrix;
		}

		public override IList<IList<double>> GetMassMatrix(IList<Node> nodes)
		{
			double[][] massMatrix = new double[NodesCount][];
			var xLimits = new Node(nodes[0].X, nodes[1].X);
			var yLimits = new Node(nodes[0].Y, nodes[2].Y);

			for (int i = 0; i < NodesCount; i++)
			{
				massMatrix[i] = new double[NodesCount];
				for (int j = 0; j < NodesCount; j++)
				{

					MassIntegrationFuncClass intFunc = new(i, j, xLimits, yLimits);
					massMatrix[i][j] += Integration.GaussIntegration(nodes[0], nodes[3], intFunc.MassIntegrationalFunction, Integration.PointsCount.Three);
				}
			}

			return massMatrix;
		}

		public override void ConsiderSecondBoundaryCondition(Slae slae, IList<Node> nodes, IList<int> nodesIndexes) => throw new NotImplementedException();

		private Node GetV(int areaNumber)
		{
			return areaNumber switch
			{
				0 => new Node(1, 0),
				1 => new Node(0, 1),
				2 => new Node(-1, 0),
				3 => new Node(0, -1),
			};
		}

		public double[][] GetGradTMatrix(IList<Node> nodes, int areaNumber)
		{
			double[][] matrix = new double[4][];
			var xLimits = new Node(nodes[0].X, nodes[1].X);
			var yLimits = new Node(nodes[0].Y, nodes[2].Y);
			var v = new Node(0, 0);//GetV(areaNumber);
			for (int i = 0; i < 4; i++)
			{
				matrix[i] = new double[4];
				for (int j = 0; j < 4; j++)
				{
					HMatrixClass hMatrixClass = new(j, i, xLimits, yLimits, v);
					matrix[i][j] = NumericsMethods.Integration.GaussIntegration(nodes[0], nodes[3], hMatrixClass.HMatrixFunc, Integration.PointsCount.Two);
				}
			}

			return matrix;
		}

		class HMatrixClass
		{
			private readonly int _i, _j;
			private readonly Node _xLimits, _yLimits;
			private Node _v;
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

		public override (IList<IList<double>>, IList<double>) ConsiderThirdBoundaryCondition(Slae slae, IList<Node> nodes, IList<int> nodesIndexes, Func<Node, int, double> func, int formulaNumber)
		{
			double betta = 1;
			Node limits;
			double secondVariable;
			bool isR;
			if (nodes[0].X - nodes[1].X == 0)
			{
				limits = new Node(nodes[0].Y, nodes[1].Y);
				secondVariable = nodes[0].X;
				isR = false;
			}
			else
			{
				limits = new Node(nodes[0].X, nodes[1].X);
				secondVariable = nodes[0].Y;

				isR = true;
			}

			var localVector = new double[2];


			for (int i = 0; i < nodes.Count; i++)
			{
				SecondBoundaryConditionClass intF = new SecondBoundaryConditionClass(limits, func, i, formulaNumber, isR, secondVariable);
				localVector[i] = Integration.GaussIntegration(limits, intF.SecondBoundaryConditionFunc, Integration.PointsCount.Two);
				localVector[i] *= betta;
			}

			var localMatrix = new double[nodes.Count][];
			for (int i = 0; i < nodes.Count; i++)
			{
				localMatrix[i] = new double[nodes.Count];
				for (int j = 0; j < nodes.Count; j++)
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
				var result = (xW1 * xW2 + yW1 * yW2) * r;

				return result;
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