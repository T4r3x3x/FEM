using Grid.Enum;

namespace Grid.Models
{
    public class Node
    {
        public double X, Y, Z;

        public Node(double x)
        {
            X = x;
        }

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

        public double Component(AxisOrientation axisOrientation)
        {
            return axisOrientation switch
            {
                AxisOrientation.X => X,
                AxisOrientation.Y => Y,
                AxisOrientation.Z => Z,
                _ => throw new NotImplementedException(),
            };
        }
        public override string ToString() =>
            $"{X.ToString().Replace(",", ".")} {Y.ToString().Replace(",", ".")} {Z.ToString().Replace(",", ".")}";
    }
}