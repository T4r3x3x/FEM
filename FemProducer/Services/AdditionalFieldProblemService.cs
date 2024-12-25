using FemProducer.Models;

using Grid.Models;

namespace FemProducer.Services;

public class AdditionalFieldProblemService(ProblemParameters problemParameters) : ProblemService(problemParameters)
{
    private readonly ProblemParameters _problemParameters1 = problemParameters;
    public override double Lambda(int formulaNumber) => _problemParameters1.Lambda[formulaNumber];
    public override double LambdaRight(int formulaNumber) => -(_problemParameters1.Lambda[0] - _problemParameters1.Lambda[formulaNumber]);
    public override double F(Node node, int formulaNumber) => Q[NodesIndexes[node]];
}