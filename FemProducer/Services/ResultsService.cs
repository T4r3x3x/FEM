using FemProducer.Logger;

using Grid.Models;
using Grid.Models.InputModels;
using MathModels.Models;

using System.Collections;
using System.Text;

namespace FemProducer.Services
{
    /// <summary>
    /// Класс отвечает за запись результатов работы программы
    /// </summary>
    public class ResultsService<TLogger> where TLogger : ILogger
    {
        private readonly TLogger _logger;
        private readonly GridModel _grid;
        private readonly SolutionService _resultProducer;
        private readonly StringBuilder stringBuilder = new StringBuilder();
        private readonly ProblemService _problemParametrs;

        public ResultsService(TLogger logger, GridModel grid, SolutionService resultProducer, ProblemService problemParametrs)
        {
            _logger = logger;
            _grid = grid;
            _resultProducer = resultProducer;
            _problemParametrs = problemParametrs;
        }

        public void PrintTimeGrid()
        {
            stringBuilder.Clear();
            stringBuilder.AppendLine();

            for (int i = 0; i < _grid.T.Count; i++)
                stringBuilder.AppendLine("[" + _grid.T[i] + "] ");

            stringBuilder.AppendLine();

            _logger.Log(stringBuilder.ToString());
        }

        public void PrintSlae(Slae slae)
        {
            double[][] matrix = slae.Matrix.ConvertToDenseFormat();
            for (int i = 0; i < matrix.Length; i++)
            {
                for (int j = 0; j < matrix.Length; j++)
                {
                    if (matrix[i][j] >= 0)
                        Console.Write(" ");
                    Console.Write(matrix[i][j].ToString("E2") + " ");
                }
                Console.Write(" " + slae.Vector[i].ToString("E2"));
                Console.WriteLine();
            }
            Console.WriteLine("\n\n");
        }


        public void PrintResult(int timeLayer, bool printSolution)
        {
            Vector exactSolution = _resultProducer.AnalyticsSolves[timeLayer];
            var solution = _resultProducer.NumericalSolves[timeLayer];

            if (printSolution)
            {
                Console.WriteLine("      Численное решение      |         точное решение        |      разница решений      ");
                Console.WriteLine("--------------------------------------------------------------------------------------");
                for (int i = 0; i < solution.Length; i++)
                    Console.WriteLine("u{0} = {1:E16} | u*{0} = {2:E16} | {3:E16}", i + 1, solution[i], exactSolution[i], Math.Abs(exactSolution[i] - solution[i]));
            }

            Console.WriteLine("Относительная погрешность: " + _resultProducer.GetSolveDifference(timeLayer));
        }

        public void WriteSolveWithGrid(string path, Vector solve)
        {
            using (var file = File.Open(path, FileMode.Create))
            using (StreamWriter sw = new StreamWriter(file))
            {
                sw.WriteLine(_grid.Subdomains.Count);
                foreach (var subDomain in _grid.Subdomains)
                {
                    foreach (var value in subDomain)
                    {
                        sw.Write(value.ToString().Replace(',', '.') + " ");
                    }
                    sw.WriteLine();
                }

                sw.WriteLine(_grid.Nodes.Count);

                for (int i = 0; i < _grid.Nodes.Count; i++)
                {
                    sw.Write(_grid.Nodes[i].ToString().Replace(",", ".") + " ");
                    sw.Write(solve[i].ToString().Replace(",", ".") + "\n");
                }
            }
        }

        private void WriteEnumerable(string path, IEnumerable collection)
        {
            using (var file = File.Open(path, FileMode.Create))
            using (StreamWriter sw = new StreamWriter(file))
                foreach (var item in collection)
                    sw.WriteLine(item.ToString().Replace(',', '.'));
        }

        public void PrintSlaeByComponents(string folder, Slae slae)
        {
            using (var file = File.Open("slae/N", FileMode.Create))
            using (StreamWriter sw = new StreamWriter(file))
            {
                sw.WriteLine(slae.Size.ToString());
            }
            WriteEnumerable(folder + "/" + nameof(slae.Matrix.Al), slae.Matrix.Al);
            WriteEnumerable(folder + "/" + nameof(slae.Matrix.Au), slae.Matrix.Au);
            WriteEnumerable(folder + "/" + nameof(slae.Matrix.Di), slae.Matrix.Di);
            WriteEnumerable(folder + "/" + nameof(slae.Matrix.Ia), slae.Matrix.Ia);
            WriteEnumerable(folder + "/" + nameof(slae.Matrix.Ja), slae.Matrix.Ja);
            WriteEnumerable(folder + "/" + "B", slae.Vector);
        }

        public void WriteSolve(string path, IList<Vector> solve)
        {
            using (var file = File.Open(path, FileMode.Create))
            using (StreamWriter sw = new StreamWriter(file))
            {
                sw.WriteLine(_grid.XCount * _grid.YCount);
                sw.WriteLine(_grid.Z.Count);

                //   for (int j = 0; j < _grid.T.Count; j++)
                for (int i = 0; i < _grid.Nodes.Count; i++)
                    sw.WriteLine(_grid.Nodes[i].ToString() + " " + solve[0][i].ToString().Replace(",", "."));
            }
        }

        public void WriteGrid(string path, GridInputParameters gridParameter)
        {
            using (var file = File.Open(path, FileMode.OpenOrCreate))
            using (StreamWriter sw = new StreamWriter(file))
            {
                sw.WriteLine(_grid.Elements.Count);
                foreach (var element in _grid.Elements)
                {
                    sw.WriteLine(_grid.Nodes[element.NodesIndexes[0]]);
                    sw.WriteLine(_grid.Nodes[element.NodesIndexes[1]]);
                    sw.WriteLine(_grid.Nodes[element.NodesIndexes[3]]);
                    sw.WriteLine(_grid.Nodes[element.NodesIndexes[2]]);
                    sw.WriteLine();
                }
            }
        }
        public void WriteGrid2(string path, GridInputParameters gridParameter)
        {
            using (var file = File.Open(path, FileMode.OpenOrCreate))
            using (StreamWriter sw = new StreamWriter(file))
            {
                sw.WriteLine(_grid.Elements.Count);
                foreach (var element in _grid.Elements)
                {
                    sw.WriteLine(_grid.Nodes[element.NodesIndexes[0]]);
                    sw.WriteLine(_grid.Nodes[element.NodesIndexes[1]]);
                    sw.WriteLine(_grid.Nodes[element.NodesIndexes[5]]);
                    sw.WriteLine(_grid.Nodes[element.NodesIndexes[4]]);
                    sw.WriteLine();
                }
            }
        }
    }
}