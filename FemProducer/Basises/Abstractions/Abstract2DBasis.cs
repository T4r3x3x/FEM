using FemProducer.Services;

using Grid.Enum;

namespace FemProducer.Basises.Abstractions
{
    public abstract class Abstract2DBasis : AbstractBasis
    {
        public AxisOrientation Section { get; set; }

        protected Abstract2DBasis(ProblemService problemService, AxisOrientation section = AxisOrientation.XY) : base(problemService) => Section = section;
    }
}
