namespace GameProject.GameMath
{
    public struct Vector3F
    {
        public readonly float X;
        public readonly float Y;
        public readonly float Z;

        public float LengthSquared => this * this;

        public float Length => MathF.Sqrt(LengthSquared);

        public Vector3F Normalized => this / Length;

        public Vector3F(Vector2F vector, float z) => (X, Y, Z) = (vector.X, vector.Y, z);

        public Vector3F(float x, float y, float z) => (X, Y, Z) = (x, y, z);

        public static Vector3F operator +(Vector3F a, Vector3F b) => new Vector3F(a.X + b.X, a.Y + b.Y, a.Z + b.Z);

        public static Vector3F operator -(Vector3F a, Vector3F b) => new Vector3F(a.X - b.X, a.Y - b.Y, a.Z - b.Z);

        public static float operator *(Vector3F a, Vector3F b) => a.X * b.X + a.Y * b.Y + a.Z * b.Z;

        public static Vector3F operator *(Vector3F v, float a) => new Vector3F(v.X * a, v.Y * a, v.Z * a);

        public static Vector3F operator *(float a, Vector3F v) => v * a;
        
        public static Vector3F operator /(Vector3F v, float a) => v * (1.0f / a);

        public static bool operator ==(Vector3F a, Vector3F b) => a.Equals(b);

        public static bool operator !=(Vector3F a, Vector3F b) => !a.Equals(b);

        public override bool Equals(object obj) => obj is Vector3F f && X == f.X && Y == f.Y && Z == f.Z;

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

        public override string ToString() => $"({X}, {Y}, {Z})";
    }
}
