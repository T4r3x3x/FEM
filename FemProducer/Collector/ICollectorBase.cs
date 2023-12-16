using MathModels.Models;

namespace FemProducer.Collector
{
	public interface ICollectorBase
	{
		(Dictionary<string, Matrix>, Vector) Collect();
		void GetBoundaryConditions(Slae slae);
	}
}
