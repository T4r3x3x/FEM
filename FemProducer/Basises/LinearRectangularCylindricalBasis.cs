using FemProducer.Basises.BasisFunctions;

using Grid.Models;

using NumericsMethods;

namespace FemProducer.Basises
{
	internal class LinearRectangularCylindricalBasis : IBasis
	{
		private const int NodesCount = 4;

		public Dictionary<string, IList<IList<double>>> GetLocalMatrixes(IList<Node> nodes) => throw new NotImplementedException();
		public IList<double> GetLocalVector(IList<Node> nodes, Func<Node, double> func)
		{
			double[] localVector = new double[NodesCount];
			Node xLimits = new Node(nodes[0].X, nodes[1].X);
			Node yLimits = new Node(nodes[0].Y, nodes[2].Y);

			for (int i = 0; i < NodesCount; i++)
			{
				LocalVectorIntegrationFuncClass intF = new LocalVectorIntegrationFuncClass(nodes, xLimits, yLimits, func, i);
				localVector[i] += Integration.GaussIntegration(nodes[0], nodes[3], intF.LocalVectorIntegrationFunc, Integration.PointsCount.Two);
			}

			return localVector;
		}

		public IList<IList<double>> GetStiffnessMatrix(IList<Node> nodes)
		{
			double[][] stiffnessMatrix = new double[NodesCount][];

			for (int i = 0; i < NodesCount; i++)
			{
				stiffnessMatrix[i] = new double[NodesCount];
				for (int j = 0; j < NodesCount; j++)
				{
					var xLimits = new Node(nodes[0].X, nodes[1].X);
					var yLimits = new Node(nodes[0].Y, nodes[2].Y);
					StiffnessIntegrationFuncClass intFunc = new StiffnessIntegrationFuncClass(i, j, xLimits, yLimits);
					stiffnessMatrix[i][j] += Integration.GaussIntegration(nodes[0], nodes[3], intFunc.StiffnessIntegrationFunction, Integration.PointsCount.Two);
				}
			}

			return stiffnessMatrix;
		}

		public IList<IList<double>> GetMassMatrix(IList<Node> nodes)
		{
			double[][] massMatrix = new double[NodesCount][];

			for (int i = 0; i < NodesCount; i++)
			{
				massMatrix[i] = new double[NodesCount];
				for (int j = 0; j < NodesCount; j++)
				{
					var xLimits = new Node(nodes[0].X, nodes[1].X);
					var yLimits = new Node(nodes[0].Y, nodes[2].Y);
					MassIntegrationFuncClass intFunc = new(i, j, xLimits, yLimits);
					massMatrix[i][j] += Integration.GaussIntegration(nodes[0], nodes[3], intFunc.MassIntegrationalFunction, Integration.PointsCount.Two);
				}
			}

			return massMatrix;
		}

		class LocalVectorIntegrationFuncClass
		{
			private IList<Node> _nodes;
			private Node _xLimits, _yLimits;
			private Func<Node, double> _func;
			private int _i = 0;

			public LocalVectorIntegrationFuncClass(IList<Node> nodes, Node xLimits, Node yLimits, Func<Node, double> func, int i)
			{
				this._nodes = nodes;
				_xLimits = xLimits;
				_yLimits = yLimits;
				_func = func;
				_i = i;
			}

			public double LocalVectorIntegrationFunc(double r, double z)
			{
				var node = new Node(r, z);
				var res = _func(node) * LinearBasisFunctions.GetBasisFunctionValue(_i, node, _xLimits, _yLimits) * r;
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
				return (LinearBasisFunctions.XDerivativeBasisFunction(_i, node, _xLimits, _yLimits) * LinearBasisFunctions.XDerivativeBasisFunction(_j, node, _xLimits, _yLimits)
					+ LinearBasisFunctions.YDerivativeBasisFunction(_i, node, _xLimits, _yLimits) * LinearBasisFunctions.YDerivativeBasisFunction(_j, node, _xLimits, _yLimits)) * r;

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
				return LinearBasisFunctions.GetBasisFunctionValue(_i, node, _xLimits, _yLimits) * LinearBasisFunctions.GetBasisFunctionValue(_j, node, _xLimits, _yLimits) * r;

			}
		}
	}
}
