using MathModels.Models;

namespace FemProducer.Collector
{
	internal interface ICollectorBase
	{
		(Dictionary<string, Matrix>, Vector) Collect();
	}
}
