using MathModels;

namespace Grid.Models
{
	public class GridParameters
	{
		public Point[][] linesNodes;
		public int[][] areas;
		public double[] qx;
		public double[] qy;
		public List<int> xSplitsCount;
		public List<int> ySplitsCount;
	}
}
