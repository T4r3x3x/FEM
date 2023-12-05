namespace FemProducer.DTO
{
	internal record GridParameters(double[] XW, double[] YW, int[][] areas, double[] qx, double[] qy, List<int> xSplitsCount, List<int> ySplitsCount);
}
