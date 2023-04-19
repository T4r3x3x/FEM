using ReaserchPaper;
using System.Security.Principal;

namespace ResearchPaper
{
    class Master
    {
        static double ro, fita, K, nu,mu;
       
        public static double Lamda = 1, Gamma = 1, Sigma = 1;//Sigma в массе масс вместо гаммы для времени     
        public static SLAU Slau;
        public static int[] boundaryConditions = new int[4] {1,1,1,1};


        public static double Func1(double x, double y) => x+y;
        public static double DivFuncX1(double x, double y) => 0;
        public static double DivFuncY1(double x, double y) => 0;
        public static double F1(double x, double y) => x + y;


        public static double Func2(double x, double y, double t) => x*x+y*y + t;
        public static double DivFuncX2(double x, double y, double t) => y * t;
        public static double DivFuncY2(double x, double y, double t) => x * t;
        public static double F2(double x, double y, double t) => -3 - 2* x - 2 * y;


    


        static void Main(string[] args)
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            Solver solver = new();
            Grid.ReadData();
            //  Grid.PrintPartialGrid();
            //   Grid.PrintTimeGrid();
            Grid.WriteGrid();
            Slau = new SLAU(Grid.NodesCount, Grid.TimeLayersCount);
            Collector collector = new(Grid.NodesCount);

            collector.Collect();
            Slau.p = solver.Solve(Slau.A, Slau.b);
            Slau.PrintResult(-1, false);
            collector.GetMatrixH();
            collector.RebuildMatrix();
            
            for (int i = 2; i < Grid.TimeLayersCount; i++)
            {
                collector.Collect(i);
                //  ;
                Slau.q[i] = solver.Solve(Slau.A, Slau.b);
                //Console.WriteLine("solving in proccess: {0} of {1} time layers...", i+1, Grid.TimeLayersCount);
            }
            Slau.PrintResult(2, false);
            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds);
        }
    }
}