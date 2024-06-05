using Grid.Models;

namespace Grid.Factories
{
    internal static class BaseMethods
    {
        internal static int GetAreaNumber(Area<double>[] subDomains, Node node)
        {
            for (int i = subDomains.Length - 1; i > -1; i--)//идём в обратном порядке чтобы не было бага, когда в качестве подобласти возвращается 0 (0 - вся расчётная область, которая уже вкл. подобласти)
                if (subDomains[i].YBottom <= node.Y && node.Y <= subDomains[i].YTop)
                    if (subDomains[i].XLeft <= node.X && node.X <= subDomains[i].XRight)
                        return i;
            return -1;
        }
    }
}