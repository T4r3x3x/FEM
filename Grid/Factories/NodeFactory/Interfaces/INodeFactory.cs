using Grid.Models;

using MathModels;
using MathModels.Models;

namespace Grid.Factories.NodeFactory.Interfaces
{
    public interface INodeFactory
    {
        (List<Node> nodes, List<int> missingNodesCounts) GetNodes(Area<int>[] areas, Area<double>[] subDomains, Point[][] lines, SpatialCoordinates coordinates);
    }
}
