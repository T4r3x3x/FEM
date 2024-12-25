using FemProducer.Models;

using Grid.Models;

using MathModels.Models;

namespace FemProducer.Services;

public class ProblemService(ProblemParameters problemParameters)
{
    /// <summary>
    /// Решение базовой задачи
    /// </summary>
    public Vector Q { get; set; }

    public Dictionary<Node, int> NodesIndexes { get; set; }

    /// <summary>
    /// Мощности источников
    /// </summary>
    /// <param name="formulaNumber"></param>
    /// <returns></returns>
    public double SourcePowers(int formulaNumber) => problemParameters.SourcePowers[formulaNumber];

    public virtual double Lambda(int formulaNumber)
    {
        if (formulaNumber > problemParameters.Lambda.Count - 1)
            throw new ArgumentException($"Лямбда для формулы {formulaNumber} не задана!");

        return problemParameters.Lambda[formulaNumber];
    }

    public double Gamma(int formulaNumber)
    {
        if (formulaNumber > problemParameters.Gamma.Count - 1)
            throw new ArgumentException($"Гамма для формулы {formulaNumber} не задана!");

        return problemParameters.Gamma[formulaNumber];
    }

    public virtual double LambdaRight(int formulaNumber)
    {
        if (formulaNumber > problemParameters.Gamma.Count - 1)
            throw new ArgumentException($"Гамма для формулы {formulaNumber} не задана!");

        return problemParameters.Lambda[formulaNumber];
    }

    public double Function(Node node, int area)
    {
        var x = node.X;
        var y = node.Y;
        var z = node.Z;

        return area switch
        {
            _ => x

            //_ => throw new ArgumentException(),
        };
    }

    public double SourceFunction(Node node, int formulaNumber) => SourcePowers(formulaNumber);

    public double SecondBoundaryFunction(Node node, int formulaNumber)
    {
        var x = node.X;
        var y = node.Y;
        var z = node.Z;
        return Lambda(formulaNumber) * formulaNumber switch
        {
            0 => -y * z,
            1 => x * z,
            2 => y * z,
            3 => -x * z,
            4 => -y * x,
            5 => y * x,
            _ => throw new ArgumentException($"Гамма для формулы {formulaNumber} не задана!")
        };
    }

    public double ThirdBoundaryFunction(Node node, int formulaNumber)
    {
        var x = node.X;
        var y = node.Y;
        var z = node.Z;
        return Function(node, formulaNumber) + SecondBoundaryFunction(node, formulaNumber);
        //return formulaNumber switch
        //{
        //    2 => Function(node, formulaNumber) + SecondBoundaryFunction(node, formulaNumber),
        //    1 => Function(node, formulaNumber) + SecondBoundaryFunction(node, formulaNumber),

        //    _ => throw new ArgumentException(),
        //};
    }

    public virtual double F(Node node, int formulaNumber)
    {
        var x = node.X;
        var y = node.Y;
        var z = node.Z;
        return formulaNumber switch
        {
            _ => Gamma(formulaNumber) * Function(node, formulaNumber)
            //_ => throw new ArgumentException(),
        };
    }
}