using Grid.Models;
using Grid.Models.InputModels;

namespace Grid.Factories.GridFactories.Interfaces
{
    public interface IGridFactory
    {
        public GridModel GetGrid(GridInputParameters gridParametrs);
    }
}
