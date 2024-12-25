using MathModels.Models;

namespace FemProducer.Collectors.CollectorBases.Interfaces
{
    public interface ICollectorBase
    {
        (Dictionary<string, Matrix>, Vector) Collect();
        void GetBoundaryConditions(Slae slae);
    }
}
