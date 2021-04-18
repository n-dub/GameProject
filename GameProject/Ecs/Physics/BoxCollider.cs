﻿using System.Collections.Generic;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using Microsoft.Xna.Framework;

namespace GameProject.Ecs.Physics
{
    internal class BoxCollider : Collider
    {
        public float SizeX { get; set; } = 1f;

        public float SizeY { get; set; } = 1f;

        public override Shape GetFarseerShape() => new PolygonShape(new Vertices(GetVertices()), 1.0f);

        private IEnumerable<Vector2> GetVertices()
        {
            yield return new Vector2(SizeX / 2, SizeY / 2);
            yield return new Vector2(SizeX / -2, SizeY / 2);
            yield return new Vector2(SizeX / 2, SizeY / -2);
            yield return new Vector2(SizeX / -2, SizeY / -2);
        }
    }
}