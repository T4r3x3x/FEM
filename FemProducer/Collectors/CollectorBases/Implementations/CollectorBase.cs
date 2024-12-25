using FemProducer.Basises.Abstractions;
using FemProducer.Collectors.CollectorBases.Interfaces;
using FemProducer.Services;

using Grid.Models;

using MathModels.Models;

using Tools;

namespace FemProducer.Collectors.CollectorBases.Implementations;

public class CollectorBase : ICollectorBase
{
    private readonly GridModel _grid;
    private readonly MatrixFactory.MatrixFactory _matrixFactory;
    private readonly ProblemService _problemService;
    private readonly object _lock = new object();
    private readonly AbstractBasis _basis;

    public CollectorBase(GridModel grid, MatrixFactory.MatrixFactory matrixFactory, ProblemService problemService, AbstractBasis basis)
    {
        _grid = grid;
        _matrixFactory = matrixFactory;
        _problemService = problemService;
        _basis = basis;
    }

    public (Dictionary<string, Matrix>, Vector) Collect() => GetMatricesMG();

    private (Dictionary<string, Matrix>, Vector) GetMatricesMG()
    {
        var M = _matrixFactory.CreateMatrix(_grid);
        var G = _matrixFactory.CreateMatrix(_grid);
        var H = _matrixFactory.CreateMatrix(_grid);
        var vector = new Vector(M.Size);

        Parallel.ForEach(_grid.Elements, elementSheme =>
            //foreach (var elementSheme in _grid.Elements)
        {
            var formulaNumber = elementSheme.FormulaNumber;
            var element = _grid.GetFiniteElement(elementSheme);

            var massMatrix = _basis.GetMassMatrix(element);
            var stiffnessMatrix = _basis.GetStiffnessMatrix(element); //todo для обычного мкэ должна быть матрица масс
            var localVector = _basis.GetLocalVector(element, _problemService.F, formulaNumber, stiffnessMatrix);

            massMatrix.MultiplyLocalMatrix(_problemService.Gamma(formulaNumber));
            AddLocalMatrix(M, massMatrix, elementSheme);

            stiffnessMatrix.MultiplyLocalMatrix(_problemService.Lambda(formulaNumber));
            AddLocalMatrix(G, stiffnessMatrix, elementSheme);

            for (var i = 0; i < localVector.Count; i++)
                localVector[i] *= _problemService.LambdaRight(formulaNumber);

            AddLocalVector(vector, localVector, elementSheme);
        });

        var matrixes = new Dictionary<string, Matrix>
        {
            {
                "_M", M
            },
            {
                "_G", G
            },
            {
                "H", H
            }
        };
        return (matrixes, vector);
    }

    private void AddLocalVector(Vector vector, IList<double> localVector, FiniteElementScheme element)
    {
        lock (_lock)
        {
            for (var i = 0; i < localVector.Count; i++)
                vector[element.NodesIndexes[i]] += localVector[i];
        }
    }

    private void AddLocalMatrix(Matrix matrix, IList<IList<double>> localMatrix, FiniteElementScheme element)
    {
        lock (_lock)
        {
            for (var i = 0; i < element.NodesIndexes.Length; i++)
            {
                var ibeg = element.NodesIndexes[i];
                matrix.Di[ibeg] += localMatrix[i][i];
                for (var j = i + 1; j < element.NodesIndexes.Length; j++)
                {
                    var iend = element.NodesIndexes[j];
                    var inewIbeg = ibeg;

                    if (inewIbeg < iend)
                        (iend, inewIbeg) = (inewIbeg, iend);

                    var h = matrix.Ia[inewIbeg];
                    while (matrix.Ja[h++] - iend != 0)
                    {
                    }
                    h--;
                    matrix.Al[h] += localMatrix[i][j];
                    matrix.Au[h] += localMatrix[j][i];
                }
            }
        }
    }

    public void GetBoundaryConditions(Slae slae)
    {
        ConsiderSecondBoundaryConditions(slae);
        ConsiderThirdBoundaryConditions(slae);
        ConsiderFirstBoundaryConditions(slae);

        slae.Vector[0] = 10;
    }


    private void ConsiderFirstBoundaryConditions(Slae slae) => Parallel.ForEach(_grid.FirstBoundaryNodes, boundaryNodeIndex =>
    {
        var area = _grid.GetSubDomain(_grid.Nodes[boundaryNodeIndex]);
        SetFirstBoundaryCondition(slae, boundaryNodeIndex, area);
    });

    private void SetFirstBoundaryCondition(Slae slae, int boundaryNodeIndex, int area)
    {
        slae.Matrix.ZeroingRow(boundaryNodeIndex);
        slae.Matrix.Di[boundaryNodeIndex] = 1;
        slae.Vector[boundaryNodeIndex] = _problemService.Function(_grid.Nodes[boundaryNodeIndex], area);
    }

    private void ConsiderSecondBoundaryConditions(Slae slae) => Parallel.ForEach(_grid.SecondBoundaryNodes, scheme =>
    {
        var finiteElement = _grid.GetFiniteElement(scheme);
        var res = _basis.GetSecondBoundaryData(finiteElement, _problemService.SecondBoundaryFunction, scheme.FormulaNumber);
        AddLocalVector(slae.Vector, res, scheme);
    });

    private void ConsiderThirdBoundaryConditions(Slae slae) => Parallel.ForEach(_grid.ThirdBoundaryNodes, scheme =>
    {
        var finiteElement = _grid.GetFiniteElement(scheme);
        var res = _basis.GetThirdBoundaryData(slae, finiteElement, _problemService.ThirdBoundaryFunction, scheme.FormulaNumber);
        AddLocalMatrix(slae.Matrix, res.matrix, scheme);
        AddLocalVector(slae.Vector, res.vector, scheme);
    });
}