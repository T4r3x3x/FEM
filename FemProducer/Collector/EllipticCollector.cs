using FemProducer.MatrixBuilding;

using Grid.Models;

using MathModels.Models;

namespace FemProducer.Collector
{
	public class EllipticCollector : AbstractCollector
	{
		public EllipticCollector(ICollectorBase collector, GridModel grid, MatrixFactory matrixFactory) : base(collector, grid, matrixFactory)
		{
		}

		public override Slae Collect(int timeLayer)
		{
			(var matrixes, var vector) = _collectorBase.Collect();
			var slae = GetSlae(matrixes.GetValueOrDefault("M"), matrixes.GetValueOrDefault("G"), vector);
			return slae;
		}

		private Slae GetSlae(Matrix M, Matrix G, Vector vector)
		{
			var matrix = M + G;

			var slae = new Slae(matrix, vector);

			_collectorBase.GetBoundaryConditions(slae);

			return slae;
		}
	}
}
