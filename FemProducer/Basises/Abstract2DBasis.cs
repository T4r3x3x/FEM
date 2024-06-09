using FemProducer.Services;

using Grid.Enum;

namespace FemProducer.Basises
{
    public abstract class Abstract2DBasis : AbstractBasis
    {
        public Section2D Section { get; set; }

        protected Abstract2DBasis(ProblemService problemService, Section2D section = Section2D.XY) : base(problemService)
        {
            Section = section;
        }

    }
}
