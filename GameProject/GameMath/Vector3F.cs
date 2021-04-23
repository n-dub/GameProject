namespace GameProject.GameMath
{
    internal readonly struct Vector3F
    {
        /// <summary>
        ///     A unit vector in X-axis
        /// </summary>
        public static readonly Vector3F UnitX = new Vector3F(1, 0, 0);

        /// <summary>
        ///     A unit vector in Y-axis
        /// </summary>
        public static readonly Vector3F UnitY = new Vector3F(0, 1, 0);

        /// <summary>
        ///     A unit vector in Z-axis
        /// </summary>
        public static readonly Vector3F UnitZ = new Vector3F(0, 0, 1);

        /// <summary>
        ///     A vector which coordinates are only ones
        /// </summary>
        public static readonly Vector3F One = new Vector3F(1, 1, 1);

        /// <summary>
        ///     A vector which coordinates are only zeros
        /// </summary>
        public static readonly Vector3F Zero = new Vector3F(0, 0, 0);

        /// <summary>
        ///     X component of the vector
        /// </summary>
        public readonly float X;

        /// <summary>
        ///     Y component of the vector
        /// </summary>
        public readonly float Y;

        /// <summary>
        ///     Z component of the vector
        /// </summary>
        public readonly float Z;

        /// <summary>
        ///     Length of the vector in the power of two
        /// </summary>
        public float LengthSquared => this * this;

        /// <summary>
        ///     Length of the vector
        /// </summary>
        public float Length => MathF.Sqrt(LengthSquared);

        /// <summary>
        ///     Get normalized vector
        /// </summary>
        public Vector3F Normalized => this / Length;

        public Vector3F(Vector2F vector, float z)
        {
            (X, Y, Z) = (vector.X, vector.Y, z);
        }

        public Vector3F(float x, float y, float z)
        {
            (X, Y, Z) = (x, y, z);
        }

        /// <summary>
        ///     Duplicate this vector but set another X coordinate
        /// </summary>
        /// <param name="x">New X coordinate</param>
        /// <returns>Created vector</returns>
        public Vector3F WithX(float x)
        {
            return new Vector3F(x, Y, Z);
        }

        /// <summary>
        ///     Duplicate this vector but set another Y coordinate
        /// </summary>
        /// <param name="y">New Y coordinate</param>
        /// <returns>Created vector</returns>
        public Vector3F WithY(float y)
        {
            return new Vector3F(X, y, Z);
        }

        /// <summary>
        ///     Duplicate this vector but set another Z coordinate
        /// </summary>
        /// <param name="z">New Z coordinate</param>
        /// <returns>Created vector</returns>
        public Vector3F WithZ(float z)
        {
            return new Vector3F(X, Y, z);
        }

        /// <summary>
        ///     Add vectors
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <returns>Sum of two vectors</returns>
        public static Vector3F operator +(Vector3F a, Vector3F b)
        {
            return new Vector3F(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }

        /// <summary>
        ///     Subtract vectors
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <returns>Difference of two vectors</returns>
        public static Vector3F operator -(Vector3F a, Vector3F b)
        {
            return new Vector3F(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }

        /// <summary>
        ///     Negate a vector
        /// </summary>
        /// <param name="vector">Vector to negate</param>
        /// <returns>A negated vector</returns>
        public static Vector3F operator -(Vector3F vector)
        {
            return vector * -1;
        }

        /// <summary>
        ///     Get a dot product of two vectors
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <returns>Dot product</returns>
        public static float operator *(Vector3F a, Vector3F b)
        {
            return a.X * b.X + a.Y * b.Y + a.Z * b.Z;
        }

        /// <summary>
        ///     Multiply a vector by a scalar value
        /// </summary>
        /// <param name="v">Vector</param>
        /// <param name="a">Scalar</param>
        /// <returns>The result of multiplication</returns>
        public static Vector3F operator *(Vector3F v, float a)
        {
            return new Vector3F(v.X * a, v.Y * a, v.Z * a);
        }

        /// <inheritdoc cref="operator*(Vector3F, float)" />
        public static Vector3F operator *(float a, Vector3F v)
        {
            return v * a;
        }

        /// <summary>
        ///     Divide a vector by a scalar value
        /// </summary>
        /// <param name="v">Vector</param>
        /// <param name="a">Scalar</param>
        /// <returns>The result of division</returns>
        public static Vector3F operator /(Vector3F v, float a)
        {
            return v * (1.0f / a);
        }

        public static bool operator ==(Vector3F a, Vector3F b)
        {
            return AreAlmostEqual(a, b);
        }

        public static bool operator !=(Vector3F a, Vector3F b)
        {
            return !AreAlmostEqual(a, b);
        }

        public override bool Equals(object obj)
        {
            return obj is Vector3F f
                   // ReSharper disable once CompareOfFloatsByEqualityOperator
                   && X == f.X
                   // ReSharper disable once CompareOfFloatsByEqualityOperator
                   && Y == f.Y
                   // ReSharper disable once CompareOfFloatsByEqualityOperator
                   && Z == f.Z;
        }

        /// <summary>
        ///     Check if two vectors are approximately equal
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <param name="delta">Allowed difference</param>
        /// <returns>True if vectors are almost equal</returns>
        public static bool AreAlmostEqual(Vector3F a, Vector3F b, float delta = 1e-6f)
        {
            return MathF.Abs(a.X - b.X) < delta
                   && MathF.Abs(a.Y - b.Y) < delta
                   && MathF.Abs(a.Z - b.Z) < delta;
        }

        public override int GetHashCode()
        {
            var hashCode = 1861411795;
            hashCode = hashCode * -1521134295 + X.GetHashCode();
            hashCode = hashCode * -1521134295 + Y.GetHashCode();
            hashCode = hashCode * -1521134295 + Z.GetHashCode();
            return hashCode;
        }

        public override string ToString()
        {
            return $"({X}, {Y}, {Z})";
        }
    }
}