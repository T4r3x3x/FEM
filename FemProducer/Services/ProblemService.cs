using FemProducer.Models;

using Grid.Models;

namespace FemProducer.Services
{
    public class ProblemService
    {
        private readonly ProblemParameters _problemParameters;

        public ProblemService(ProblemParameters problemParameters) => _problemParameters = problemParameters;

        public double Lambda(int formulaNumber)
        {
            if (formulaNumber > _problemParameters.Lambda.Count - 1)
                throw new ArgumentException($"Лямбда для формулы {formulaNumber} не задана!");

            return _problemParameters.Lambda[formulaNumber];
        }

        public double Gamma(int formulaNumber)
        {
            if (formulaNumber > _problemParameters.Gamma.Count - 1)
                throw new ArgumentException($"Гамма для формулы {formulaNumber} не задана!");

            return _problemParameters.Gamma[formulaNumber];
        }

        public double Function(Node node, int area)
        {
            var x = node.X;
            var y = node.Y;
            var z = node.Z;

            return area switch
            {
                _ => x * y * z,

                //_ => throw new ArgumentException(),
            };
        }

        public double SecondBoundaryFunction(Node node, int formulaNumber)
        {
            var x = node.X;
            var y = node.Y;
            var z = node.Z;
            return formulaNumber switch
            {
                0 => y * z,
                1 => -y * z,
                _ => throw new ArgumentException($"Гамма для формулы {formulaNumber} не задана!")
            };
        }

        public double ThridBoundaryFunction(Node node, int formulaNumber)
        {
            var x = node.X;
            var y = node.Y;
            var z = node.Z;

            return formulaNumber switch
            {
                0 => Function(node, formulaNumber) + SecondBoundaryFunction(node, formulaNumber),
                1 => Function(node, formulaNumber) + SecondBoundaryFunction(node, formulaNumber),

                _ => throw new ArgumentException(),
            };
        }

        public double F(Node node, int formulaNumber)
        {
            var x = node.X;
            var y = node.Y;
            var z = node.Z;
            return formulaNumber switch
            {
                _ => Gamma(formulaNumber) * Function(node, formulaNumber),
                //_ => throw new ArgumentException(),
            };
        }
    }
}
