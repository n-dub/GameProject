using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using GameProject.GameMath;
using Microsoft.Xna.Framework;

namespace GameProject.CoreEngine
{
    internal static class GeometryUtils
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<Vector2F> GenerateRegularPolygon(Vector2F location, Vector2F radial,
            float initialRotation, int vertexCount)
        {
            var current = Vector2F.UnitY.Rotate(initialRotation) / 2;
            var angle = 2 * MathF.PI / vertexCount;

            for (var i = 0; i < vertexCount; i++)
            {
                var v = current + location;
                yield return new Vector2F(v.X * radial.X, v.Y * radial.Y);
                current = current.Rotate(angle);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<(Vector2F v1, Vector2F v2)> PolygonVerticesToEdges(IEnumerable<Vector2F> vertices)
        {
            return PolygonVerticesToEdges(vertices.Select(v => (Vector2) v).ToArray());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<(Vector2F v1, Vector2F v2)> PolygonVerticesToEdges(Vector2[] vertices)
        {
            return vertices
                .Select(v => new Vector2F(v))
                .Zip(vertices
                    .Skip(1)
                    .Concat(vertices.Take(1))
                    .Select(v => new Vector2F(v)), (v1, v2) => (v1, v2));
        }
    }
}