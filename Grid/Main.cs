﻿using ReaserchPaper;
using System.Diagnostics;
using System.Security.Principal;

namespace ResearchPaper
{
    class Master
    {
        static double ro = 730, fita = 1.7e+3, K, nu,mu;

        public static double Lamda = 0.2 * 0.4 / 0.005, Gamma = 0, Sigma = 1;//Sigma в массе масс вместо гаммы для времени     
        public static double Lamda2 = 0.01;
        public static SLAU Slau;
        public static int[] boundaryConditions = new int[4] {1,1,1,1};
        public static int[] borehole = new int[4] { 22, 23, 22, 23 }; //индексы x0 x1 y0 y1

        public static double PressuereInReservoir(double x, double y) => 13172250;
        public static double BoreholePower() => +6.9e-4;
        public static double F1(double x, double y) => 0;

        public static double TemperatureInReservoir() => 0;
        public static double TemperatureInBorehole() => 10;
        public static double F2(double x, double y, double t) => 0;


        static void ExecuteCommand(string command)
        {
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = "C:\\Python\\python.exe";
            start.Arguments = string.Format("C:\\Users\\hardb\\source\\repos\\Grid\\Grid\\bin\\Debug\\net6.0\\grid.py");
            start.UseShellExecute = false;
            start.RedirectStandardOutput = true;
            Process.Start(start);
            start.Arguments = string.Format("C:\\Users\\hardb\\source\\repos\\Grid\\Grid\\bin\\Debug\\net6.0\\pressure.py");
            Process.Start(start);
            start.Arguments = string.Format("C:\\Users\\hardb\\source\\repos\\Grid\\Grid\\bin\\Debug\\net6.0\\temperature.py");
            Process.Start(start);
        }

        static void Main(string[] args)
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
          
            Solver solver = new();
            Grid.ReadData();
            //  Grid.PrintPartialGrid();
            //   Grid.PrintTimeGrid();
            Grid.WriteGrid();
            Slau = new SLAU(Grid.NodesCount, Grid.TimeLayersCount);
            Collector collector = new(Grid.NodesCount);
            collector.Collect();
            Slau.p = solver.Solve(Slau.A, Slau.b);
           // Master.Slau.Print();
           // Slau.PrintResult(-1, true);
            collector.GetMatrixH();
           collector.RebuildMatrix();
            boundaryConditions = new int[4] { 2,2,2,2};



            for (int i = 2; i < Grid.TimeLayersCount; i++)
            {
                collector.Collect(i);
                ;
                Slau.q[i] = solver.Solve(Slau.A, Slau.b);
                Console.WriteLine("solving in proccess: {0} of {1} time layers...", i + 1, Grid.TimeLayersCount);
            }
           // Slau.PrintResult(1, false);

            Slau.WriteSolves();
            Console.WriteLine(sw.ElapsedMilliseconds);
            ExecuteCommand("python func.py");
        }
    }
}