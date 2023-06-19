using ReaserchPaper;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace ResearchPaper
{
    class Master
    {
        public static SLAU Slau;

        public static double PressuereInReservoir(double x, double y) => 13172250;
        public static double BoreholePower() => +6.9e-4;
        public static double F1(double x, double y) => 0;


        public static double TemperatureAtBegin() => 20;
        public static double TemperatureAtBoundary() => 20;
        public static double TemperatureInBorehole() => 100;
        public static double F2(double x, double y, double t) => 0;


        public static void PrintB()
        {
            for (int i = 0; i < Slau.Size; i++)
            {
                Console.WriteLine(Slau.b.Elements[i]);
            }
        }


        static void DrawGrid()
        {
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = "C:\\Python\\python.exe";
            start.Arguments = string.Format("C:\\Users\\hardb\\source\\repos\\Grid\\Grid\\bin\\Debug\\net6.0\\grid.py");
            start.UseShellExecute = false;
            start.RedirectStandardOutput = true;
            Process.Start(start);
        }

        static void DrawPressure()
        {
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = "C:\\Python\\python.exe";
            start.Arguments = string.Format("C:\\Users\\hardb\\source\\repos\\Grid\\Grid\\bin\\Debug\\net6.0\\pressure.py");
            start.UseShellExecute = false;
            start.RedirectStandardOutput = true;
            Process.Start(start);
        }
        static void DrawTemperature(string command)
        {
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = "C:\\Python\\python.exe";
            start.Arguments = string.Format("C:\\Users\\hardb\\source\\repos\\Grid\\Grid\\bin\\Debug\\net6.0\\temperature.py");
            start.UseShellExecute = false;
            start.RedirectStandardOutput = true;
            Process.Start(start);
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
            DrawGrid();
            Slau = new SLAU(Grid.NodesCount, Grid.TimeLayersCount);
            Collector collector = new(Grid.NodesCount);
            collector.Collect();
            Slau.p = solver.Solve(Slau.A, Slau.b);
            
           // Master.Slau.Print();
           // Slau.PrintResult(-1, true);
            collector.GetMatrixH();
            collector.SwitchTask(solver);

         
            for (int i = 2; i < Grid.TimeLayersCount; i++)
            {
        //        PrintB();
                collector.Collect(i);
                //           PrintB();
            //Slau.Print();
                Slau.q[i] = solver.Solve(Slau.A, Slau.b);
                Console.WriteLine("solving in proccess: {0} of {1} time layers...", i + 1, Grid.TimeLayersCount);
            }
            //  Slau.Print();
            //Slau.PrintResult(1, false);
            double max = 0;
            for (int i = 0; i < Grid.NodesCount; i++)
            {
                if(max < Slau.p.Elements[i])
                    max = Slau.p.Elements[i];
            }
            Console.WriteLine(max);
            Slau.WriteSolves();
            DrawPressure();
            Console.WriteLine(sw.ElapsedMilliseconds);
            DrawTemperature("python func.py");
        }
    }
}