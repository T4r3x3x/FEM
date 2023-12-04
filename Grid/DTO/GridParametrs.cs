namespace FemProducer.DTO
{
	internal record GridParametrs(int n, double[] XW, int m, double[] YW, int[][] areas, double[] qx, double[] qy, List<int> xAreaLenghtes, List<int> yAreaLenghtes);
}
