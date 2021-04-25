using System;
using System.Drawing.Drawing2D;
using System.Runtime.CompilerServices;
using unvell.D2DLib;

namespace GameProject.GameMath
{
    /// <summary>
    ///     Represents a 3x3 matrix, read-only
    /// </summary>
    internal class Matrix3F
    {
        /// <summary>
        ///     An identity matrix
        /// </summary>
        public static readonly Matrix3F Identity;

        /// <summary>
        ///     Size of matrix
        /// </summary>
        public const int Size = 3;

        /// <summary>
        ///     Get an element on certain row and column
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        public float this[int row, int column] => matrix[row, column];

        /// <summary>
        ///     Get a row of the matrix as a vector
        /// </summary>
        public Vector3F Row1 => new Vector3F(matrix[0, 0], matrix[0, 1], matrix[0, 2]);

        /// <summary>
        ///     Get a row of the matrix as a vector
        /// </summary>
        public Vector3F Row2 => new Vector3F(matrix[1, 0], matrix[1, 1], matrix[1, 2]);

        /// <summary>
        ///     Get a row of the matrix as a vector
        /// </summary>
        public Vector3F Row3 => new Vector3F(matrix[2, 0], matrix[2, 1], matrix[2, 2]);

        /// <summary>
        ///     Get a column of the matrix as a vector
        /// </summary>
        public Vector3F Column1 => new Vector3F(matrix[0, 0], matrix[1, 0], matrix[2, 0]);

        /// <summary>
        ///     Get a column of the matrix as a vector
        /// </summary>
        public Vector3F Column2 => new Vector3F(matrix[0, 1], matrix[1, 1], matrix[2, 1]);

        /// <summary>
        ///     Get a column of the matrix as a vector
        /// </summary>
        public Vector3F Column3 => new Vector3F(matrix[0, 2], matrix[1, 2], matrix[2, 2]);

        /// <summary>
        ///     Get transposed matrix
        /// </summary>
        public Matrix3F Transposed => new Matrix3F(Column1, Column2, Column3);

        private readonly float[,] matrix;

        /// <summary>
        ///     Create a new matrix with specified elements
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

        /// <summary>
        ///     Create a new matrix with specified rows
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Matrix3F(Vector3F row1, Vector3F row2, Vector3F row3)
            : this(row1.X, row1.Y, row1.Z,
                row2.X, row2.Y, row2.Z,
                row3.X, row3.Y, row3.Z)
        {
        }

        static Matrix3F()
        {
            var identityArray = new float[Size, Size];

            for (var i = 0; i < Size; ++i)
            for (var j = 0; j < Size; ++j)
                identityArray[i, j] = i == j ? 1 : 0;

            Identity = new Matrix3F(identityArray);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Matrix3F(float[,] matrix)
        {
            this.matrix = matrix;
        }

        /// <summary>
        ///     Add two matrices
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix3F operator +(Matrix3F a, Matrix3F b)
        {
            var result = new float[Size, Size];

            for (var i = 0; i < Size; ++i)
            for (var j = 0; j < Size; ++j)
                result[i, j] = a[i, j] + b[i, j];

            return new Matrix3F(result);
        }

        /// <summary>
        ///     Subtract two matrices
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix3F operator -(Matrix3F a, Matrix3F b)
        {
            var result = new float[Size, Size];

            for (var i = 0; i < Size; ++i)
            for (var j = 0; j < Size; ++j)
                result[i, j] = a[i, j] - b[i, j];

            return new Matrix3F(result);
        }

        /// <summary>
        ///     Multiply two matrices
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix3F operator *(Matrix3F a, Matrix3F b)
        {
            var result = new float[Size, Size];

            for (var i = 0; i < Size; ++i)
            for (var j = 0; j < Size; ++j)
            for (var k = 0; k < Size; ++k)
                result[i, j] += a[i, k] * b[k, j];

            return new Matrix3F(result);
        }

        /// <summary>
        ///     Multiply a matrix by a 3-dimensional vector
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3F operator *(Matrix3F a, Vector3F b)
        {
            return new Vector3F(a.Row1 * b, a.Row2 * b, a.Row3 * b);
        }

        /// <summary>
        ///     Multiply a matrix by a scalar value
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix3F operator *(Matrix3F m, float a)
        {
            return new Matrix3F(m.Row1 * a, m.Row2 * a, m.Row3 * a);
        }

        /// <summary>
        ///     Create a new matrix from an array of elements, copies the passed array
        /// </summary>
        /// <param name="source">An array of elements</param>
        /// <returns>The created matrix</returns>
        /// <exception cref="ArgumentException">Thrown if the array isn't exactly [Size x Size]</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix3F CreateFromArray(float[,] source)
        {
            if (source.GetLength(0) != Size || source.GetLength(1) != Size)
                throw new ArgumentException($"Array must be [{Size} x {Size}]");

            var destination = new float[Size, Size];
            Array.Copy(source, destination, destination.Length);

            return new Matrix3F(destination);
        }

        /// <summary>
        ///     Create a translation matrix
        /// </summary>
        /// <param name="position">Translation vector to use</param>
        /// <returns>The created matrix</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix3F CreateTranslation(Vector2F position)
        {
            return new Matrix3F(
                1, 0, position.X,
                0, 1, position.Y,
                0, 0, 1);
        }

        /// <summary>
        ///     Create a rotation matrix
        /// </summary>
        /// <param name="angle">Angle in radians</param>
        /// <returns>The created matrix</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix3F CreateRotation(float angle)
        {
            return new Matrix3F(
                MathF.Cos(angle), -MathF.Sin(angle), 0,
                MathF.Sin(angle), MathF.Cos(angle), 0,
                0, 0, 1);
        }

        /// <summary>
        ///     Create a scaling matrix
        /// </summary>
        /// <param name="scale">Vector that represents scaling</param>
        /// <returns>The created matrix</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix3F CreateScale(Vector2F scale)
        {
            return new Matrix3F(
                scale.X, 0, 0,
                0, scale.Y, 0,
                0, 0, 1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Matrix(Matrix3F source)
        {
            source = source.Transposed;
            return new Matrix(
                source[0, 0], source[0, 1],
                source[1, 0], source[1, 1],
                source[2, 0], source[2, 1]);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator D2DMatrix3x2(Matrix3F source)
        {
            source = source.Transposed;
            return new D2DMatrix3x2(
                source[0, 0], source[0, 1],
                source[1, 0], source[1, 1],
                source[2, 0], source[2, 1]);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Matrix3F a, Matrix3F b)
        {
            return AreAlmostEqual(a, b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Matrix3F a, Matrix3F b)
        {
            return !AreAlmostEqual(a, b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object obj)
        {
            return obj is Matrix3F f
                   && Row1.Equals(f.Row1)
                   && Row2.Equals(f.Row2)
                   && Row3.Equals(f.Row3);
        }

        /// <summary>
        ///     Checks if two matrices are approximately equal
        /// </summary>
        /// <param name="a">First matrix</param>
        /// <param name="b">Second matrix</param>
        /// <param name="delta">Allowed difference</param>
        /// <returns>True if matrices are almost equal</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

        public override string ToString()
        {
            return $"{Row1}\n{Row2}\n{Row3}";
        }
    }
}