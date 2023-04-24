using ReaserchPaper;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.CompilerServices;

namespace ResearchPaper
{
    class SLAU
    {
        public Matrix A;
        public Vector b, p;
        public Vector[] q;

        public SLAU(int nodesCount, int timeLayers)
        {
            A = new Matrix(nodesCount);
            b = new Vector(nodesCount);
            p = new Vector(nodesCount);
            q = new Vector[timeLayers];

            for (int i = 0; i < timeLayers; i++)
                q[i] = new Vector(nodesCount);
        }

        public double Size => A.Size;


        public void PrintResult(int timeLayer, bool isPrint)
        {
            Vector exactSolution = new Vector(q[0].Length);

            if (isPrint)
            {
                Console.WriteLine("      Численное решение      |         точное решение        |      разница решений      ");
                Console.WriteLine("--------------------------------------------------------------------------------------");

            }
            if (timeLayer == -1)
            {
              //  for (int i = 0; i < Grid.M; i++)
                 //   for (int j = 0; j < Grid.N; j++)
          //              exactSolution.Elements[i * Grid.N + j] = Master.Func1(Grid.x[j], Grid.y[i]);

                if (isPrint)
                    for (int i = 0; i < A.Size; i++)                  
                        Console.WriteLine("u{0} = {1:E16} | u*{0} = {2:E16} | {3:E16}", i + 1, p.Elements[i], exactSolution.Elements[i], Math.Abs(exactSolution.Elements[i] - p.Elements[i]));
                    
                Console.WriteLine("Относительная погрешность: " + GetSolveDifference(p, exactSolution));
            }
            else
            {              
             //   for (int i = 0; i < Grid.M; i++)
                //    for (int j = 0; j < Grid.N; j++)
                  //      exactSolution.Elements[i * Grid.N + j] = Master.Func2(Grid.x[j], Grid.y[i], Grid.t[timeLayer]);

                if (isPrint)
                    for (int i = 0; i < A.Size; i++)
                        Console.WriteLine("u{0} = {1:E16} | u*{0} = {2:E16} | {3:E16}", i + 1, q[timeLayer].Elements[i], exactSolution.Elements[i], Math.Abs(exactSolution.Elements[i] - q[timeLayer].Elements[i]));
                Console.WriteLine("--------------------------------------------------------------------------------------");
                Console.WriteLine("Относительная погрешность: " + GetSolveDifference(q[timeLayer], exactSolution));
            }
           
        }

        //void PrintSLAU()
        //{
        //    Console.WriteLine("--------------------");
        //    for (int i = 0; i < A.size; i++)
        //    {
        //        for (int j = 0; j < A.size; j++)
        //        {
        //            Console.Write(A[i][j] + "   ");
        //        }
        //        Console.WriteLine("    " + Master.Slau.b.Elements[i]);
        //    }
        //    Console.WriteLine("--------------------");
        //}

        public void WriteSolves()
        {
            using (StreamWriter sw = new StreamWriter("pressure.txt"))
                for (int j = 0; j < Grid.M; j++)
                    for (int i = 0; i < Grid.N; i++)                    
                        sw.WriteLine(Grid.x[i].ToString().Replace(",", ".") + " " + Grid.y[j].ToString().Replace(",", ".") +
                             " " + p.Elements[i*Grid.N + j].ToString().Replace(",", "."));

            using (StreamWriter sw = new StreamWriter("temperature.txt"))
            {
                sw.WriteLine(Size);
                sw.WriteLine(Grid.TimeLayersCount);
                for (int k = 0; k < Grid.TimeLayersCount; k++)
                    for (int j = 0; j < Grid.M; j++)
                       for (int i = 0; i < Grid.N; i++)
                          sw.WriteLine(Grid.x[i].ToString().Replace(",", ".") + " " + Grid.y[j].ToString().Replace(",", ".") +
                                " " + q[k].Elements[i * Grid.N + j].ToString().Replace(",", "."));
            }
        }

        public void Print()
        {
            double[][] matrix = A.Convert();
            for (int i = 0; i < matrix.Length; i++)
            {
                for (int j = 0; j < matrix.Length; j++)
                {
                    if (matrix[i][j] >= 0)
                        Console.Write(" ");
                    Console.Write(matrix[i][j].ToString("E2") + " ");
                }
                Console.Write(" " + b.Elements[i].ToString("E2"));
                Console.WriteLine();
            }
            Console.WriteLine("\n\n");
        }
        static double GetSolveDifference(Vector u, Vector uk)
        {
            Vector temp = new Vector(u.Length);

            for (int i = 0; i < u.Length; i++)
                temp.Elements[i] = u.Elements[i] - uk.Elements[i];

            return temp.GetNorm() / u.GetNorm();
        }
    }
}