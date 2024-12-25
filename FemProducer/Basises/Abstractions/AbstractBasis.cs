using FemProducer.Services;

using Grid.Models;

using MathModels.Models;

namespace FemProducer.Basises.Abstractions
{
    public abstract class AbstractBasis
    {
        protected AbstractBasis? _boundaryBasis;
        protected readonly ProblemService _problemService;

        protected AbstractBasis(ProblemService problemService) => _problemService = problemService;

        protected abstract int NodesCountInElement { get; }

        public abstract IList<IList<double>> GetMassMatrix(FiniteElement finiteElement);
        public abstract IList<IList<double>> GetStiffnessMatrix(FiniteElement finiteElement);
        public abstract Dictionary<string, IList<IList<double>>> GetLocalMatrixes(FiniteElement finiteElement);
        public virtual IList<double> GetLocalVector(FiniteElement finiteElement, Func<Node, int, double> func, int formulaNumber, IList<IList<double>> massMatrix = null!)
        {
            var localVector = InitializeVector(NodesCountInElement);

            massMatrix ??= GetMassMatrix(finiteElement);
            var funcValues = GetRightSideFuctionValuesInNodes(finiteElement.Nodes, func, formulaNumber);

            for (int i = 0; i < NodesCountInElement; i++)
                for (int j = 0; j < NodesCountInElement; j++)
                    localVector[i] += funcValues[j] * massMatrix[i][j];
            return localVector;
        }

        public virtual IList<double> GetSecondBoundaryData(FiniteElement finiteElement, Func<Node, int, double> func, int formulaNumber)
        {
            if (_boundaryBasis is null) throw new ArgumentNullException();
            return _boundaryBasis.GetLocalVector(finiteElement, func, formulaNumber);
        }

        public virtual (IList<IList<double>> matrix, IList<double> vector) GetThirdBoundaryData(Slae slae, FiniteElement finiteElement, Func<Node, int, double> func, int formulaNumber)
        {
            if (_boundaryBasis is null) throw new ArgumentNullException();
            var matrix = _boundaryBasis.GetMassMatrix(finiteElement);
            var vector = _boundaryBasis.GetLocalVector(finiteElement, func, formulaNumber, matrix);
            return (matrix, vector);
        }

        /// <summary>
        /// Расчитывает значение вектора правой части уравнения в точках
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="func"></param>
        /// <param name="formulaNumber"></param>
        /// <returns></returns>
        protected IList<double> GetRightSideFuctionValuesInNodes(IList<Node> nodes, Func<Node, int, double> func, int formulaNumber)
        {
            var funcValues = new double[nodes.Count];
            for (int i = 0; i < nodes.Count; i++)
                funcValues[i] = func(nodes[i], formulaNumber);
            return funcValues;
        }

        protected static IList<IList<double>> InitializeMatrix(int lenght)
        {
            double[][] matrix = new double[lenght][];
            for (int i = 0; i < lenght; i++)
                matrix[i] = new double[lenght];
            return matrix;
        }

        protected static IList<double> InitializeVector(int lenght) => new double[lenght];
    }
}
