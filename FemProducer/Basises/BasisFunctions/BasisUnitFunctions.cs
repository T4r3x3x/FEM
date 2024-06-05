namespace FemProducer.Basises.BasisFunctions
{
    public static class BasisUnitFunctions
    {
        public static List<Func<double, double>> W = [
            (a) => 1 - a,
            (a) => a,
        ];
        public static List<Func<double>> WDiv = [
            () => -1,
            () => 1,
        ];
    }
}