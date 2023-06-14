using ReaserchPaper;
using System.Diagnostics;
using System.Security.Principal;

namespace ResearchPaper
{
    class Master
    {
        static double ro, fita, K, nu,mu;
       
        public static double Lamda = 1, Gamma = 1, Sigma = 2;//Sigma в массе масс вместо гаммы для времени     
        public static SLAU Slau;
        public static int[] boundaryConditions = new int[4] {1,1,1,1};
        

        public static double Func1(double x, double y, int area)
        {
            switch (area)
            {
                case 0: return x + y;
                case 1: return x - y;
                default: return 0;
            }
        }

        public static double DivFuncX1(double x, double y, int area)
        {
            switch (area)
            {
                default: return 0;
            }
        }
        public static double DivFuncY1(double x, double y, int area)
        {
            switch (area)
            {
                default: return 0;
            }
        }
        public static double F1(double x, double y, int area)
        {
            switch (area)
            {
                case 0: return x + y;
                case 1: return x - y;
                default: return 0;
            }
        }


        public static double Func2(double x, double y, double t, int area)
        {
            switch (area)
            {
                case 0: return -x * y * t;
                case 1: return x * y * t;
                default: return x * y * t;
            }
        }
        public static double DivFuncX2(double x, double y, double t, int area) {
            switch (area)
            {
                default:
                    return y * t;
            }
        }
        public static double DivFuncY2(double x, double y, double t, int area) 
        {
            switch (area)
            {
                default:
                    return x * t;
            }
        }
        public static double F2(double x, double y, double t, int area)
        {
            switch (area)
            {
                case 0: return - 2 * x * y + y * t + x * t;
                case 1: return 2 * x * y - y * t + x * t;
                default:
                    return 2 * x * y - x * t + y * t;
            }
        }


        static void ExecuteCommand(string command)
        {
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = "C:\\Python\\python.exe";
            start.Arguments = string.Format("C:\\Users\\hardb\\source\\repos\\Grid\\Grid\\bin\\Debug\\net6.0\\grid.py");
            start.UseShellExecute = false;
            start.RedirectStandardOutput = true;
            Process.Start(start);
            //start.Arguments = string.Format("C:\\Users\\hardb\\source\\repos\\Grid\\Grid\\bin\\Debug\\net6.0\\pressure.py");
            //Process.Start(start);
            //start.Arguments = string.Format("C:\\Users\\hardb\\source\\repos\\Grid\\Grid\\bin\\Debug\\net6.0\\temperature.py");
            //Process.Start(start);
        }

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
          //  Master.Slau.Print();

            Slau.PrintResult(-1, false);
            collector.GetMatrixH();
            collector.SwitchTask(solver);
      
            for (int i = 2; i < Grid.TimeLayersCount; i++)
            {
                collector.Collect(i);
                //  ;
                Slau.q[i] = solver.Solve(Slau.A, Slau.b);
                //Console.WriteLine("solving in proccess: {0} of {1} time layers...", i+1, Grid.TimeLayersCount);
                Slau.PrintResult(i, false);
            }
       //     Slau.PrintResult(Grid.TimeLayersCount - 1, true);
            sw.Stop();
            Slau.WriteSolves();
            Console.WriteLine(sw.ElapsedMilliseconds);
            ExecuteCommand("python func.py");
        }
    }
}