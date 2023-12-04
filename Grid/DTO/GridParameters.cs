namespace FemProducer.DTO
{
	internal record GridParameters(double[] XW, double[] YW, int[][] areas, double[] qx, double[] qy, List<int> xAreaLenghtes, List<int> yAreaLenghtes);
}
