using ResearchPaper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReaserchPaper
{
    internal class Collector
    {
        static Matrix _M, _G, _H;
        static double timeCoef;

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

        public Collector(int nodesCount)
        {
            _M = new Matrix(nodesCount);
            _G = new Matrix(nodesCount);
            _H = new Matrix(nodesCount);
            GetMatrixesMG();
            //        _H.Print();
            GetTimeConditions();
            //  _M.Print();
            //    _G.Print();
        }

        //static Point GetV(int i, int j, int k)
        //{

        //}
        /// <summary>
        ///  Сборка слау для первой задачи
        /// </summary>
        public void Collect()
        {
            ResetSlau();
            MakeSLau();          
            GetBoundaryConditions();
        }


        public void Collect(int timeLayer)
        {
            ResetSlauOptimized();
            MakeSLau(timeLayer);
            GetBoundaryConditions(timeLayer);
        }

        static void SubstractM()
        {
            Master.Slau.A -= _M * timeCoef;
        }

        static void ResetSlauOptimized()
        {
            SubstractM();
            Master.Slau.b.Reset();
        }

        static void ResetSlau()
        {
            Master.Slau.A.Reset();
            Master.Slau.b.Reset();
        }

        public void SwitchTask(Solver solver)
        {
            ResetSlau();
            _M *= Master.Sigma;
            SolveSecondTimeLayer(solver);
            ResetSlau();
            Master.Slau.A += _G * Master.Lamda + _H;
        }

        private void SolveSecondTimeLayer(Solver solver)
        {
            double deltaT = Grid.t[1] - Grid.t[0];

            Master.Slau.A = _M * 1 / deltaT + _G * Master.Lamda + _H;
            for (int j = 0; j < Grid.M - 1; j++)
                for (int i = 0; i < Grid.N - 1; i++) // проходим по КЭ 
                    AddLocalB(i, j, 1);

            Master.Slau.b += _M * (1 / deltaT) * Master.Slau.q[0];
            GetBoundaryConditions(1);
           
            Master.Slau.q[1] = solver.Solve(Master.Slau.A, Master.Slau.b);
        }

        private void GetMatrixesMG()
        {
            double[][] localMatrix;
            for (int j = 0; j < Grid.M - 1; j++)
                for (int i = 0; i < Grid.N - 1; i++) // проходим по КЭ 
                {
                    localMatrix = GetMassMatrix(Grid.hx[i], Grid.hy[j]);
                    AddLocalMatrix(_M, localMatrix, i, j);

                    localMatrix = GetStiffnessMatrix(Grid.hx[i], Grid.hy[j]);
                    AddLocalMatrix(_G, localMatrix, i, j);
                }
        }
        public void GetMatrixH()
        {
            double[][] localMatrix;
            for (int j = 0; j < Grid.M - 1; j++)
                for (int i = 0; i < Grid.N - 1; i++) // проходим по КЭ 
                {
                    localMatrix = GetGradTMatrix(LocalNumToGlobal(i, j, 0), Grid.hx[i], Grid.hy[j], Grid.x[i], Grid.y[j]);
                    AddLocalMatrix(_H, localMatrix, i, j);
                }
        }
        public static int LocalNumToGlobal(int i, int j, int k)
        {
            return i + j * Grid.N + k / 2 * Grid.N + k % 2;
        }
        static void AddLocalMatrix(Matrix matrix, double[][] localMatrix, int i, int j)
        {
            for (int p = 0; p < 4; p++)
            {
                matrix.di[LocalNumToGlobal(i, j, p)] += localMatrix[p][p];

                int ibeg = matrix.ia[LocalNumToGlobal(i, j, p)];
                int iend = matrix.ia[LocalNumToGlobal(i, j, p) + 1];
                for (int k = 0; k < p; k++)
                {
                    int index = BinarySearch(matrix.ja, LocalNumToGlobal(i, j, k), ibeg, iend - 1);

                    matrix.au[index] += localMatrix[k][p];
                    matrix.al[index] += localMatrix[p][k];
                    ibeg++;
                }
            }
        }
        static int BinarySearch(List<int> list, int value, int l, int r)
        {
            while (l != r)
            {
                int mid = (l + r) / 2 + 1;

                if (list[mid] > value)
                    r = mid - 1;
                else
                    l = mid;
            }

            return (list[l] == value) ? l : -1;
        }
        private static void GetTimeConditions()
        {
            for (int p = 0; p < 2; p++) //записываем в [1] и в [2] так как потом в цикле вызовется SwapSolves и значение перезапишутся в [0] и [1] соотвественно.     
                for (int j = 0; j < Grid.M - 1; j++)
                    for (int i = 0; i < Grid.N - 1; i++) // проходим по КЭ 
                    {
                        Master.Slau.q[p].Elements[i + j * Grid.N] = Master.Func2(Grid.x[i], Grid.y[j], Grid.t[p]);
                        Master.Slau.q[p].Elements[i + j * Grid.N + 1] = Master.Func2(Grid.x[i + 1], Grid.y[j], Grid.t[p]);
                        Master.Slau.q[p].Elements[i + (j + 1) * Grid.N] = Master.Func2(Grid.x[i], Grid.y[j + 1], Grid.t[p]);
                        Master.Slau.q[p].Elements[i + (j + 1) * Grid.N + 1] = Master.Func2(Grid.x[i + 1], Grid.y[j + 1], Grid.t[p]);
                    }
        }
        static int mu(int i) => ((i) % 2);
        static int nu(int i) => ((i) / 2);

        static double[][] GetMassMatrix(double hx, double hy)// Grid.M - номер кэ 
        {
            // инициализация
            double[][] result = new double[4][];
            for (int i = 0; i < result.Length; i++)
                result[i] = new double[result.Length];

            //матрица масс
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result.Length; j++)
                    result[i][j] += M(hx, mu(i), mu(j)) * M(hy, nu(i), nu(j));

            return result;
        }
        static double[][] GetStiffnessMatrix(double hx, double hy)// Grid.M - номер кэ 
        {
            // инициализация
            double[][] result = new double[4][];
            for (int i = 0; i < result.Length; i++)
                result[i] = new double[result.Length];

            //матрица жесткости
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result.Length; j++)
                    result[i][j] = (G(hx, mu(i), mu(j)) * M(hy, nu(i), nu(j)) + M(hx, mu(i), mu(j)) * G(hy, nu(i), nu(j))); //матрица жётскости

            return result;
        }

        static void MakeSLau()
        {
            Master.Slau.A +=  _M * Master.Gamma + _G * Master.Lamda;

            for (int j = 0; j < Grid.M - 1; j++)
                for (int i = 0; i < Grid.N - 1; i++) // проходим по КЭ 
                    AddLocalB(i, j);
        }

        static void MakeSLau(int timeLayer)
        {
            double deltaT = Grid.t[timeLayer] - Grid.t[timeLayer - 2];
            double deltaT1 = Grid.t[timeLayer - 1] - Grid.t[timeLayer - 2];
            double deltaT0 = Grid.t[timeLayer] - Grid.t[timeLayer - 1];

            Vector vector1 = _M * Master.Slau.q[timeLayer - 2];
            Vector vector2 = _M * Master.Slau.q[timeLayer - 1];

            timeCoef = ((deltaT + deltaT0) / (deltaT * deltaT0));

            Master.Slau.b += -(deltaT0 / (deltaT * deltaT1)) * vector1 + deltaT / (deltaT1 * deltaT0) * vector2;
            Master.Slau.A += _M * timeCoef;

            for (int j = 0; j < Grid.M - 1; j++)
                for (int i = 0; i < Grid.N - 1; i++) // проходим по КЭ 
                    AddLocalB(i, j, timeLayer);
        }
        static void AddLocalB(int i, int j)
        {
            Master.Slau.b.Elements[i + j * Grid.N] += Grid.hx[i] * Grid.hy[j] / 36 * (4 * Master.F1(Grid.x[i], Grid.y[j]) + 2 * Master.F1(Grid.x[i + 1], Grid.y[j]) + 2 * Master.F1(Grid.x[i], Grid.y[j + 1]) + Master.F1(Grid.x[i + 1], Grid.y[j + 1]));
            Master.Slau.b.Elements[i + j * Grid.N + 1] += Grid.hx[i] * Grid.hy[j] / 36 * (2 * Master.F1(Grid.x[i], Grid.y[j]) + 4 * Master.F1(Grid.x[i + 1], Grid.y[j]) + Master.F1(Grid.x[i], Grid.y[j + 1]) + 2 * Master.F1(Grid.x[i + 1], Grid.y[j + 1]));
            Master.Slau.b.Elements[i + (j + 1) * Grid.N] += Grid.hx[i] * Grid.hy[j] / 36 * (2 * Master.F1(Grid.x[i], Grid.y[j]) + Master.F1(Grid.x[i + 1], Grid.y[j]) + 4 * Master.F1(Grid.x[i], Grid.y[j + 1]) + 2 * Master.F1(Grid.x[i + 1], Grid.y[j + 1]));
            Master.Slau.b.Elements[i + (j + 1) * Grid.N + 1] += Grid.hx[i] * Grid.hy[j] / 36 * (Master.F1(Grid.x[i], Grid.y[j]) + 2 * Master.F1(Grid.x[i + 1], Grid.y[j]) + 2 * Master.F1(Grid.x[i], Grid.y[j + 1]) + 4 * Master.F1(Grid.x[i + 1], Grid.y[j + 1]));
        }
        static void AddLocalB(int i, int j, int timeLayer)
        {
            Master.Slau.b.Elements[i + j * Grid.N] += Grid.hx[i] * Grid.hy[j] / 36 * (4 * Master.F2(Grid.x[i], Grid.y[j], Grid.t[timeLayer]) + 2 * Master.F2(Grid.x[i + 1], Grid.y[j], Grid.t[timeLayer]) + 2 * Master.F2(Grid.x[i], Grid.y[j + 1], Grid.t[timeLayer]) + Master.F2(Grid.x[i + 1], Grid.y[j + 1], Grid.t[timeLayer]));
            Master.Slau.b.Elements[i + j * Grid.N + 1] += Grid.hx[i] * Grid.hy[j] / 36 * (2 * Master.F2(Grid.x[i], Grid.y[j], Grid.t[timeLayer]) + 4 * Master.F2(Grid.x[i + 1], Grid.y[j], Grid.t[timeLayer]) + Master.F2(Grid.x[i], Grid.y[j + 1], Grid.t[timeLayer]) + 2 * Master.F2(Grid.x[i + 1], Grid.y[j + 1], Grid.t[timeLayer]));
            Master.Slau.b.Elements[i + (j + 1) * Grid.N] += Grid.hx[i] * Grid.hy[j] / 36 * (2 * Master.F2(Grid.x[i], Grid.y[j], Grid.t[timeLayer]) + Master.F2(Grid.x[i + 1], Grid.y[j], Grid.t[timeLayer]) + 4 * Master.F2(Grid.x[i], Grid.y[j + 1], Grid.t[timeLayer]) + 2 * Master.F2(Grid.x[i + 1], Grid.y[j + 1], Grid.t[timeLayer]));
            Master.Slau.b.Elements[i + (j + 1) * Grid.N + 1] += Grid.hx[i] * Grid.hy[j] / 36 * (Master.F2(Grid.x[i], Grid.y[j], Grid.t[timeLayer]) + 2 * Master.F2(Grid.x[i + 1], Grid.y[j], Grid.t[timeLayer]) + 2 * Master.F2(Grid.x[i], Grid.y[j + 1], Grid.t[timeLayer]) + 4 * Master.F2(Grid.x[i + 1], Grid.y[j + 1], Grid.t[timeLayer]));
        }
        static void ZeroingRow(int row)
        {
            for (int i = Master.Slau.A.ia[row]; i < Master.Slau.A.ia[row + 1]; i++)
                Master.Slau.A.al[i] = 0;

            for (int j = row + 1; j < Master.Slau.A.Size; j++)
            {
                int jbeg = Master.Slau.A.ia[j];
                int jend = Master.Slau.A.ia[j + 1];
                int index = BinarySearch(Master.Slau.A.ja, row, jbeg, jend - 1);
                if (index != -1)
                    Master.Slau.A.au[index] = 0;
            }
        }

        static void GetBoundaryConditions()
        {
            //нижняя граница
            if (Master.boundaryConditions[0] == 1)//первое краевое
                for (int i = 0; i < Grid.N; i++)
                {
                    ZeroingRow(i);
                    Master.Slau.A.di[i] = 1;
                    Master.Slau.b.Elements[i] = Master.Func1(Grid.x[i], Grid.y[0]);
                }
            else //второе краевое            
                for (int i = 0; i < Grid.N - 1; i++)
                {
                    Master.Slau.b.Elements[i] -= Master.Lamda * Grid.hx[i] / 6 * (2 * Master.DivFuncY1(Grid.x[i], Grid.y[0]) + Master.DivFuncY1(Grid.x[i + 1], Grid.y[0]));
                    Master.Slau.b.Elements[i + 1] -= Master.Lamda * Grid.hx[i] / 6 * (2 * Master.DivFuncY1(Grid.x[i], Grid.y[0]) + Master.DivFuncY1(Grid.x[i + 1], Grid.y[0]));
                }

            if (Master.boundaryConditions[2] == 1)//верхняя граница
                for (int i = Master.Slau.A.Size - 1; i > Master.Slau.A.Size - Grid.N - 1; i--)
                {
                    ZeroingRow(i);
                    Master.Slau.A.di[i] = 1;
                    Master.Slau.b.Elements[i] = Master.Func1(Grid.x[i % Grid.N], Grid.y[Grid.M - 1]);
                }
            else
                for (int i = 0; i < Grid.N - 1; i++)
                {
                    Master.Slau.b.Elements[Grid.N * (Grid.M - 1) + i] += Master.Lamda * Grid.hx[i] / 6 * (2 * Master.DivFuncY1(Grid.x[i], Grid.y[Grid.M - 1]) + Master.DivFuncY1(Grid.x[i + 1], Grid.y[Grid.M - 1]));
                    Master.Slau.b.Elements[Grid.N * (Grid.M - 1) + i + 1] += Master.Lamda * Grid.hx[i] / 6 * (Master.DivFuncY1(Grid.x[i], Grid.y[Grid.M - 1]) + 2 * Master.DivFuncY1(Grid.x[i + 1], Grid.y[Grid.M - 1]));
                }

            if (Master.boundaryConditions[3] == 1)//левая гравнь
                for (int i = Grid.N; i < Master.Slau.A.Size - Grid.N - 1; i += Grid.N)
                {
                    ZeroingRow(i);
                    Master.Slau.A.di[i] = 1;
                    Master.Slau.b.Elements[i] = Master.Func1(Grid.x[0], Grid.y[i / Grid.N]);
                }
            else
                for (int i = 0; i < Grid.N - 1; i++)
                {
                    Master.Slau.b.Elements[Grid.N * i] -= Grid.hy[i] * Master.Lamda / 6 * (2 * Master.DivFuncX1(Grid.x[0], Grid.y[i]) + Master.DivFuncX1(Grid.x[0], Grid.y[i + 1]));
                    Master.Slau.b.Elements[Grid.N * (i + 1)] -= Grid.hy[i] * Master.Lamda / 6 * (Master.DivFuncX1(Grid.x[0], Grid.y[i]) + 2 * Master.DivFuncX1(Grid.x[0], Grid.y[i + 1]));
                }

            if (Master.boundaryConditions[1] == 1)//правая граница
                for (int i = 2 * Grid.N - 1; i < Master.Slau.A.Size - 1; i += Grid.N)
                {
                    ZeroingRow(i);
                    Master.Slau.A.di[i] = 1;
                    Master.Slau.b.Elements[i] = Master.Func1(Grid.x[Grid.N - 1], Grid.y[i / Grid.N]);
                }
            else
                for (int i = 0; i < Grid.N - 1; i++)
                {
                    Master.Slau.b.Elements[Grid.N * (i + 1) - 1] += Grid.hy[i] * Master.Lamda / 6 * (2 * Master.DivFuncX1(Grid.x[Grid.N - 1], Grid.y[i]) + Master.DivFuncX1(Grid.x[0], Grid.y[i + 1]));
                    Master.Slau.b.Elements[Grid.N * (i + 2) - 1] += Grid.hy[i] * Master.Lamda / 6 * (Master.DivFuncX1(Grid.x[Grid.N - 1], Grid.y[i]) + 2 * Master.DivFuncX1(Grid.x[0], Grid.y[i + 1]));
                }
        }
        static void GetBoundaryConditions(int timeLayer)
        {
            //нижняя граница
            if (Master.boundaryConditions[0] == 1)//первое краевое
                for (int i = 0; i < Grid.N; i++)
                {
                    ZeroingRow(i);
                    Master.Slau.A.di[i] = 1;
                    Master.Slau.b.Elements[i] = Master.Func2(Grid.x[i], Grid.y[0], Grid.t[timeLayer]);
                    //  Master.Slau.A.di[i] = C;
                    //   Master.Slau.b.Elements[i] = C * Master.Func2(Grid.x[i], Grid.y[0], Grid.t[timeLayer]);
                }
            else //второе краевое            
                for (int i = 0; i < Grid.N - 1; i++)
                {
                    Master.Slau.b.Elements[i] -= Master.Lamda * Grid.hx[i] / 6 * (2 * Master.DivFuncY2(Grid.x[i], Grid.y[0], Grid.t[timeLayer]) + Master.DivFuncY2(Grid.x[i + 1], Grid.y[0], Grid.t[timeLayer]));
                    Master.Slau.b.Elements[i + 1] -= Master.Lamda * Grid.hx[i] / 6 * (Master.DivFuncY2(Grid.x[i], Grid.y[0], Grid.t[timeLayer]) + 2 * Master.DivFuncY2(Grid.x[i + 1], Grid.y[0], Grid.t[timeLayer]));
                }

            if (Master.boundaryConditions[2] == 1)//верхняя граница
                for (int i = Master.Slau.A.Size - 1; i > Master.Slau.A.Size - Grid.N - 1; i--)
                {
                    ZeroingRow(i);
                    Master.Slau.A.di[i] = 1;
                    Master.Slau.b.Elements[i] = Master.Func2(Grid.x[i % Grid.N], Grid.y[Grid.M - 1], Grid.t[timeLayer]);
                }
            else
                for (int i = 0; i < Grid.N - 1; i++)
                {
                    Master.Slau.b.Elements[Grid.N * (Grid.M - 1) + i] += Master.Lamda * Grid.hx[i] / 6 * (2 * Master.DivFuncY2(Grid.x[i], Grid.y[Grid.M - 1], Grid.t[timeLayer]) + Master.DivFuncY2(Grid.x[i + 1], Grid.y[Grid.M - 1], Grid.t[timeLayer]));
                    Master.Slau.b.Elements[Grid.N * (Grid.M - 1) + i + 1] += Master.Lamda * Grid.hx[i] / 6 * (Master.DivFuncY2(Grid.x[i], Grid.y[Grid.M - 1], Grid.t[timeLayer]) + 2 * Master.DivFuncY2(Grid.x[i + 1], Grid.y[Grid.M - 1], Grid.t[timeLayer]));
                }

            if (Master.boundaryConditions[3] == 1)//левая гравнь
                for (int i = Grid.N; i < Master.Slau.A.Size - Grid.N - 1; i += Grid.N)
                {
                    ZeroingRow(i);
                    Master.Slau.A.di[i] = 1;
                    Master.Slau.b.Elements[i] = Master.Func2(Grid.x[0], Grid.y[i / Grid.N], Grid.t[timeLayer]);
                }
            else
                for (int i = 0; i < Grid.N - 1; i++)
                {
                    Master.Slau.b.Elements[Grid.N * i] -= Grid.hy[i] * Master.Lamda / 6 * (2 * Master.DivFuncX2(Grid.x[0], Grid.y[i], Grid.t[timeLayer]) + Master.DivFuncX2(Grid.x[0], Grid.y[i + 1], Grid.t[timeLayer]));
                    Master.Slau.b.Elements[Grid.N * (i + 1)] -= Grid.hy[i] * Master.Lamda / 6 * (Master.DivFuncX2(Grid.x[0], Grid.y[i], Grid.t[timeLayer]) + 2 * Master.DivFuncX2(Grid.x[0], Grid.y[i + 1], Grid.t[timeLayer]));
                }

            if (Master.boundaryConditions[1] == 1)//правая граница
                for (int i = 2 * Grid.N - 1; i < Master.Slau.A.Size - 1; i += Grid.N)
                {
                    ZeroingRow(i);
                    Master.Slau.A.di[i] = 1;
                    Master.Slau.b.Elements[i] = Master.Func2(Grid.x[Grid.N - 1], Grid.y[i / Grid.N], Grid.t[timeLayer]);
                }
            else
                for (int i = 0; i < Grid.N - 1; i++)
                {
                    Master.Slau.b.Elements[Grid.N * (i + 1) - 1] += Grid.hy[i] * Master.Lamda / 6 * (2 * Master.DivFuncX2(Grid.x[Grid.N - 1], Grid.y[i], Grid.t[timeLayer]) + Master.DivFuncX2(Grid.x[0], Grid.y[i + 1], Grid.t[timeLayer]));
                    Master.Slau.b.Elements[Grid.N * (i + 2) - 1] += Grid.hy[i] * Master.Lamda / 6 * (Master.DivFuncX2(Grid.x[Grid.N - 1], Grid.y[i], Grid.t[timeLayer]) + 2 * Master.DivFuncX2(Grid.x[0], Grid.y[i + 1], Grid.t[timeLayer]));
                }
        }


        static double[][] GetGradTMatrix(int elemNumber, double hx, double hy, double xLeft, double yLower)
        {
            double[][] matrix = new double[4][];
            Point xBoundaries = new Point(xLeft, xLeft + hx);
            Point yBoundaries = new Point(yLower, yLower + hy);

            for (int i = 0; i < 4; i++)
            {
                matrix[i] = new double[4];
                for (int j = 0; j < 4; j++)
                    matrix[i][j] = NumericalMethods.GaussIntegration(elemNumber, i, j, xBoundaries, yBoundaries, hx, hy);                
            }

            return matrix;
        }

    }
}