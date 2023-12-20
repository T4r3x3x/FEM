using FemProducer.MatrixBuilding;

using Grid.Models;

using MathModels.Models;

namespace FemProducer.Collector
{
	internal class EllipticCollector : AbstractCollector
	{
		private Matrix _matrix;
		private Vector _vector;

		public EllipticCollector(ICollectorBase collector, GridModel grid, MatrixFactory matrixFactory) : base(collector, grid, matrixFactory)
		{
			_matrix = _matrixFactory.CreateMatrix(grid);
			_vector = new Vector(_matrix.Size);
			//Slae slae = new Slae(matrix, vector);
		}

		public override Slae Collect(int timeLayer)
		{
			ResetSlae();
			var result = _collectorBase.Collect();
			var matrixes = result.Item1;
			_vector = result.Item2;
			var slae = GetSlae(matrixes.GetValueOrDefault("M"), matrixes.GetValueOrDefault("G"));
			return slae;
		}

		private void ResetSlae()
		{
			_matrix.Reset();
			_vector.Reset();
		}

		private Slae GetSlae(Matrix M, Matrix G)
		{
			_matrix = M + G;

			var slae = new Slae(_matrix, _vector);
			_collectorBase.GetBoundaryConditions(slae);

			return slae;
		}
	}
}
