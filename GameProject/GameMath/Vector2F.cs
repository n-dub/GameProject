using System.Drawing;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using unvell.D2DLib;

namespace GameProject.GameMath
{
    /// <summary>
    ///     Represents a tow-dimensional vector
    /// </summary>
    internal readonly struct Vector2F
    {
        /// <inheritdoc cref="Vector3F.UnitX" />
        public static readonly Vector2F UnitX = new Vector2F(1, 0);

        /// <inheritdoc cref="Vector3F.UnitY" />
        public static readonly Vector2F UnitY = new Vector2F(0, 1);

        /// <inheritdoc cref="Vector3F.One" />
        public static readonly Vector2F One = new Vector2F(1, 1);

        /// <inheritdoc cref="Vector3F.Zero" />
        public static readonly Vector2F Zero = new Vector2F(0, 0);

        /// <inheritdoc cref="Vector3F.X" />
        public readonly float X;

        /// <inheritdoc cref="Vector3F.Y" />
        public readonly float Y;

        /// <inheritdoc cref="Vector3F.LengthSquared" />
        public float LengthSquared => this * this;

        /// <inheritdoc cref="Vector3F.Length" />
        public float Length => MathF.Sqrt(LengthSquared);

        /// <inheritdoc cref="Vector3F.Normalized" />
        public Vector2F Normalized => this / Length;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector2F(float value) : this(value, value)
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector2F(float x, float y)
        {
            (X, Y) = (x, y);
        }

        /// <summary>
        ///     Create a 2-dimensional vector from x an y components of <see cref="Vector3F" />
        /// </summary>
        /// <param name="vector"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector2F(Vector3F vector)
        {
            (X, Y) = (vector.X, vector.Y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector2F(Vector2 vector)
        {
            (X, Y) = (vector.X, vector.Y);
        }

        /// <inheritdoc cref="Vector3F.WithX" />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector2F WithX(float x)
        {
            return new Vector2F(x, Y);
        }

        /// <summary>
        ///     Rotate vector by a certain angle
        /// </summary>
        /// <param name="angle">Angle to rotate by</param>
        /// <returns>Rotated vector</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector2F Rotate(float angle)
        {
            return new Vector2F(
                X * MathF.Cos(angle) - Y * MathF.Sin(angle),
                X * MathF.Sin(angle) + Y * MathF.Cos(angle));
        }

        /// <inheritdoc cref="Vector3F.WithY" />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector2F WithY(float y)
        {
            return new Vector2F(X, y);
        }

        /// <inheritdoc cref="Vector3F.operator+" />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2F operator +(Vector2F a, Vector2F b)
        {
            return new Vector2F(a.X + b.X, a.Y + b.Y);
        }

        /// <inheritdoc cref="Vector3F.operator-(Vector3F, Vector3F)" />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2F operator -(Vector2F a, Vector2F b)
        {
            return new Vector2F(a.X - b.X, a.Y - b.Y);
        }

        /// <inheritdoc cref="Vector3F.operator-(Vector3F)" />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2F operator -(Vector2F vector)
        {
            return vector * -1;
        }

        /// <inheritdoc cref="Vector3F.operator*(Vector3F, Vector3F)" />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float operator *(Vector2F a, Vector2F b)
        {
            return a.X * b.X + a.Y * b.Y;
        }

        /// <inheritdoc cref="Vector3F.operator*(Vector3F, float)" />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2F operator *(Vector2F v, float a)
        {
            return new Vector2F(v.X * a, v.Y * a);
        }

        /// <inheritdoc cref="Vector3F.operator*(float, Vector3F)" />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2F operator *(float a, Vector2F v)
        {
            return v * a;
        }

        /// <inheritdoc cref="Vector3F.operator/(Vector3F, float)" />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2F operator /(Vector2F v, float a)
        {
            return v * (1.0f / a);
        }

        /// <summary>
        ///     Transform a vector by a matrix
        /// </summary>
        /// <param name="matrix">Matrix to use for transformation</param>
        /// <returns>The transformed vector</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector2F TransformBy(Matrix3F matrix)
        {
            return new Vector2F(matrix * new Vector3F(this, 1));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Vector2F a, Vector2F b)
        {
            return AreAlmostEqual(a, b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Vector2F a, Vector2F b)
        {
            return !AreAlmostEqual(a, b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator PointF(Vector2F vector)
        {
            return new PointF(vector.X, vector.Y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator SizeF(Vector2F vector)
        {
            return new SizeF(vector.X, vector.Y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Vector2(Vector2F vector)
        {
            return new Vector2(vector.X, vector.Y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator D2DPoint(Vector2F vector)
        {
            return new D2DPoint(vector.X, vector.Y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator D2DSize(Vector2F vector)
        {
            return new D2DSize(vector.X, vector.Y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object obj)
        {
            return obj is Vector2F f
                   // ReSharper disable once CompareOfFloatsByEqualityOperator
                   && X == f.X
                   // ReSharper disable once CompareOfFloatsByEqualityOperator
                   && Y == f.Y;
        }

        /// <inheritdoc cref="Vector3F.AreAlmostEqual" />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AreAlmostEqual(Vector2F a, Vector2F b, float delta = 1e-6f)
        {
            return MathF.Abs(a.X - b.X) < delta
                   && MathF.Abs(a.Y - b.Y) < delta;
        }

        public override int GetHashCode()
        {
            var hashCode = 1861411795;
            hashCode = hashCode * -1521134295 + X.GetHashCode();
            hashCode = hashCode * -1521134295 + Y.GetHashCode();
            return hashCode;
        }

        public override string ToString()
        {
            return $"({X}, {Y})";
        }
    }
}