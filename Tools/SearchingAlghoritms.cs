namespace Tools;

public static class SearchingAlghoritms
{
    public static int BinarySearch(IList<int> list, int value, int l, int r)
    {
        while (l < r)
        {
            var mid = (l + r) / 2 + 1;

            if (list[mid] > value)
                r = mid - 1;
            else
                l = mid;
        }

        return list[l] == value ? l : -1;
    }

    public static double GetMinValueInCollection(IEnumerable<double> collection)
    {
        var min = double.MaxValue;
        foreach (var item in collection)
            if (item < min)
                min = item;

        return min;
    }

    public static double GetMaxValueInCollection(IEnumerable<double> collection)
    {
        var max = double.MinValue;
        foreach (var item in collection)
            if (item > max)
                max = item;

        return max;
    }
}