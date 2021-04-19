using FarseerPhysics.Collision.Shapes;

namespace GameProject.Ecs.Physics
{
    /// <summary>
    ///     Represents circular collision model for an entity
    /// </summary>
    internal class CircleCollider : Collider
    {
        /// <summary>
        ///     Radius of the collider circle
        /// </summary>
        public float Radius { get; set; } = 1f;

        public override Shape GetFarseerShape()
        {
            return new CircleShape(Radius, 1.0f);
        }
    }
}