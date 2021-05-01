using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using GameProject.CoreEngine;
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
            return GetVerticesUnscaled().Select(v => new Vector2(v.X * sx, v.Y * sy));
        }

        private IEnumerable<Vector2> GetVerticesUnscaled()
        {
            var size = new Vector2F(SizeX, SizeY) * MathF.Sqrt(2);
            return GeometryUtility
                .GenerateRegularPolygon(Vector2F.Zero, size, MathF.PI / 4, 4)
                .Select(v => (Vector2) v);
        }

        public override Shape GetFarseerShape()
        {
            return new PolygonShape(new Vertices(GetVertices()), 1.0f);
        }

        public void DrawDebugOverlay(DebugDraw debugDraw)
        {
            var mat = Entity.GlobalTransform;

            foreach (var (a, b) in GeometryUtility.PolygonVerticesToEdges(GetVerticesUnscaled().ToArray()))
                debugDraw.DrawLine(a.TransformBy(mat), b.TransformBy(mat), Color.Green, 0.07f);
        }
    }
}