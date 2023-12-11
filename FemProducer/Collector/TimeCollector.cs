using FemProducer.Models;

namespace FemProducer.Collector
{
	internal class TimeCollector : AbstractCollector
	{
		public TimeCollector(ICollectorBase collectorBase) : base(collectorBase)
		{
		}

		public override Slae Collect(int timeLayer) => throw new NotImplementedException();
	}
}
