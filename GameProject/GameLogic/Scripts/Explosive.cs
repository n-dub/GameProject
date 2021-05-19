using System;
using System.Collections.Generic;
using System.Linq;
using FarseerPhysics.Dynamics;
using GameProject.CoreEngine;
using GameProject.Ecs.Physics;
using GameProject.GameMath;
using Microsoft.Xna.Framework;

namespace GameProject.GameLogic.Scripts
{
    internal class Explosive : GameScript
    {
        public float Strength { get; set; } = 0.3f;
        public float ExplosionRadius { get; set; } = 5;
        public float ExplosionFragImpulse { get; set; } = 2;
        public int ExplosionFragCount { get; set; } = 20;

        private readonly List<PhysicsBody> frags = new List<PhysicsBody>();
        
        protected override void Update()
        {
            var body = Entity.GetComponent<PhysicsBody>();
            if (!body.ReadyToUse || frags.Any())
                return;

            var collision = LevelUtility.FindMaximumImpulseContact(body);
            
            if (collision.NormalImpulse < Strength)
                return;

            var direction = Vector2F.UnitY * ExplosionRadius;
            var angle = MathF.PI * 2 / ExplosionFragCount;
            var rand = new Random();
            for (var i = 0; i < ExplosionFragCount; i++)
            {
                var frag = LevelUtility.CreatePolygon(Entity.Position + direction.Normalized * 0.5f,
                    false, GeometryUtils.GenerateRegularPolygon(Vector2F.Zero,
                        Vector2F.One * 0.1f, 0, rand.Next(3, 6)).ToArray());
                StartCoroutine(ApplyFragImpulse);
                GameState.AddEntity(frag);
                frag.Destroy(0.3f);
                frags.Add(frag.GetComponent<PhysicsBody>());
                
                //GameState.PhysicsWorld.RayCast(RaycastCallback, body.FarseerBody.Position, direction);
                direction = direction.Rotate(angle);
            }
        }

        private IEnumerable<Awaiter> ApplyFragImpulse()
        {
            yield return Awaiter.WaitForFrames(2);
            foreach (var frag in frags)
            {
                var direction = frag.FarseerBody.Position - Entity.Position;
                frag.FarseerBody.ApplyLinearImpulse(-direction);
            }
            
            yield return Awaiter.WaitForNextFrame();

            Entity.Destroy();
        }

        private float RaycastCallback(Fixture fixture, Vector2 point1, Vector2 point2, float distance)
        {
            fixture.Body.ApplyLinearImpulse((point1 - point2) * distance * ExplosionFragImpulse);
            fixture.Body.ApplyAngularImpulse(ExplosionFragImpulse / 10);
            return distance;
        }
    }
}