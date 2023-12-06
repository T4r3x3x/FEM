using ReaserchPaper;

namespace FemProducer.DTO
{
	/// <summary>
	/// 
	/// </summary>
	/// <param name="XW"></param>
	/// <param name="YW"></param>
	/// <param name="areas"></param>
	/// <param name="qx"></param>
	/// <param name="qy"></param>
	/// <param name="xSplitsCount"></param>
	/// <param name="ySplitsCount"></param>
	internal record GridParameters(Point[][] linesNodes, int[][] areas, double[] qx, double[] qy, List<int> xSplitsCount, List<int> ySplitsCount);
}
