using System.Collections;

using Grid.Enum;

namespace Grid.Models
{
    public class SpatialCoordinates : IEnumerable<Node>
    {
        public readonly double[] X;
        public readonly double[] Y;
        public readonly double[] Z;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y">не указывать в одномерном случае</param>
        /// <param name="z">не указывать в двумерном случае</param>
        public SpatialCoordinates(double[] x, double[] y = null!, double[] z = null!)
        {
            X = x;
            Y = y ?? [0];
            Z = z ?? [0];
        }

        /// <summary>
        /// Метод принимает сечение и возвращает оси, которые ему соотвествуют.
        /// </summary>
        /// <param name="section">сечение, на его основе выбираем какаие 2 оси вернуть</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">2 возвращаемые оси</exception>
        internal (double[], double[]) GetAxises(AxisOrientation section)
        {
            return section switch
            {
                AxisOrientation.XY => (X, Y),
                AxisOrientation.XZ => (X, Z),
                AxisOrientation.YZ => (Y, Z),
                _ => throw new ArgumentException($"Wrong sections - {section.ToString()}!")
            };
        }

        public IEnumerator GetEnumerator() => GetEnumerator();

        public Node GetNode(int xIndex, int yIndex, int zIndex) => new Node(X[xIndex], Y[yIndex], Z[zIndex]);

        IEnumerator<Node> IEnumerable<Node>.GetEnumerator()
        {
            for (int k = 0; k < Z.Length; k++)
                for (int j = 0; j < Y.Length; j++)
                    for (int xIndex = 0; xIndex < X.Length; xIndex++)
                        yield return GetNode(xIndex, j, k);
        }
    }
}