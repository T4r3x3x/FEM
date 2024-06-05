using Grid.Factories.ElementFactory.Interfaces;
using Grid.Factories.GridFactory.Interfaces;
using Grid.Factories.NodeFactory.Interfaces;
using Grid.Models;
using Grid.Models.InputModels;

using MathModels;

using static Grid.Factories.AxisFactory;

namespace Grid.Factories.GridFactory.Implementations
{
    public class GridFactory : IGridFactory
    {
        private readonly int _countOfNodesInElement;
        private readonly AbstractElementFactory _elementFactory;
        private readonly INodeFactory _nodeFactory;
        private readonly BoundaryFactory _boundaryFactory;

        public GridFactory(int countOfNodesInElement, AbstractElementFactory elementFactory, INodeFactory nodeFactory, BoundaryFactory boundaryFactory)
        {
            _countOfNodesInElement = countOfNodesInElement;
            _elementFactory = elementFactory;
            _nodeFactory = nodeFactory;
            _boundaryFactory = boundaryFactory;
        }

        public GridModel GetGrid(GridInputParameters @params)
        {
            (var coordinates, var t) = GetAxisesToPoints(@params);
            var subDomainsBoundaries = GetSubDomainsBoudaries(@params.ZW, @params.LinesNodes, @params.Areas);
            (var nodes, var missingNodeses) = _nodeFactory.GetNodes(@params.Areas, subDomainsBoundaries, @params.LinesNodes, coordinates);
            var elements = _elementFactory.GetElements(coordinates, subDomainsBoundaries, missingNodeses);
            var boundaryNodes = _boundaryFactory.GetBoundaryNodes(@params.Areas, @params.BoundaryConditions, coordinates, @params.XParams.SplitsCount,
                @params.YParams.SplitsCount, @params.ZParams.SplitsCount, missingNodeses.ToArray());

            var realSubdomains = GetRealSubdomians(@params.LinesNodes, @params.Areas);
            //сделать метод, который будет выдавать количество узлов в элементе
            return new GridModel(elements, nodes, boundaryNodes.Item1, [], [], realSubdomains, 8, coordinates.X, coordinates.Y, coordinates.Z, @params.Areas, t);
        }

        /// <summary></summary>
        /// <param name="ZW"></param>
        /// <param name="lines"></param>
        /// <param name="areas"></param>
        /// <returns>границы подобластей в виде значений по х,у,z</returns>
        private static Area<double>[] GetSubDomainsBoudaries(double[] ZW, Point[][] lines, Area<int>[] areas)//+
        {
            var subDomains = new Area<double>[areas.Length];
            for (int i = 0; i < subDomains.Length; i++)
            {
                var area = areas[i];
                subDomains[i] = new Area<double>(
                    lines[area.YBottom][area.XLeft].X,
                    lines[area.YTop][area.XRight].X,
                    lines[area.YBottom][area.XRight].Y,
                    lines[area.YTop][area.XRight].Y,
                    ZW[area.ZBack],
                    ZW[area.ZFront],
                    area.FormulaNumber);
            }
            return subDomains;
        }

        private List<Point[]> GetRealSubdomians(Point[][] lines, Area<int>[] areas)//+
        {
            List<Point[]> subdomains = new();
            for (int i = 0; i < areas.Length; i++)
            {
                var area = areas[i];
                Point[] points = [lines[area.YBottom][area.XLeft],
                    lines[area.YBottom][area.XRight],
                    lines[area.YTop][area.XLeft],
                    lines[area.YTop][area.XRight]];
                subdomains.Add(points);
            }
            return subdomains;
        }
    }
}