using System;
using System.IO;

namespace HelloWorld
{ 
    class umf2
    {
        static double a, c, d, e, q = 2, lamda = 1, gamma = 1, eps = 1e-20, delta = 1e-33, result, w = 1,h;
        static int n = 0, m = 0, grid_type, iter_count = 0, max_iter_count = 10000;
        static double[] b, temp, b0, XW, YW, t;
        static double[][] A, B, L, U, u, uk;
        static int leftBoundary, rightBoundary;
        static int size;        
        static double max_diff;
        static double temper;
        static int[][] areas;
        static List<double> x,y,hy,hx;
        static List<int> IX, IY;
        static Dictionary<int, int> W; // структура которая ассоциирует глобальный номер узла и номер подобласти
            // static double[] Gx, Gy, Mx, My;


        static double Func(double x, double y) => x+y;
        static double DivFunc(double x, double y) => 1;
        static double F(double x, double y) =>  x+y;


        static double G(double h, int i, int j)
        {
            switch (i)
            {
                case 0:
                    switch (j)
                    {
                        case 0:
                            return 1 / h;
                        case 1:
                            return -1 / h;
                    }
                    break;
                case 1:
                    switch (j)
                    {
                        case 0:
                            return -1 / h;
                        case 1:
                            return 1 / h;
                    }
                    break;
            }
            return 0;
        }
        static double M(double h, int i, int j)
        {

            switch (i)
            {
                case 0:
                    switch (j)
                    {
                        case 0:
                            return 2 * h / 6;
                        case 1:
                            return h / 6;
                    }
                    break;
                case 1:
                    switch (j)
                    {
                        case 0:
                            return h / 6;
                        case 1:
                            return 2 * h / 6;
                    }
                    break;
            }
            return 0;
        }

        static void Main(string[] args)
        {
            #region just fem
            //ReadData();
            x = new List<double>();
            y = new List<double>();
            hx = new List<double>();
            hy = new List<double>();
            W = new Dictionary<int, int>();
            IX = new List<int>();
            IY = new List<int>();


            ReadData2();


            n = x.Count();
            m = y.Count();
            size = n * m;
            A = new double[size][];
            b = new double[size];
            for (int i = 0; i < size; i++)
            {
                A[i] = new double[size];
                for (int j = 0; j < size; j++)
                {
                    A[i][j] = 0;
                    b[i] = 0;
                }
            }


            for (int i = 0; i < areas.Length; i++) //по всем подобластям
            {
                Console.WriteLine(IX[areas[i][1]]);
                W.Add(IX[areas[i][0]] + IY[areas[i][2]] *n,i);//глобальный номер левой нижней точки
                W.Add(IX[areas[i][1]] + IY[areas[i][2]] * n-1, i);//номер х правой границы
                W.Add(IX[areas[i][0]] + IY[areas[i][3]] * n, i);//номер y вверхней границы
                W.Add(IX[areas[i][1]] + IY[areas[i][3]] * n-1, i);//номер y нижней границы
            }
            W.OrderBy(x => x.Key);


            u = new double[2][];
            uk = new double[2][];
            for (int i = 0; i < u.Length; i++)
            {
                u[i] = new double[size];
                uk[i] = new double[size];
            }
            
            temp = new double[size];
                       

            MakeGrid();
            MakeSLau();
            GetBoundaryConditions();
            PrintGrid();
            PrintTimeGrid();
            Console.WriteLine("\n--------------------------------------------------------------------\n");
         //   PrintSLAU();
            SolveSlau();
            PrintResult();
            #endregion
        }

        private static void PrintTimeGrid()
        {
            Console.WriteLine();
            for (int i = 0; i < t.Length; i++)
            {
                Console.Write(t[i] + " ");
            }
        }

        static double GetDiscrepancy(double[][] A, double[] u, double[] b)
        {
            double[] result = MultitplyMatrixOnVector(A, u);

            for (int i = 0; i < result.Length; i++)
            {
                //  Console.WriteLine(result[i]);
                result[i] -= b[i];
            }
            return GetNorm(result) / GetNorm(b);
        }

        static double GetSolveDifference(double[] u, double[] uk)
        {

            for (int i = 0; i < u.Length; i++)
            {
                temp[i] = u[i];
                temp[i] -= uk[i];
            }
            // Console.WriteLine(GetNorm(temp) / GetNorm(u));
            return GetNorm(temp) / GetNorm(u);
        }

        static double GetNorm(double[] vector)
        {
            result = 0;
            for (int i = 0; i < vector.Length; i++)
            {
                result += vector[i] * vector[i];
            }
            return Math.Sqrt(result);
        }

        static void ReadData()
        {
            using (StreamReader sr = new StreamReader("input.txt"))
            {
                string[] data = sr.ReadLine().Split(' ');
                a = double.Parse(data[0]);
                c = double.Parse(data[1]);
                n = int.Parse(data[2]);


                d = double.Parse(data[3]);
                e = double.Parse(data[4]);
                m = int.Parse(data[5]);

                size = n * m;

                grid_type = int.Parse(data[6]);
                leftBoundary = int.Parse(data[7]);
                rightBoundary = int.Parse(data[8]);
            }
        }


        static void ReadData2()
        {
            using (StreamReader sr = new StreamReader("domain.txt"))
            {
                string[] data = sr.ReadLine().Split(' ');
                n = int.Parse(data[0]);
                XW = new double[n];

                data = sr.ReadLine().Split(' ');
                for (int i = 0; i < n; i++)
                {
                    XW[i] = double.Parse(data[i]);
                }

                data = sr.ReadLine().Split(' ');
                m = int.Parse(data[0]);
                YW = new double[m];

                data = sr.ReadLine().Split(' ');
                for (int i = 0; i < m; i++)
                {
                    YW[i] = double.Parse(data[i]);
                }

                data = sr.ReadLine().Split(' ');
                areas = new int[int.Parse(data[0])][];
                for (int i = 0; i < areas.Length; i++)
                {
                    areas[i] = new int[4];
                }

                for (int i = 0; i < areas.Length; i++)
                {
                    data = sr.ReadLine().Split(' ');
                    for (int j = 0; j < 4; j++)
                    {
                        areas[i][j] = int.Parse(data[j]);
                    }

                }


            }

            using (StreamReader sr = new StreamReader("mesh.txt"))
            {
                string[] data = sr.ReadLine().Split(' ');
                x.Add(XW[0]);
                IX.Add(0);
                for (int i = 0; i < data.Length - 1; i += 2)
                {
                    n = int.Parse(data[i]);
                    q = double.Parse(data[i + 1]);
                    if (q == 1)
                        h = (XW[i / 2 + 1] - XW[i / 2]) / n;
                    else
                    {
                        h = (XW[i / 2 + 1] - XW[i / 2]) * (q - 1) / (Math.Pow(q, n - 1) - 1);
                        n--;
                    }
                    MakeArea(h, x, hx, n, q);
                    IX.Add(x.Count() - 1);
                }

                data = sr.ReadLine().Split(' ');
                y.Add(YW[0]);
                IY.Add(0);
                for (int i = 0; i < data.Length - 1; i += 2)
                {
                    n = int.Parse(data[i]);
                    q = double.Parse(data[i + 1]);
                    if (q == 1)
                        h = (YW[i / 2 + 1] - YW[i / 2]) / n;
                    else
                    {
                        h = (YW[i / 2 + 1] - YW[i / 2]) * (q - 1) / (Math.Pow(q, n - 1) - 1);
                        n--;
                    }
                    MakeArea(h, y, hy, n, q);
                    IY.Add(y.Count() - 1);
                }
            }

            using (StreamReader sr = new StreamReader("timeGrid.txt"))
            {
                string[] data = sr.ReadLine().Split(' ');
                int layersCount = int.Parse(data[0]);
                t = new double[layersCount];
                t[0] = double.Parse(data[1]);
                t[layersCount-1] = double.Parse(data[2]);
                q = double.Parse(data[3]);

                if (q == 1)
                    h = (t[layersCount-1] - t[0]) / n;
                else
                    h = (t[layersCount - 1] - t[0]) * (q - 1) / (Math.Pow(q, layersCount-1) - 1);

                for (int i = 1; i < layersCount-1; i++)
                    t[i] = t[i - 1] + h;
            }
        }

        static void MakeArea(double h, List<double> points, List<double> steps,int n, double q)//режим область на части и записываем в массив, h - шаг,  j - номер подобласти
        {
            int size = points.Count();
            for (int j = size; j < n+size; j++)
            {
                points.Add(points[j-1]+ h);
                steps.Add(h);
                h *= q;
            }
        }

        //                  y      x
        int GetAreaNumber(int _i, int _j)
        {
            for (int i = 0; i < areas.Length; i++)
            {
                if (IY[areas[i][2]] < _i && _i < IY[areas[i][3]])
                {
                    if (IY[areas[i][0]] < _j && _j < IY[areas[i][1]])
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        static void MakeGrid()
        {
            if (grid_type == 0)//фиксированный шаг
            {
                hx[0] = (c - a) / (n - 1);
                hy[0] = (e - d) / (m - 1);
                for (int i = 0; i < n; i++)
                {
                    x[i] = a + i * hx[0];
                    hx[i] = hx[0];
                }
                for (int i = 0; i < m; i++)
                {
                    y[i] = d + i * hy[0];
                    hy[i] = hy[0];
                }
            }
            else
            {
                hx[0] = (c - a) * (q - 1) / (Math.Pow(q, n - 1) - 1);
                hy[0] = (e - d) * (q - 1) / (Math.Pow(q, m - 1) - 1);
                x[0] = a;
                y[0] = d;
                for (int i = 1; i < n; i++)
                {
                    x[i] = x[i - 1] + hx[i - 1];
                   
                    hx[i] = q * hx[i - 1];
                }

                for (int i = 1; i < m; i++)
                {
                    y[i] = y[i - 1] + hy[i - 1];
                    hy[i] = q * hy[i - 1];
                }

            }
        }
        static int mu(int i) => ((i) % 2);
        static int nu(int i) => ((i) / 2);

        static double[][] GetMatrix(double hx, double hy)// m - номер кэ 
        {
            //матрица жесткости
            double[][] result = new double[4][];
            for (int i = 0; i < result.Length; i++)
                result[i] = new double[result.Length];

            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result.Length; j++)
                    result[i][j] = lamda* (G(hx, mu(i), mu(j)) * M(hy, nu(i), nu(j)) + M(hx, mu(i), mu(j)) * G(hy, nu(i), nu(j))); //матрица жётскости


            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result.Length; j++)
                    result[i][j] += gamma * M(hx, mu(i), mu(j)) * M(hy, nu(i), nu(j)); // матрица масс


            return result;
        }

        static double[][] GetMassMatrix(double hx, double hy)
        {
            double[][] result = new double[4][];
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result.Length; j++)
                    result[i][j] += gamma * M(hx, mu(i), mu(j)) * M(hy, nu(i), nu(j)); // матрица масс
            return result;
        }

        static void MakeSLau()
        {
            double[][] matrix;
            for (int j = 0; j < m-1; j++)
            {
                for (int i = 0; i < n-1; i++) // проходим по КЭ 
                {

                    matrix = GetMatrix(hx[i], hy[j]);

                    for (int k = 0; k < matrix.Length; k++)
                        for (int p = 0; p < matrix[0].Length; p++) 
                            A[i + j * n + k / 2 * n + k % 2][i + j * n + p / 2 * n + p % 2] += matrix[k][p];
                      

                    b[i + j * n] += (4 * F(x[i], y[j]) + 2 * F(x[i + 1], y[j]) + 2 * F(x[i], y[j + 1]) + F(x[i + 1], y[j + 1]))* hx[i] * hy[j] / 36;
                    b[i + j * n + 1] += (2 * F(x[i], y[j]) + 4 * F(x[i + 1], y[j]) + F(x[i], y[j + 1]) + 2 * F(x[i + 1], y[j + 1]))*hx[i] * hy[j] / 36;
                    b[i + (j+1) * n ] += (2 * F(x[i], y[j]) + F(x[i + 1], y[j]) + 4 * F(x[i], y[j + 1]) + 2 * F(x[i + 1], y[j + 1]))*hx[i] * hy[j] / 36;
                    b[i + (j+1) * n + 1] += (F(x[i], y[j]) + 2 * F(x[i + 1], y[j]) + 2 * F(x[i], y[j + 1]) + 4 * F(x[i + 1], y[j + 1]))*hx[i] * hy[j] / 36;


                }
            }
        }

        static void GetBoundaryConditions()
        {
            for (int i = 0; i < n; i++) //нижняя граница
            {
                for (int j = 0; j < A[0].Length; j++)
                {
                    A[i][j] = 0;
                }
                A[i][i] = 1;
                b[i] = Func(x[i], y[0]);
            }

            for (int i = A.Length-1; i > A.Length - m-1; i--)//верхняя граница
            {
                for (int j = 0; j < A[0].Length; j++)
                {
                    A[i][j] = 0;
                }
                A[i][i] = 1;
                b[i] = Func(x[i%n], y[m-1]);
            }

            for (int i = n; i < A.Length - m - 1; i+=n)//левая гравнь
            {
                for (int j = 0; j < A[0].Length; j++)
                {
                    A[i][j] = 0;
                }
                A[i][i] = 1;
                b[i] = Func(x[0], y[i/m]);
            }
            for (int i = 2*n-1; i < A.Length; i += n)//правая граница
            {
                for (int j = 0; j < A[0].Length; j++)
                {
                    A[i][j] = 0;
                }
                A[i][i] = 1;
                b[i] = Func(x[n-1], y[i/m]);
            }
        }
        static void SolveSlau()
        {
            ILU();
            ForwardStep();
            BackStep();
        }
        static void ForwardStep()
        {
            for (int i = 0; i < size; i++)
            {
                temp[i] = b[i];
                for (int j = 0; j < i; j++)
                {
                    temp[i] -= L[i][j] * temp[j];
                }
                temp[i] /= L[i][i];
            }
        }
        static void BackStep()
        {
            for (int i = size - 1; i >= 0; i--)
            {
                u[u.Length - 1][i] = temp[i];
                for (int j = size - 1; j > i; j--)
                {
                    u[u.Length-1][i] -= U[i][j] * u[u.Length - 1][j];
                }
                u[u.Length - 1][i] /= U[i][i];
            }
        }
        static void ILU()
        {
            L = new double[size][];
            U = new double[size][];

            for (int i = 0; i < size; i++)
            {
                L[i] = new double[size];

                U[i] = new double[size];
                for (int j = 0; j < size; j++)
                {
                    L[i][j] = 0;
                    L[i][i] = 1;
                    U[i][j] = 0;
                }
            }

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {

                    if (i > j)
                    {
                        L[i][j] = A[i][j];
                        for (int k = 0; k < j; k++)
                        {
                            L[i][j] -= L[i][k] * U[k][j];
                        }
                        L[i][j] /= U[j][j];
                    }
                    else //i < j
                    {
                        U[i][j] = A[i][j];
                        for (int k = 0; k < i; k++)
                        {
                            U[i][j] -= L[i][k] * U[k][j];
                        }
                    }
                }
            }
        }
        static void PrintGrid()
        {
            for (int j = 0; j < m; j++)
            {
                for (int i = 0; i < n; i++)
                {

                    Console.Write(" [{0}, {1}] ", x[i].ToString("e2"), y[j].ToString("e2"));
                }
                Console.WriteLine("\n");
            }
        }
        static void PrintSLAU()
        {
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    Console.Write(A[i][j] + "   ");
                }
                Console.WriteLine("    " + b[i]);
            }
        }
        static void PrintResult()
        {

            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    uk[uk.Length - 1][i * n + j] = Func(x[j],y[i]);
                }
            }

            for (int i = 0; i < size; i++)
            {

                //      Console.WriteLine("u{0} = {1} | u*{0} = {3} | {4}", i + 1, u[i],i+1,uk[i],u[i]-uk[i]);
                Console.WriteLine("u{0} = {1}", i + 1, u[u.Length - 1][i]);
            }
            Console.WriteLine("Относительная погрешность: " + GetSolveDifference(u[u.Length-1], uk[uk.Length-1]));
        }
        static double[] MultitplyMatrixOnVector(double[][] A, double[] u)
        {
            if (A.Length != u.Length)
            {
                throw new ArgumentException("Size of matrix isn't equals to size of vector");
            }

            double[] result = new double[u.Length];

            for (int i = 0; i < A.Length; i++)
            {
                result[i] = 0;
                for (int j = 0; j < A.Length; j++)
                {
                    result[i] += A[i][j] * u[j];
                }
            }
            return result;
        }
    }
}