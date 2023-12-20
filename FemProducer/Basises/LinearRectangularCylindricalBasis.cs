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
				localVector[i] += Integration.GaussIntegration(nodes[0], nodes[3], intF.LocalVectorIntegrationFunc, Integration.PointsCount.Two);
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
					stiffnessMatrix[i][j] += Integration.GaussIntegration(nodes[0], nodes[3], intFunc.StiffnessIntegrationFunction, Integration.PointsCount.Two);
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
					massMatrix[i][j] += Integration.GaussIntegration(nodes[0], nodes[3], intFunc.MassIntegrationalFunction, Integration.PointsCount.Two);
				}
			}

			return massMatrix;
		}

		public override void ConsiderSecondBoundaryCondition(Slae slae, Node node, int nodeIndex) => throw new NotImplementedException();
		public override void ConsiderThirdBoundaryCondition(Slae slae, Node node, int nodeIndex) => throw new NotImplementedException();

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