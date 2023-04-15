using ReaserchPaper;
using System.Security.Principal;

namespace ResearchPaper
{
    class Master
    {
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
            Grid.PrintPartialGrid();
            Grid.PrintTimeGrid();

            Slau = new SLAU(Grid.NodesCount, Grid.TimeLayersCount);
            Collector collector = new(Grid.NodesCount);

            collector.Collect();
            Slau.p = solver.Solve(Slau.b, Slau.A);

            Slau.PrintResult(-1, false);
            //     GetV(1, 1, 2, 1, 2, 1.0694318442029738, 0.06943184420297371, 1, 1).Print();
            collector.GetMatrixH();
            //   GetV(4,1,2,1,2,1.5,1.5,1,1).Print();

            for (int i = 2; i < Grid.TimeLayersCount; i++)
            {
                collector.Collect(i);
                //   Slau.Print();
                Slau.q[i] = solver.Solve(Slau.b, Slau.A);
                //    Console.WriteLine("solving in proccess: {0} of {1} time layers...", i+1, Grid.TimeLayersCount);
            }
            Slau.PrintResult(2, true);
            sw.Stop();
            //  Console.WriteLine(sw.ElapsedMilliseconds);
        }
    }
}