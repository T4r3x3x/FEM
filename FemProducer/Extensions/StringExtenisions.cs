namespace FemProducer.Extensions
{
    public static class StringExtenisions
    {
        public static string? ToString(this double obj)
        {
            return obj.ToString()!.Replace(",", ".");
        }
    }
}
