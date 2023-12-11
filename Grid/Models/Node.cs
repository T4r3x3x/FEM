namespace Grid.Models
{
	public class Node
	{
		public double X, Y;

		public Node(double x, double y)
		{
			X = x;
			Y = y;
		}

		public override string ToString() => $"{X.ToString().Replace(",", ".")} {Y.ToString().Replace(",", ".")}";
	}
}