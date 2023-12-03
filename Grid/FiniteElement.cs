namespace ReaserchPaper
{
	public class FiniteElement
	{
		public int ElemNumber;
		public Point XBoundaries, YBoundaries;
		int i, j;
		public double hx => XBoundaries.Y - XBoundaries.X;
		public double hy => YBoundaries.Y - YBoundaries.X;
	}
}
