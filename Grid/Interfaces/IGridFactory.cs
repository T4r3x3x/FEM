using Grid.Models;

namespace Grid.Interfaces
{
	public interface IGridFactory
	{
		public GridModel GetGrid(GridParameters gridParametrs);
	}
}
