namespace GameProject.GameMath
{
    public struct Vector2F
    {
        public float X;
        public float Y;

        public float LengthSquared => this * this;

        public float Length => MathF.Sqrt(LengthSquared);

        public Vector2F(float x, float y) => (X, Y) = (x, y);

        public static Vector2F operator +(Vector2F a, Vector2F b) => new Vector2F(a.X + b.X, a.Y + b.Y);

        public static Vector2F operator -(Vector2F a, Vector2F b) => new Vector2F(a.X - b.X, a.Y - b.Y);

        public static float operator *(Vector2F a, Vector2F b) => a.X * b.X + a.Y * b.Y;

        public static bool operator ==(Vector2F a, Vector2F b) => a.Equals(b);

        public static bool operator !=(Vector2F a, Vector2F b) => !a.Equals(b);

        public override bool Equals(object obj) => obj is Vector2F f && X == f.X && Y == f.Y;

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
