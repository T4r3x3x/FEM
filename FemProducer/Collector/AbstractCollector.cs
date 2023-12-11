using MathModels.Models;

namespace FemProducer.Collector
{
	internal abstract class AbstractCollector
	{
		protected readonly ICollectorBase _collectorBase;

		protected AbstractCollector(ICollectorBase collectorBase) => this._collectorBase = collectorBase;

		public abstract Slae Collect(int timeLayer);
	}
}
