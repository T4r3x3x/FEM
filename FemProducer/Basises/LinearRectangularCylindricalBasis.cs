using FemProducer.Basises.BasisFunctions;

using Grid.Models;

namespace FemProducer.Basises
{
	internal class LinearRectangularCylindricalBasis : IBasis
	{
		private double MassIntegrationalFunction(double r, double z)
		{

			return 0;
		}

		public Dictionary<string, IList<IList<double>>> GetLocalMatrixes(IList<Node> nodes) => throw new NotImplementedException();
		public IList<IList<double>> GetMassMatrix(IList<Node> nodes) => throw new NotImplementedException();
		public IList<IList<double>> GetStiffnessMatrix(IList<Node> nodes) => throw new NotImplementedException();
		public IList<double> GetLocalVector(IList<Node> nodes, Func<Node, double> func) => LinearBasisFunctions.GetLocalVector(nodes, func);
	}
}
