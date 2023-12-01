using ReaserchPaper;
using ReaserchPaper.Assemblier;
using ReaserchPaper.Grid;

namespace FemProducer.Assemblier
{
	internal class CollectorTimeDecorator : Collector
	{
		private readonly ICollector _collector;
		private readonly Grid _grid;

		public CollectorTimeDecorator(ICollector collector, Grid grid, int nodesCount) : base(nodesCount) => _collector = collector;

		public Slae Collect(int timeLayer)
		{
			throw new NotImplementedException();
		}
	}
}
