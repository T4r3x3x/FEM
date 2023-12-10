using Tensus;

namespace FemProducer.Collector
{
	internal interface ICollectorBase
	{
		(IList<Matrix>, Vector) Collect();
	}
}
