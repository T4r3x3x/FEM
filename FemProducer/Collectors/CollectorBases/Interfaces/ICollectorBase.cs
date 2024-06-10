using MathModels.Models;

namespace FemProducer.Collector.CollectorBase.Interfaces
{
    public interface ICollectorBase
    {
        (Dictionary<string, Matrix>, Vector) Collect();
        void GetBoundaryConditions(Slae slae);
    }
}
