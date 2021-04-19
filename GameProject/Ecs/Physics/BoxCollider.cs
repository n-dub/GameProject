using System.Collections.Generic;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using Microsoft.Xna.Framework;

namespace GameProject.Ecs.Physics
{
    /// <summary>
    ///     Represents rectangular collision model for an entity
    /// </summary>
    internal class BoxCollider : Collider
    {
        /// <summary>
        ///     Width of the collider rectangle
        /// </summary>
        public float SizeX { get; set; } = 1f;

        /// <summary>
        ///     Height of the collider rectangle
        /// </summary>
        public float SizeY { get; set; } = 1f;

        /// <summary>
        ///     Calculates position of rectangle vertices according to collider width,
        ///     height and entity scale
        /// </summary>
        /// <returns></returns>
        private IEnumerable<Vector2> GetVertices()
        {
            var (sx, sy) = (Entity.Scale.X, Entity.Scale.Y);
            yield return new Vector2(SizeX * sx / 2, SizeY * sy / 2);
            yield return new Vector2(SizeX * sx / -2, SizeY * sy / 2);
            yield return new Vector2(SizeX * sx / 2, SizeY * sy / -2);
            yield return new Vector2(SizeX * sx / -2, SizeY * sy / -2);
        }

        public override Shape GetFarseerShape()
        {
            return new PolygonShape(new Vertices(GetVertices()), 1.0f);
        }
    }
}