using System.Drawing;
using FarseerPhysics.Collision.Shapes;
using GameProject.GameDebug;
using GameProject.GameMath;

namespace GameProject.Ecs.Physics
{
    /// <summary>
    ///     Represents circular collision model for an entity
    /// </summary>
    internal class CircleCollider : Collider, IDebuggable
    {
        /// <summary>
        ///     Radius of the collider circle
        /// </summary>
        public float Radius { get; set; } = 1f;

        protected override Shape GetFarseerShapeImpl()
        {
            return new CircleShape(Radius, 1.0f);
        }

        public void DrawDebugOverlay(DebugDraw debugDraw)
        {
            debugDraw.DrawEllipse(Entity.Position - new Vector2F(Radius),
                new Vector2F(Radius * 2), Color.Green, 0.07f);
        }
    }
}