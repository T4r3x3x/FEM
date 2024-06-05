namespace MathModels.Models
{
    public class DenseMatrix
    {
        private readonly double[,] _elems;

        public DenseMatrix(double[,] elems) => _elems = elems;
        public DenseMatrix(int size) => _elems = new double[size, size];
        public DenseMatrix(int n, int m) => _elems = new double[n, m];

        public int Size => _elems.GetLength(0) == _elems.GetLength(1) ? RowSize : throw new Exception("Матрица не квадратная");

        public int RowSize => _elems.GetLength(0);
        public int ColumnSize => _elems.GetLength(1);

        public double this[int i, int j]
        {
            get => _elems[i, j];
            set => _elems[i, j] = value;
        }

        public Vector GetColumn(int columnIndex)
        {
            var elems = new double[ColumnSize];
            for (int i = 0; i < elems.Length; i++)
            {
                elems[i] = _elems[i, columnIndex];
            }
            return new(elems);
        }

        public Vector GetRaw(int rawIndex)
        {
            var elems = new double[ColumnSize];
            for (int i = 0; i < elems.Length; i++)
            {
                elems[i] = _elems[rawIndex, i];
            }
            return new(elems);
        }

        public double Determinant()
        {
            var positive =
            _elems[0, 0] * _elems[1, 1] * _elems[2, 2] +
            _elems[1, 0] * _elems[2, 1] * _elems[0, 2] +
            _elems[2, 0] * _elems[0, 1] * _elems[1, 2];
            var negative =
                _elems[2, 0] * _elems[1, 1] * _elems[0, 2] +
                _elems[0, 0] * _elems[2, 1] * _elems[1, 2] +
                _elems[1, 0] * _elems[0, 1] * _elems[2, 2];

            return positive - negative;
        }

        public DenseMatrix GetInverseMatrix()
        {
            var detA = Determinant();

            for (int i = 0; i < RowSize; i++)
                for (int j = i; j < ColumnSize; j++)
                    (_elems[i, j], _elems[j, i]) = (_elems[j, i], _elems[i, j]);

            DenseMatrix inverseMatrix = new(Size);
            inverseMatrix[0, 0] = _elems[1, 1] * _elems[2, 2] - _elems[1, 2] * _elems[2, 1];
            inverseMatrix[0, 1] = -(_elems[1, 0] * _elems[2, 2] - _elems[1, 2] * _elems[2, 0]);
            inverseMatrix[0, 2] = _elems[1, 0] * _elems[2, 1] - _elems[1, 1] * _elems[2, 0];
            inverseMatrix[1, 0] = -(_elems[0, 1] * _elems[2, 2] - _elems[0, 2] * _elems[2, 1]);
            inverseMatrix[1, 1] = _elems[0, 0] * _elems[2, 2] - _elems[0, 2] * _elems[2, 0];
            inverseMatrix[1, 2] = -(_elems[0, 0] * _elems[2, 1] - _elems[0, 1] * _elems[2, 0]);
            inverseMatrix[2, 0] = _elems[0, 1] * _elems[1, 2] - _elems[0, 2] * _elems[1, 1];
            inverseMatrix[2, 1] = -(_elems[0, 0] * _elems[1, 2] - _elems[0, 2] * _elems[1, 0]);
            inverseMatrix[2, 2] = _elems[0, 0] * _elems[1, 1] - _elems[0, 1] * _elems[1, 0];

            for (int i = 0; i < RowSize; i++)
                for (int j = 0; j < ColumnSize; j++)
                    inverseMatrix[i, j] /= detA;
            return inverseMatrix;
        }



        public override string ToString()
        {
            string res = "";
            for (int i = 0; i < _elems.GetLength(0); i++)
            {
                for (int j = 0; j < _elems.GetLength(1); j++)
                    res += _elems[i, j] + " ";
                res += "\n";
            }
            return res;
        }

        public void SwapRows(int i, int j)
        {
            for (int k = 0; k < Size; k++)
                (_elems[i, k], _elems[j, k]) = (_elems[j, k], _elems[i, k]);
        }

        private void GetMaxValueInColumn(int columnIndex, out double maxNumber, out int maxNumberIndex)
        {
            maxNumber = Math.Abs(_elems[columnIndex, columnIndex]);
            maxNumberIndex = columnIndex;
            for (int j = columnIndex + 1; j < Size; j++)
            {
                if (Math.Abs(_elems[j, columnIndex]) > maxNumber)
                {
                    maxNumber = Math.Abs(_elems[j, columnIndex]);
                    maxNumberIndex = j;
                }
            }
        }

        public static Vector operator *(DenseMatrix matrix, Vector vector)
        {
            Vector result = new(vector.Length);

            for (int i = 0; i < matrix.Size; i++)
                for (int j = 0; j < matrix.Size; j++)
                    result[i] += matrix[i, j] * vector[j];

            return result;
        }

        public static DenseMatrix operator *(DenseMatrix a, DenseMatrix b)
        {
            if (a.ColumnSize != b.RowSize)
                throw new ArgumentException("Невозможно умножить матрицы!");
            double[,] elems = new double[a.RowSize, b.ColumnSize];
            for (int i = 0; i < a.RowSize; i++)
                for (int j = 0; j < b.ColumnSize; j++)
                    for (int k = 0; k < a.ColumnSize; k++)
                        elems[i, j] += a[i, k] * b[k, j];

            return new(elems);
        }
    }
}