using ResearchPaper;

namespace ReaserchPaper.Assemblier
{
	internal interface ICollector
	{
		public IList<Matrix> Collect(Grid.Grid grid);
	}
}
