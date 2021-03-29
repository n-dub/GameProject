namespace GameProject.GameMath
{
    public struct Vector2F
    {
        public readonly float X;
        public readonly float Y;

        public float LengthSquared => this * this;

        public float Length => MathF.Sqrt(LengthSquared);

        public Vector2F(float x, float y) => (X, Y) = (x, y);

        public Vector2F(Vector3F vector) => (X, Y) = (vector.X, vector.Y);

        public static Vector2F operator +(Vector2F a, Vector2F b) => new Vector2F(a.X + b.X, a.Y + b.Y);

        public static Vector2F operator -(Vector2F a, Vector2F b) => new Vector2F(a.X - b.X, a.Y - b.Y);

        public static float operator *(Vector2F a, Vector2F b) => a.X * b.X + a.Y * b.Y;

        public static Vector2F operator *(Vector2F v, float a) => new Vector2F(v.X * a, v.Y * a);

        public static Vector2F operator *(float a, Vector2F v) => v * a;

        public Vector2F TransformBy(Matrix3F matrix) => new Vector2F(matrix * new Vector3F(this, 1));

        public static bool operator ==(Vector2F a, Vector2F b) => a.Equals(b);

        public static bool operator !=(Vector2F a, Vector2F b) => !a.Equals(b);

        public override bool Equals(object obj) => obj is Vector2F f && X == f.X && Y == f.Y;

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

        public override string ToString() => $"({X}, {Y})";
    }
}
