using FemProducer.Basises.Abstractions;
using FemProducer.Collector.CollectorBase.Interfaces;
using FemProducer.MatrixBuilding;
using FemProducer.Services;

using Grid.Models;

using MathModels.Models;

using Tools;

namespace FemProducer.Collector.CollectorBases.Implementations
{
    public class CollectorBase : ICollectorBase
    {
        private readonly GridModel _grid;
        private readonly MatrixFactory _matrixFactory;
        private readonly ProblemService _problemService;
        private readonly object _lock = new object();
        private readonly AbstractBasis _basis;

        public CollectorBase(GridModel grid, MatrixFactory matrixFactory, ProblemService problemService, AbstractBasis basis)
        {
            _grid = grid;
            _matrixFactory = matrixFactory;
            _problemService = problemService;
            _basis = basis;
        }

        public (Dictionary<string, Matrix>, Vector) Collect()
        {
            return GetMatrixesMG();
        }

        private (Dictionary<string, Matrix>, Vector) GetMatrixesMG()
        {
            Matrix M = _matrixFactory.CreateMatrix(_grid);
            Matrix G = _matrixFactory.CreateMatrix(_grid);
            Matrix H = _matrixFactory.CreateMatrix(_grid);
            Vector vector = new Vector(M.Size);

            //	Parallel.ForEach(_grid.Elements, element =>
            foreach (var elementSheme in _grid.Elements)
            {
                int formulaNumber = elementSheme.FormulaNumber;
                var element = _grid.GetFiniteElement(elementSheme);

                var massMatrix = _basis.GetMassMatrix(element);
                var localVector = _basis.GetLocalVector(element, _problemService.F, formulaNumber, massMatrix);
                var stiffnessMatrix = _basis.GetStiffnessMatrix(element);

                massMatrix.MultiplyLocalMatrix(_problemService.Gamma(formulaNumber));
                AddLocalMatrix(M, massMatrix, elementSheme);

                stiffnessMatrix.MultiplyLocalMatrix(_problemService.Lambda(formulaNumber));
                AddLocalMatrix(G, stiffnessMatrix, elementSheme);

                AddLocalVector(vector, localVector, elementSheme);
            }//);

            Dictionary<string, Matrix> matrixes = new() { { "_M", M }, { "_G", G }, { "H", H } };
            return (matrixes, vector);
        }

        private void AddLocalVector(Vector vector, IList<double> localVector, FiniteElementScheme element)
        {
            lock (_lock)
                for (int i = 0; i < localVector.Count; i++)
                    vector[element.NodesIndexes[i]] += localVector[i];
        }

        private void AddLocalMatrix(Matrix matrix, IList<IList<double>> localMatrix, FiniteElementScheme element)
        {
            lock (_lock)
            {
                for (int p = 0; p < element.NodesIndexes.Length; p++)
                {
                    matrix.Di[element.NodesIndexes[p]] += localMatrix[p][p];

                    int ibeg = matrix.Ia[element.NodesIndexes[p]];
                    int iend = matrix.Ia[element.NodesIndexes[p] + 1];
                    for (int k = 0; k < p; k++)
                    {
                        int index = SearchingAlghoritms.BinarySearch(matrix.Ja, element.NodesIndexes[k], ibeg, iend - 1);

                        matrix.Au[index] += localMatrix[k][p];
                        matrix.Al[index] += localMatrix[p][k];
                        ibeg++;
                    }
                }
            }
        }

        public void GetBoundaryConditions(Slae slae)
        {
            ConsiderSecondBoundaryConditions(slae);
            ConsiderThirdBoundaryConditions(slae);
            ConsiderFirstBoundaryConditions(slae);
        }

        private void ConsiderFirstBoundaryConditions(Slae slae)
        {
            //    Parallel.ForEach(_grid.FirstBoundaryNodes, boundaryNodeIndex =>
            // {
            foreach (var boundaryNodeIndex in _grid.FirstBoundaryNodes)
            {
                var area = _grid.GetSubDomain(_grid.Nodes[boundaryNodeIndex]);
                SetFisrtBoundaryCondition(slae, boundaryNodeIndex, area);
            }
            // });
        }

        private void SetFisrtBoundaryCondition(Slae slae, int boundaryNodeIndex, int area)
        {
            slae.Matrix.ZeroingRow(boundaryNodeIndex);
            slae.Matrix.Di[boundaryNodeIndex] = 1;
            slae.Vector[boundaryNodeIndex] = _problemService.Function(_grid.Nodes[boundaryNodeIndex], area);
        }

        private void ConsiderSecondBoundaryConditions(Slae slae)
        {
            //Parallel.ForEach(_grid.SecondBoundaryNodes, nodesIndexes =>
            foreach (var sheme in _grid.SecondBoundaryNodes)
            {
                var finiteElement = _grid.GetFiniteElement(sheme);
                var res = _basis.GetSecondBoundaryData(finiteElement, _problemService.SecondBoundaryFunction, sheme.FormulaNumber);
                AddLocalVector(slae.Vector, res, sheme);
            };
        }

        private void ConsiderThirdBoundaryConditions(Slae slae)
        {
            //Parallel.ForEach(_grid.ThirdBoundaryNodes, nodesIndexes =>
            foreach (var sheme in _grid.ThirdBoundaryNodes)
            {
                var finiteElement = _grid.GetFiniteElement(sheme);
                var res = _basis.GetThirdBoundaryData(slae, finiteElement, _problemService.ThridBoundaryFunction, sheme.FormulaNumber);
                AddLocalMatrix(slae.Matrix, res.matrix, sheme);
                AddLocalVector(slae.Vector, res.vector, sheme);
            };
        }
    }
}