namespace FemProducer.Extensions
{
    public static class StringExtenisions
    {
        public static string? ToString(this object obj)
        {
            return obj.ToString()!.Replace(",", ".");
        }
    }
}
