using Grid.Models;

using MathModels.Models;

namespace FemProducer.MatrixFactory
{
	public interface IMatrixFactory
	{
		public Matrix CreateMatrix(GridModel grid);
	}
}