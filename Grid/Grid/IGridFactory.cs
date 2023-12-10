using FemProducer.Models;

namespace FemProducer.Grid
{
	public interface IGridFactory
	{
		public GridModel GetGrid(GridParameters gridParametrs);
	}
}
