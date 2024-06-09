using FemProducer.Basises;
using FemProducer.MatrixBuilding;
using FemProducer.Services;

using Grid.Models;

using MathModels.Models;

using Tools;

namespace FemProducer.Collector
{
    public class CollectorBase : ICollectorBase
    {
        private readonly GridModel _grid;
        private readonly MatrixFactory _matrixFactory;
        private readonly ProblemService _problemService;
        private object _lock = new object();
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

                var localMatrix = _basis.GetMassMatrix(element);
                localMatrix.MultiplyLocalMatrix(_problemService.Gamma(formulaNumber));
                AddLocalMatrix(M, localMatrix, elementSheme);

                //lock (vector)
                //{
                //    for (int i = 0; i < 8; i++)
                //    {
                //        for (int j = 0; j < 8; j++)
                //        {
                //            Console.Write(localMatrix[i][j] + " ");
                //        }
                //        Console.WriteLine();
                //    }
                //}

                localMatrix = _basis.GetStiffnessMatrix(element);
                localMatrix.MultiplyLocalMatrix(_problemService.Lambda(formulaNumber));
                AddLocalMatrix(G, localMatrix, elementSheme);

                //var v = GetV(nodes);
                //localMatrix = ((LinearRectangularCylindricalBasis)_basis).GetGradTMatrix(nodes, v);
                //localMatrix.MultiplyLocalMatrix(_problemService.Gamma(formulaNumber));
                //AddLocalMatrix(H, localMatrix, element);

                var localVector = _basis.GetLocalVector(element, _problemService.F, formulaNumber);
                AddLocalVector(vector, localVector, elementSheme);


            }//);

            Dictionary<string, Matrix> matrixes = new() { { "ColumnSize", M }, { "G", G }, { "H", H } };
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
            foreach (var element in _grid.SecondBoundaryNodes)
            {
                var nodes = _grid.GetFiniteElement(element);
                var res = _basis.GetSecondBoundaryVector(nodes, _problemService.SecondBoundaryFunction, element.FormulaNumber);
                AddLocalVector(slae.Vector, res, element);
            };
        }

        private void ConsiderThirdBoundaryConditions(Slae slae)
        {
            //Parallel.ForEach(_grid.ThirdBoundaryNodes, nodesIndexes =>
            //foreach (var nodesIndexes in _grid.ThirdBoundaryNodes)
            //{
            //    List<Node> nodes = new();
            //    for (int i = 0; i < nodesIndexes.Item1.Count; i++)
            //        nodes.Add(_grid.Nodes[nodesIndexes.Item1[i]]);

            //    int formulaNumber = nodesIndexes.Item2;
            //    var res = _basis.ConsiderThirdBoundaryCondition(slae, nodes, nodesIndexes.Item1, _problemService.ThridBoundaryFunction, formulaNumber);
            //    var elem = new FiniteElement(nodesIndexes.Item1.ToArray(), formulaNumber);
            //    AddLocalVector(slae.Vector, res.Item2, elem);
            //    AddLocalMatrix(slae.Matrix, res.Item1, elem);
            //}
        }
    }
}