namespace Grid.Models
{
	public class Node
	{
		public double X, Y, Z;

		public Node(double x, double y)
		{
			X = x;
			Y = y;
		}

		public Node(double x, double y, double z)
		{
			X = x;
			Y = y;
			Z = z;
		}

		public override string ToString() => $"{X.ToString().Replace(",", ".")} {Y.ToString().Replace(",", ".")}";
	}
}