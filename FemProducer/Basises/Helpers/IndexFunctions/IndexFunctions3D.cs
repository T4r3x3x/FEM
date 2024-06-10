namespace FemProducer.Basises.Helpers.IndexFunctions
{
    public static class IndexFunctions3D
    {
        public static int Mu(int i) => i % 2;
        public static int Nu(int i) => i / 2 % 2;
        public static int Eps(int i) => i / 4;
    }
}
