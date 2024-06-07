using FemProducer.Services;

using Grid.Enum;
using Grid.Models;

namespace FemProducer.Basises
{
    public abstract class Abstract2DBasis : AbstractBasis
    {
        public Section2D Section { get; set; }

        protected Abstract2DBasis(ProblemService problemService, Section2D section = Section2D.XY) : base(problemService)
        {
            Section = section;
        }

        /// <summary </summary>
        /// <param name="nodes"></param>
        /// <returns>возвращает шаги по конечному элементу, согласно сечению</returns>
        protected (double, double) GetSteps2D(IList<Node> nodes)
        {
            return Section switch
            {
                Section2D.XY => (GetXStep(nodes), GetYStep(nodes)),
                Section2D.XZ => (GetXStep(nodes), nodes[2].Z - nodes[0].Z),
                Section2D.YZ => (nodes[1].Y - nodes[0].Y, nodes[2].Z - nodes[0].Z),
                _ => throw new ArgumentException("Invalid argument!")
            };
        }
    }
}
