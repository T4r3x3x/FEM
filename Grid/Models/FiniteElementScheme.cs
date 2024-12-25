using Grid.Enum;

namespace Grid.Models;

public class FiniteElementScheme
{
    public int[] NodesIndexes { get; }
    public int FormulaNumber;
    public AxisOrientation Section { get; }

    public FiniteElementScheme(int[] nodesIndexes, int formulaNumber, AxisOrientation section)
    {
        NodesIndexes = nodesIndexes;
        FormulaNumber = formulaNumber;
        Section = section;
    }
}