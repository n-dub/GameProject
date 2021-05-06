using GameProject.GameMath;

namespace GameProject.Ecs.Physics
{
    internal readonly struct CollisionInfo
    {
        public Vector2F Point { get; }
        public Vector2F Normal { get; }
        public float NormalImpulse { get; }

        public CollisionInfo(Vector2F point, Vector2F normal, float normalImpulse)
        {
            Point = point;
            Normal = normal;
            NormalImpulse = normalImpulse;
        }
    }
}