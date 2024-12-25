using FemProducer.Models;

using Grid.Models;

using MathModels.Models;

namespace FemProducer.Services;

public class BaseProblemService(ProblemParameters problemParameters) : ProblemService(problemParameters)
{
    public override double Lambda(int formulaNumber) => problemParameters.Lambda[0];
    public override double LambdaRight(int formulaNumber) => problemParameters.Lambda[0];
}