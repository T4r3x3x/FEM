using Grid.Models;

using MathModels.Models;

namespace FemProducer.MatrixBuilding
{
	public interface IMatrixFactory
	{
		public Matrix CreateMatrix(GridModel grid);
	}
}