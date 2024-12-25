namespace MathModels.Models
{
	public class Point
	{
		public double X, Y;

		public Point(double X, double Y)
		{
			this.X = X;
			this.Y = Y;
		}

		public override string ToString()
		{
			return $"{X} {Y}";
		}
	}
}
