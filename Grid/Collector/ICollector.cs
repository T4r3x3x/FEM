using ResearchPaper;

namespace ReaserchPaper.Assemblier
{
	internal interface ICollector
	{
		public (IList<Matrix>, Vector) Collect();
	}
}
