using System;
using System.Collections.Generic;
using System.Linq;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Factories;
using GameProject.CoreEngine;
using GameProject.Ecs;
using GameProject.Ecs.Graphics;
using GameProject.Ecs.Physics;
using GameProject.GameGraphics.RenderShapes;
using GameProject.GameMath;
using Microsoft.Xna.Framework;

namespace GameProject.GameLogic.Scripts
{
    internal class ShellScript : GameScript
    {
        public static readonly List<GameEntity> Shells = new List<GameEntity>();
        public float Impulse { get; set; }

        public bool Exploded { get; set; }

        private Body bodyA;
        private Joint joint;

        public override void Destroy(GameState state)
        {
            var shell = LevelUtility.CreatePhysicalRectangle(Entity.Position,
                MachineEditor.CellSize * Vector2F.One * 0.9f, false, 1, false);
            if (shell.GetComponent<Sprite>().Shapes.First() is QuadRenderShape shape)
                shape.ImagePath = "Resources/machine_parts/shell.png";
            GameState.AddEntity(shell);
            Shells.Remove(Entity);
        }

        protected override void Update()
        {
            var raycastDir = Entity.Right * MachineEditor.CellSize * 0.55f;
            if (bodyA is null)
                GameState.PhysicsWorld.RayCast(ConnectProjectile, Entity.Position,
                    Entity.Position + raycastDir.Rotate(Entity.Rotation));
            if (!Exploded) return;

            Entity.Destroy();
            if (bodyA != null && joint != null)
            {
                GameState.PhysicsWorld.RemoveJoint(joint);
                bodyA.ApplyLinearImpulse(Entity.Right * Impulse);
                Entity.GetComponent<PhysicsBody>().ApplyLinearImpulse(Entity.Right * -Impulse);
            }

            OnExplode?.Invoke();
        }

        private float ConnectProjectile(Fixture fixture, Vector2 point1, Vector2 point2, float d)
        {
            bodyA = fixture.Body;
            var bodyB = Entity.GetComponent<PhysicsBody>().FarseerBody;
            joint = JointFactory.CreateDistanceJoint(GameState.PhysicsWorld, bodyA,
                bodyB, bodyA.Position, bodyB.Position);
            return 0.0f;
        }

        public event Action OnExplode;
    }
}