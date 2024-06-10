using Grid.Enum;
using Grid.Models;

namespace FemProducer.Basises.Implementations.TwoDimensional
{
    internal class LinearQuadrangularCartesianBasisCoefficients
    {
        public double b1, b2, b3, b4, b5, b6, a0, a1, a2;

        public LinearQuadrangularCartesianBasisCoefficients(FiniteElement element) => CalculateCoefficients(element);

        private void CalculateCoefficients(FiniteElement element)
        {
            switch (element.AxisOrientation)
            {
                case AxisOrientation.XY:
                    {
                        CalculateCoefficientsByOrientaition(element.Nodes, AxisOrientation.X, AxisOrientation.Y);
                        break;
                    };
                case AxisOrientation.XZ:
                    {
                        CalculateCoefficientsByOrientaition(element.Nodes, AxisOrientation.X, AxisOrientation.Z);
                        break;
                    };
                case AxisOrientation.YZ:
                    {
                        CalculateCoefficientsByOrientaition(element.Nodes, AxisOrientation.Y, AxisOrientation.Z);
                        break;
                    };
                default:
                    throw new NotImplementedException();
            };
        }
        /// <summary>
        /// Вычисляем коэффициенты относительно ориентации
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="x">внутренняя ось</param>
        /// <param name="y">внешняя ось</param>
        private void CalculateCoefficientsByOrientaition(IList<Node> nodes, AxisOrientation x, AxisOrientation y)
        {
            var p1 = nodes[0];
            var p2 = nodes[1];
            var p3 = nodes[2];
            var p4 = nodes[3];

            b1 = p3.Component(x) - p1.Component(x);
            b2 = p2.Component(x) - p1.Component(x);
            b3 = p3.Component(y) - p1.Component(y);
            b4 = p2.Component(y) - p1.Component(y);
            b5 = p1.Component(x) - p2.Component(x) - p3.Component(x) + p4.Component(x);
            b6 = p1.Component(y) - p2.Component(y) - p3.Component(y) + p4.Component(y);

            a0 = (p2.Component(x) - p1.Component(x)) * (p3.Component(y) - p1.Component(y))
                - (p2.Component(y) - p1.Component(y)) * (p3.Component(x) - p1.Component(x));
            a1 = (p2.Component(x) - p1.Component(x)) * (p4.Component(y) - p3.Component(y))
                - (p2.Component(y) - p1.Component(y)) * (p4.Component(x) - p3.Component(x));
            a2 = (p3.Component(y) - p1.Component(y)) * (p4.Component(x) - p2.Component(x))
                - (p3.Component(x) - p1.Component(x)) * (p4.Component(y) - p2.Component(y));
        }
    }
}