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
        ///     Width and height of the collider rectangle
        /// </summary>
        public Vector2F Size { get; set; } = Vector2F.One;

        /// <summary>
        ///     Offset of the collider rectangle
        /// </summary>
        public Vector2F Offset { get; set; } = Vector2F.Zero;

        /// <summary>
        ///     If true the collider size will depend on entity's scale
        /// </summary>
        public bool Scaled { get; set; } = true;

        protected override Shape GetFarseerShapeImpl()
        {
            return new PolygonShape(new Vertices(GetVertices()), 1.0f);
        }

        public void DrawDebugOverlay(DebugDraw debugDraw)
        {
            var mat = Entity.GlobalTransform;
            if (!Scaled)
                mat *= Matrix3F.CreateScale(new Vector2F(1f / Entity.Scale.X, 1f / Entity.Scale.Y));

            foreach (var (a, b) in GeometryUtils.PolygonVerticesToEdges(GetVerticesUnscaled().ToArray()))
                debugDraw.DrawLine(a.TransformBy(mat), b.TransformBy(mat), Color.Green, 0.07f);
        }

        /// <summary>
        ///     Calculates position of rectangle vertices according to collider width,
        ///     height and entity scale
        /// </summary>
        /// <returns></returns>
        private IEnumerable<Vector2> GetVertices()
        {
            var scale = Scaled ? Entity.Scale : Vector2F.One;
            var (sx, sy) = (scale.X - 0.015f, scale.Y - 0.015f);
            return GetVerticesUnscaled().Select(v => new Vector2(v.X * sx, v.Y * sy));
        }

        private IEnumerable<Vector2> GetVerticesUnscaled()
        {
            var size = Size * MathF.Sqrt(2);
            return GeometryUtils
                .GenerateRegularPolygon(Vector2F.Zero, size, MathF.PI / 4, 4)
                .Select(v => (Vector2) (Offset + v));
        }
    }
}