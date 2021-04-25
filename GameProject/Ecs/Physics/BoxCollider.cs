using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using GameProject.GameDebug;
using GameProject.GameMath;
using Microsoft.Xna.Framework;

namespace GameProject.Ecs.Physics
{
    /// <summary>
    ///     Represents rectangular collision model for an entity
    /// </summary>
    internal class BoxCollider : Collider, IDebuggable
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
        
        private IEnumerable<Vector2> GetVerticesUnscaled()
        {
            var (sx, sy) = (Entity.Scale.X, Entity.Scale.Y);
            yield return new Vector2(SizeX / 2, SizeY / 2);
            yield return new Vector2(SizeX / -2, SizeY / 2);
            yield return new Vector2(SizeX / 2, SizeY / -2);
            yield return new Vector2(SizeX / -2, SizeY / -2);
        }

        public override Shape GetFarseerShape()
        {
            return new PolygonShape(new Vertices(GetVertices()), 1.0f);
        }

        public void DrawDebugOverlay(DebugDraw debugDraw)
        {
            var mat = Entity.GlobalTransform;
            foreach (var (a, b) in GetVerticesUnscaled()
                .Select(v => new Vector2F(v))
                .SelectMany(a => GetVerticesUnscaled()
                    .Select(v => new Vector2F(v))
                    .Select(b => (a, b)))
                .Where(p => p.a != p.b))
                debugDraw.DrawLine(a.TransformBy(mat), b.TransformBy(mat), Color.Green, 0.03f);
        }
    }
}