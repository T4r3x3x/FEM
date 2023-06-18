﻿using ResearchPaper;
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
        static int area = 0;

     

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
            _G.Reset();
            _M.Reset();

            double[][] localMatrix;
            for (int j = 0; j < Grid.M - 1; j++) //y
                for (int i = 0; i < Grid.N - 1; i++) //x | проходим по КЭ 
                {
                    int area = Grid.GetAreaNumber(i, j);

                    localMatrix = FEM.GetMassMatrix(Grid.hx[i], Grid.hy[j]);

                    for (int p = 0; p < 4; p++)
                        for (int k = 0; k < 4; k++)
                            localMatrix[p][k] *= Master.Sigma(area);

                    AddLocalMatrix(_M, localMatrix, i, j);


                    localMatrix = FEM.GetStiffnessMatrix(Grid.hx[i], Grid.hy[j]);

                    for (int p = 0; p < 4; p++)
                        for (int k = 0; k < 4; k++)
                            localMatrix[p][k] *= Master.Lamda(area);

                    AddLocalMatrix(_G, localMatrix, i, j);
                }
            SolveSecondTimeLayer(solver);
            ResetSlau();
            Master.Slau.A += _G + _H;
        }

        private void SolveSecondTimeLayer(Solver solver)
        {
         //   double deltaT = Grid.ht[0];//Grid.t[1] - Grid.t[0];

            Master.Slau.A = _M * 1 / Grid.ht[0] + _G * Master.Lamda(0) + _H;
            for (int j = 0; j < Grid.M - 1; j++)
                for (int i = 0; i < Grid.N - 1; i++) // проходим по КЭ 
                    AddLocalB(i, j, 1);

            Master.Slau.b += _M * (1 / Grid.ht[0]) * Master.Slau.q[0];
            GetBoundaryConditions(1);
           
            Master.Slau.q[1] = solver.Solve(Master.Slau.A, Master.Slau.b);
        }

        private void GetMatrixesMG()
        {
            double[][] localMatrix;
            for (int j = 0; j < Grid.M - 1; j++) //y
                for (int i = 0; i < Grid.N - 1; i++) //x | проходим по КЭ 
                {
                    int area = Grid.GetAreaNumber(i, j);

                    localMatrix = FEM.GetMassMatrix(Grid.hx[i], Grid.hy[j]);

                    for (int p = 0; p < 4; p++)
                        for (int k = 0; k < 4; k++)
                            localMatrix[p][k] *= Master.Gamma(area);

                    AddLocalMatrix(_M, localMatrix, i, j);



                    localMatrix = FEM.GetStiffnessMatrix(Grid.hx[i], Grid.hy[j]);
                   
                    for (int p = 0; p < 4; p++)
                        for (int k = 0; k < 4; k++)
                            localMatrix[p][k] *= Master.Lamda(area);
                     
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
                        area = Grid.GetAreaNumber(i, j);
                        Master.Slau.q[p].Elements[i + j * Grid.N] = Master.Func2(Grid.x[i], Grid.y[j], Grid.t[p],area);
                        Master.Slau.q[p].Elements[i + j * Grid.N + 1] = Master.Func2(Grid.x[i + 1], Grid.y[j], Grid.t[p], area);
                        Master.Slau.q[p].Elements[i + (j + 1) * Grid.N] = Master.Func2(Grid.x[i], Grid.y[j + 1], Grid.t[p], area);
                        Master.Slau.q[p].Elements[i + (j + 1) * Grid.N + 1] = Master.Func2(Grid.x[i + 1], Grid.y[j + 1], Grid.t[p], area);
                    }
        }
        

        static void MakeSLau()
        {
            Master.Slau.A +=  _M + _G;

            for (int j = 0; j < Grid.M - 1; j++)
                for (int i = 0; i < Grid.N - 1; i++) // проходим по КЭ 
                    AddLocalB(i, j);
        }

        static void MakeSLau(int timeLayer)
        {
            double deltaT = Grid.ht[timeLayer - 1] + Grid.ht[timeLayer - 2];//Grid.t[timeLayer] - Grid.t[timeLayer - 2];
            double deltaT1 = Grid.ht[timeLayer - 2];//Grid.t[timeLayer - 1] - Grid.t[timeLayer - 2];
            double deltaT0 = Grid.ht[timeLayer-1];//Grid.t[timeLayer] - Grid.t[timeLayer - 1];

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
            area = Grid.GetAreaNumber(i, j);
            Master.Slau.b.Elements[i + j * Grid.N] += Grid.hx[i] * Grid.hy[j] / 36 * (4 * Master.F1(Grid.x[i], Grid.y[j], area) + 2 * Master.F1(Grid.x[i + 1], Grid.y[j], area) + 2 * Master.F1(Grid.x[i], Grid.y[j + 1], area) + Master.F1(Grid.x[i + 1], Grid.y[j + 1], area));
            Master.Slau.b.Elements[i + j * Grid.N + 1] += Grid.hx[i] * Grid.hy[j] / 36 * (2 * Master.F1(Grid.x[i], Grid.y[j], area) + 4 * Master.F1(Grid.x[i + 1], Grid.y[j], area) + Master.F1(Grid.x[i], Grid.y[j + 1], area) + 2 * Master.F1(Grid.x[i + 1], Grid.y[j + 1], area));
            Master.Slau.b.Elements[i + (j + 1) * Grid.N] += Grid.hx[i] * Grid.hy[j] / 36 * (2 * Master.F1(Grid.x[i], Grid.y[j], area) + Master.F1(Grid.x[i + 1], Grid.y[j], area) + 4 * Master.F1(Grid.x[i], Grid.y[j + 1], area) + 2 * Master.F1(Grid.x[i + 1], Grid.y[j + 1], area));
            Master.Slau.b.Elements[i + (j + 1) * Grid.N + 1] += Grid.hx[i] * Grid.hy[j] / 36 * (Master.F1(Grid.x[i], Grid.y[j],area) + 2 * Master.F1(Grid.x[i + 1], Grid.y[j], area) + 2 * Master.F1(Grid.x[i], Grid.y[j + 1], area) + 4 * Master.F1(Grid.x[i + 1], Grid.y[j + 1], area));
        }
        static void AddLocalB(int i, int j, int timeLayer)
        {
            area = Grid.GetAreaNumber(i, j);
            Master.Slau.b.Elements[i + j * Grid.N] += Grid.hx[i] * Grid.hy[j] / 36 * (4 * Master.F2(Grid.x[i], Grid.y[j], Grid.t[timeLayer], area) + 2 * Master.F2(Grid.x[i + 1], Grid.y[j], Grid.t[timeLayer], area) + 2 * Master.F2(Grid.x[i], Grid.y[j + 1], Grid.t[timeLayer], area) + Master.F2(Grid.x[i + 1], Grid.y[j + 1], Grid.t[timeLayer], area));
            Master.Slau.b.Elements[i + j * Grid.N + 1] += Grid.hx[i] * Grid.hy[j] / 36 * (2 * Master.F2(Grid.x[i], Grid.y[j], Grid.t[timeLayer], area) + 4 * Master.F2(Grid.x[i + 1], Grid.y[j], Grid.t[timeLayer], area) + Master.F2(Grid.x[i], Grid.y[j + 1], Grid.t[timeLayer], area) + 2 * Master.F2(Grid.x[i + 1], Grid.y[j + 1], Grid.t[timeLayer], area));
            Master.Slau.b.Elements[i + (j + 1) * Grid.N] += Grid.hx[i] * Grid.hy[j] / 36 * (2 * Master.F2(Grid.x[i], Grid.y[j], Grid.t[timeLayer], area) + Master.F2(Grid.x[i + 1], Grid.y[j], Grid.t[timeLayer], area) + 4 * Master.F2(Grid.x[i], Grid.y[j + 1], Grid.t[timeLayer], area) + 2 * Master.F2(Grid.x[i + 1], Grid.y[j + 1], Grid.t[timeLayer], area));
            Master.Slau.b.Elements[i + (j + 1) * Grid.N + 1] += Grid.hx[i] * Grid.hy[j] / 36 * (Master.F2(Grid.x[i], Grid.y[j], Grid.t[timeLayer], area) + 2 * Master.F2(Grid.x[i + 1], Grid.y[j], Grid.t[timeLayer], area) + 2 * Master.F2(Grid.x[i], Grid.y[j + 1], Grid.t[timeLayer], area) + 4 * Master.F2(Grid.x[i + 1], Grid.y[j + 1], Grid.t[timeLayer], area));
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
                    area = Grid.GetAreaNumber(i, 0);
                    ZeroingRow(i);
                    Master.Slau.A.di[i] = 1;
                    Master.Slau.b.Elements[i] = Master.Func1(Grid.x[i], Grid.y[0], area);
                }
            //else //второе краевое            
            //    for (int i = 0; i < Grid.N - 1; i++)
            //    {
            //        area = Grid.GetAreaNumber(i, 0);
            //        Master.Slau.b.Elements[i] -= Master.Lamda * Grid.hx[i] / 6 * (2 * Master.DivFuncY1(Grid.x[i], Grid.y[0], area) + Master.DivFuncY1(Grid.x[i + 1], Grid.y[0], area));
            //        Master.Slau.b.Elements[i + 1] -= Master.Lamda * Grid.hx[i] / 6 * (2 * Master.DivFuncY1(Grid.x[i], Grid.y[0], area) + Master.DivFuncY1(Grid.x[i + 1], Grid.y[0], area));
            //    }

            if (Master.boundaryConditions[2] == 1)//верхняя граница
                for (int i = Master.Slau.A.Size - 1; i > Master.Slau.A.Size - Grid.N - 1; i--)
                {
                    area = Grid.GetAreaNumber(i % Grid.N, Grid.M - 1);
                    ZeroingRow(i);
                    Master.Slau.A.di[i] = 1;
                    Master.Slau.b.Elements[i] = Master.Func1(Grid.x[i % Grid.N], Grid.y[Grid.M - 1],area);
                }
           // else
                //for (int i = 0; i < Grid.N - 1; i++)
                //{
                //    area = Grid.GetAreaNumber(i, Grid.M - 1);
                //    Master.Slau.b.Elements[Grid.N * (Grid.M - 1) + i] += Master.Lamda * Grid.hx[i] / 6 * (2 * Master.DivFuncY1(Grid.x[i], Grid.y[Grid.M - 1], area) + Master.DivFuncY1(Grid.x[i + 1], Grid.y[Grid.M - 1], area));
                //    Master.Slau.b.Elements[Grid.N * (Grid.M - 1) + i + 1] += Master.Lamda * Grid.hx[i] / 6 * (Master.DivFuncY1(Grid.x[i], Grid.y[Grid.M - 1], area) + 2 * Master.DivFuncY1(Grid.x[i + 1], Grid.y[Grid.M - 1], area));
                //}

            if (Master.boundaryConditions[3] == 1)//левая гравнь
                for (int i = Grid.N; i < Master.Slau.A.Size - Grid.N - 1; i += Grid.N)
                {
                    area = Grid.GetAreaNumber(0, i / Grid.N);
                    ZeroingRow(i);
                    Master.Slau.A.di[i] = 1;
                    Master.Slau.b.Elements[i] = Master.Func1(Grid.x[0], Grid.y[i / Grid.N], area);
                }
          //  else
                //for (int i = 0; i < Grid.N - 1; i++)
                //{
                //    area = Grid.GetAreaNumber(0, i);
                //    Master.Slau.b.Elements[Grid.N * i] -= Grid.hy[i] * Master.Lamda / 6 * (2 * Master.DivFuncX1(Grid.x[0], Grid.y[i], area) + Master.DivFuncX1(Grid.x[0], Grid.y[i + 1], area));
                //    Master.Slau.b.Elements[Grid.N * (i + 1)] -= Grid.hy[i] * Master.Lamda / 6 * (Master.DivFuncX1(Grid.x[0], Grid.y[i], area) + 2 * Master.DivFuncX1(Grid.x[0], Grid.y[i + 1], area));
                //}

            if (Master.boundaryConditions[1] == 1)//правая граница
                for (int i = 2 * Grid.N - 1; i < Master.Slau.A.Size - 1; i += Grid.N)
                {
                    area = Grid.GetAreaNumber(Grid.N - 1, i / Grid.N);
                    ZeroingRow(i);
                    Master.Slau.A.di[i] = 1;
                    Master.Slau.b.Elements[i] = Master.Func1(Grid.x[Grid.N - 1], Grid.y[i / Grid.N], area);
                }
         //   else
                //for (int i = 0; i < Grid.N - 1; i++)
                //{
                //    area = Grid.GetAreaNumber(Grid.N - 1, i);
                //    Master.Slau.b.Elements[Grid.N * (i + 1) - 1] += Grid.hy[i] * Master.Lamda / 6 * (2 * Master.DivFuncX1(Grid.x[Grid.N - 1], Grid.y[i], area) + Master.DivFuncX1(Grid.x[0], Grid.y[i + 1], area));
                //    Master.Slau.b.Elements[Grid.N * (i + 2) - 1] += Grid.hy[i] * Master.Lamda / 6 * (Master.DivFuncX1(Grid.x[Grid.N - 1], Grid.y[i], area) + 2 * Master.DivFuncX1(Grid.x[0], Grid.y[i + 1], area));
                //}
        }
        static void GetBoundaryConditions(int timeLayer)
        {
            //нижняя граница
            if (Master.boundaryConditions[0] == 1)//первое краевое
                for (int i = 0; i < Grid.N; i++)
                {
                    area = Grid.GetAreaNumber(i, 0);
                    ZeroingRow(i);
                    Master.Slau.A.di[i] = 1;
                    Master.Slau.b.Elements[i] = Master.Func2(Grid.x[i], Grid.y[0], Grid.t[timeLayer], area);
                    //  Master.Slau.A.di[i] = C;
                    //   Master.Slau.b.Elements[i] = C * Master.Func2(Grid.x[i], Grid.y[0], Grid.t[timeLayer]);
                }
          //  else //второе краевое            
                //for (int i = 0; i < Grid.N - 1; i++)
                //{
                //    area = Grid.GetAreaNumber(i, 0);
                //    Master.Slau.b.Elements[i] -= Master.Lamda * Grid.hx[i] / 6 * (2 * Master.DivFuncY2(Grid.x[i], Grid.y[0], Grid.t[timeLayer],area  ) + Master.DivFuncY2(Grid.x[i + 1], Grid.y[0], Grid.t[timeLayer], area));
                //    Master.Slau.b.Elements[i + 1] -= Master.Lamda * Grid.hx[i] / 6 * (Master.DivFuncY2(Grid.x[i], Grid.y[0], Grid.t[timeLayer], area) + 2 * Master.DivFuncY2(Grid.x[i + 1], Grid.y[0], Grid.t[timeLayer], area));
                //}

            if (Master.boundaryConditions[2] == 1)//верхняя граница
                for (int i = Master.Slau.A.Size - 1; i > Master.Slau.A.Size - Grid.N - 1; i--)
                {
                    area = Grid.GetAreaNumber(i % Grid.N, Grid.M - 1);
                    ZeroingRow(i);
                    Master.Slau.A.di[i] = 1;
                    Master.Slau.b.Elements[i] = Master.Func2(Grid.x[i % Grid.N], Grid.y[Grid.M - 1], Grid.t[timeLayer], area);
                }
         //   else
                //for (int i = 0; i < Grid.N - 1; i++)
                //{
                //    area = Grid.GetAreaNumber(i, Grid.M - 1);
                //    Master.Slau.b.Elements[Grid.N * (Grid.M - 1) + i] += Master.Lamda * Grid.hx[i] / 6 * (2 * Master.DivFuncY2(Grid.x[i], Grid.y[Grid.M - 1], Grid.t[timeLayer] , area) + Master.DivFuncY2(Grid.x[i + 1], Grid.y[Grid.M - 1], Grid.t[timeLayer], area));
                //    Master.Slau.b.Elements[Grid.N * (Grid.M - 1) + i + 1] += Master.Lamda * Grid.hx[i] / 6 * (Master.DivFuncY2(Grid.x[i], Grid.y[Grid.M - 1], Grid.t[timeLayer], area) + 2 * Master.DivFuncY2(Grid.x[i + 1], Grid.y[Grid.M - 1], Grid.t[timeLayer], area));
                //}

            if (Master.boundaryConditions[3] == 1)//левая гравнь
                for (int i = Grid.N; i < Master.Slau.A.Size - Grid.N - 1; i += Grid.N)
                {
                    area = Grid.GetAreaNumber(0, i / Grid.N);
                    ZeroingRow(i);
                    Master.Slau.A.di[i] = 1;
                    Master.Slau.b.Elements[i] = Master.Func2(Grid.x[0], Grid.y[i / Grid.N], Grid.t[timeLayer], area);
                }
         //   else
                //for (int i = 0; i < Grid.N - 1; i++)
                //{
                //    area = Grid.GetAreaNumber(0, i);
                //    Master.Slau.b.Elements[Grid.N * i] -= Grid.hy[i] * Master.Lamda / 6 * (2 * Master.DivFuncX2(Grid.x[0], Grid.y[i], Grid.t[timeLayer], area) + Master.DivFuncX2(Grid.x[0], Grid.y[i + 1], Grid.t[timeLayer], area));
                //    Master.Slau.b.Elements[Grid.N * (i + 1)] -= Grid.hy[i] * Master.Lamda / 6 * (Master.DivFuncX2(Grid.x[0], Grid.y[i], Grid.t[timeLayer]   , area) + 2 * Master.DivFuncX2(Grid.x[0], Grid.y[i + 1], Grid.t[timeLayer], area));
                //}

            if (Master.boundaryConditions[1] == 1)//правая граница
                for (int i = 2 * Grid.N - 1; i < Master.Slau.A.Size - 1; i += Grid.N)
                {
                    area = Grid.GetAreaNumber(Grid.N - 1, i / Grid.N);
                    ZeroingRow(i);
                    Master.Slau.A.di[i] = 1;
                    Master.Slau.b.Elements[i] = Master.Func2(Grid.x[Grid.N - 1], Grid.y[i / Grid.N], Grid.t[timeLayer], area);
                }
        //    else
                //for (int i = 0; i < Grid.N - 1; i++)
                //{
                //    area = Grid.GetAreaNumber(Grid.N - 1, i);
                //    Master.Slau.b.Elements[Grid.N * (i + 1) - 1] += Grid.hy[i] * Master.Lamda / 6 * (2 * Master.DivFuncX2(Grid.x[Grid.N - 1], Grid.y[i], Grid.t[timeLayer], area) + Master.DivFuncX2(Grid.x[0], Grid.y[i + 1], Grid.t[timeLayer], area));
                //    Master.Slau.b.Elements[Grid.N * (i + 2) - 1] += Grid.hy[i] * Master.Lamda / 6 * (Master.DivFuncX2(Grid.x[Grid.N - 1], Grid.y[i], Grid.t[timeLayer], area) + 2 * Master.DivFuncX2(Grid.x[0], Grid.y[i + 1], Grid.t[timeLayer], area));
                //}
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