using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameProject.GameMath
{
    public class Matrix3F
    {
        public const int Size = 3;

        public float this[int row, int column] => matrix[row, column];

        public Vector3F Row1 => new Vector3F(matrix[0, 0], matrix[0, 1], matrix[0, 2]);
        public Vector3F Row2 => new Vector3F(matrix[1, 0], matrix[1, 1], matrix[1, 2]);
        public Vector3F Row3 => new Vector3F(matrix[2, 0], matrix[2, 1], matrix[2, 2]);

        public Vector3F Column1 => new Vector3F(matrix[0, 0], matrix[1, 0], matrix[2, 0]);
        public Vector3F Column2 => new Vector3F(matrix[0, 1], matrix[1, 1], matrix[2, 1]);
        public Vector3F Column3 => new Vector3F(matrix[0, 2], matrix[1, 2], matrix[2, 2]);

        public static readonly Matrix3F Identity;

        private readonly float[,] matrix;

        public Matrix3F(
            float a00, float a01, float a02,
            float a10, float a11, float a12,
            float a20, float a21, float a22)
            : this(new float[Size, Size])
        {
            (matrix[0, 0], matrix[0, 1], matrix[0, 2]) = (a00, a01, a02);
            (matrix[1, 0], matrix[1, 1], matrix[1, 2]) = (a10, a11, a12);
            (matrix[2, 0], matrix[2, 1], matrix[2, 2]) = (a20, a21, a22);
        }

        public Matrix3F(Vector3F row1, Vector3F row2, Vector3F row3)
            : this(row1.X, row1.Y, row1.Z,
                  row2.X, row2.Y, row2.Z,
                  row3.X, row3.Y, row3.Z)
        {
        }

        private Matrix3F(float[,] matrix)
        {
            this.matrix = matrix;
        }

        static Matrix3F()
        {
            var identityArray = new float[Size, Size];

            for (var i = 0; i < Size; ++i)
                for (var j = 0; j < Size; ++j)
                    identityArray[i, j] = i == j ? 1 : 0;

            Identity = new Matrix3F(identityArray);
        }

        public Matrix3F Transpose() => new Matrix3F(Column1, Column2, Column3);

        public static Matrix3F operator +(Matrix3F a, Matrix3F b)
        {
            var result = new float[Size, Size];

            for (var i = 0; i < Size; ++i)
                for (var j = 0; j < Size; ++j)
                    result[i, j] = a[i, j] + b[i, j];

            return new Matrix3F(result);
        }

        public static Matrix3F operator -(Matrix3F a, Matrix3F b)
        {
            var result = new float[Size, Size];

            for (var i = 0; i < Size; ++i)
                for (var j = 0; j < Size; ++j)
                    result[i, j] = a[i, j] - b[i, j];

            return new Matrix3F(result);
        }

        public static Matrix3F operator *(Matrix3F a, Matrix3F b)
        {
            var result = new float[Size, Size];

            for (var i = 0; i < Size; ++i)
                for (var j = 0; j < Size; ++j)
                    for (var k = 0; k < Size; ++k)
                        result[i, j] += a[i, k] * b[k, j];

            return new Matrix3F(result);
        }

        public static Vector3F operator *(Matrix3F a, Vector3F b) =>
            new Vector3F(a.Row1 * b, a.Row2 * b, a.Row3 * b);

        public static Matrix3F operator *(Matrix3F m, float a) => new Matrix3F(m.Row1 * a, m.Row2 * a, m.Row3 * a);

        public static Matrix3F CreateFromArray(float[,] source)
        {
            if (source.GetLength(0) != Size || source.GetLength(1) != Size)
                throw new ArgumentException($"Array must be [{Size} x {Size}]");

            var copiedArray = new float[Size, Size];
            for (var i = 0; i < Size; ++i)
                for (var j = 0; j < Size; ++j)
                    copiedArray[i, j] = source[i, j];

            return new Matrix3F(copiedArray);
        }

        public static Matrix3F CreateTranslation(Vector2F position)
        {
            return new Matrix3F(
                1, 0, position.X,
                0, 1, position.Y,
                0, 0, 0);
        }

        public static Matrix3F CreateRotation(float angle)
        {
            return new Matrix3F(
                MathF.Cos(angle), -MathF.Sin(angle), 0,
                MathF.Sin(angle), MathF.Cos(angle), 0,
                0, 0, 1);
        }

        public static Matrix3F CreateScale(Vector2F scale)
        {
            return new Matrix3F(
                scale.X, 0, 0,
                0, scale.Y, 0,
                0, 0, 1);
        }

        public override bool Equals(object obj)
        {
            return obj is Matrix3F f &&
                   Row1 == f.Row1 &&
                   Row2 == f.Row2 &&
                   Row3 == f.Row3;
        }

        public static bool AreAlmostEqual(Matrix3F a, Matrix3F b, float delta = 1e-6f)
        {
            return Vector3F.AreAlmostEqual(a.Row1, b.Row1, delta)
                && Vector3F.AreAlmostEqual(a.Row2, b.Row2, delta)
                && Vector3F.AreAlmostEqual(a.Row3, b.Row3, delta);
        }

        public override int GetHashCode()
        {
            var hashCode = -523877319;
            hashCode = hashCode * -1521134295 + Row1.GetHashCode();
            hashCode = hashCode * -1521134295 + Row2.GetHashCode();
            hashCode = hashCode * -1521134295 + Row3.GetHashCode();
            return hashCode;
        }

        public override string ToString() => $"{Row1}\n{Row2}\n{Row3}";
    }
}
