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
    internal class PolygonCollider : Collider, IDebuggable
    {
        public Vector2F[] Vertices { get; set; }

        public override Shape GetFarseerShape()
        {
            return new PolygonShape(new Vertices(Vertices.Select(v => (Vector2) v)), 1.0f);
        }

        public void DrawDebugOverlay(DebugDraw debugDraw)
        {
            var mat = Entity.GlobalTransform;

            foreach (var (a, b) in GeometryUtility.PolygonVerticesToEdges(Vertices))
                debugDraw.DrawLine(a.TransformBy(mat), b.TransformBy(mat), Color.Green, 0.07f);
        }
    }
}