using Grid.Enum;
using Grid.Models;

namespace Grid.Factories
{
    internal static class BaseMethods
    {
        internal static int GetAreaNumber(Area<double>[] subDomains, Node node, GridDimensional dimensional)
        {
            return dimensional switch
            {
                GridDimensional.One => GetAreaNumber1D(subDomains, node),
                GridDimensional.Two => GetAreaNumber2D(subDomains, node),
                GridDimensional.Three => GetAreaNumber3D(subDomains, node),
                _ => throw new ArgumentException($"Invalid argument - {dimensional}!")
            };
        }

        private static int GetAreaNumber1D(Area<double>[] subDomains, Node node)
        {
            for (int i = subDomains.Length - 1; i > -1; i--)//идём в обратном порядке чтобы не было бага, когда в качестве подобласти возвращается 0 (0 - вся расчётная область, которая уже вкл. подобласти)              
                if (subDomains[i].XLeft <= node.X && node.X <= subDomains[i].XRight)
                    return i;
            return -1;
        }

        private static int GetAreaNumber2D(Area<double>[] subDomains, Node node)
        {
            for (int i = subDomains.Length - 1; i > -1; i--)//идём в обратном порядке чтобы не было бага, когда в качестве подобласти возвращается 0 (0 - вся расчётная область, которая уже вкл. подобласти)
                if (subDomains[i].YBottom <= node.Y && node.Y <= subDomains[i].YTop)
                    if (subDomains[i].XLeft <= node.X && node.X <= subDomains[i].XRight)
                        return i;
            return -1;
        }

        private static int GetAreaNumber3D(Area<double>[] subDomains, Node node)
        {
            for (int i = subDomains.Length - 1; i > -1; i--)//идём в обратном порядке чтобы не было бага, когда в качестве подобласти возвращается 0 (0 - вся расчётная область, которая уже вкл. подобласти)
                if (subDomains[i].ZBack <= node.Z && node.Z <= subDomains[i].ZFront)
                    if (subDomains[i].YBottom <= node.Y && node.Y <= subDomains[i].YTop)
                        if (subDomains[i].XLeft <= node.X && node.X <= subDomains[i].XRight)
                            return i;
            return -1;
        }
    }
}