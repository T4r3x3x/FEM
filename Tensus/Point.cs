namespace ReaserchPaper
{
	public class Point
	{
		public double X, Y;

		public Point(double X, double Y)
		{
			this.X = X;
			this.Y = Y;
		}

		public void Print()
		{
			Console.WriteLine(X + " " + Y);
		}
	}
}
