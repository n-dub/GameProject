using System.Drawing;
using System.Linq;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics;
using GameProject.CoreEngine;
using GameProject.GameDebug;
using GameProject.GameMath;
using Microsoft.Xna.Framework;

namespace GameProject.Ecs.Physics
{
    /// <summary>
    ///     Represents a dynamic or static physical rigid body
    /// </summary>
    internal class PhysicsBody : IGameComponent, IDebuggable
    {
        /// <summary>
        ///     If True this body's state won't be changed by any other bodies
        /// </summary>
        public bool IsStatic { get; set; }

        public GameEntity Entity { get; set; }
        public Body FarseerBody { get; private set; }

        private Shape shape;

        public void Update(GameState state)
        {
            if (FarseerBody is null)
                Initialize(state);
            FarseerBody.IsStatic = IsStatic;
            Entity.Position = new Vector2F(FarseerBody.Position);
            Entity.Rotation = FarseerBody.Rotation;
        }

        public void Destroy(GameState state)
        {
            state.PhysicsWorld.RemoveBody(FarseerBody);
        }

        public void DrawDebugOverlay(DebugDraw debugDraw)
        {
            if (FarseerBody is null)
                return;
            debugDraw.DrawVector(Entity.Position, new Vector2F(FarseerBody.LinearVelocity), Color.Blue);
        }
        
        private void Initialize(GameState state)
        {
            FarseerBody = new Body(state.PhysicsWorld, Entity.Position, Entity.Rotation);
            shape = Entity
                .GetComponentsOfType<Collider>()
                .Single()
                .GetFarseerShape();
            FarseerBody.CreateFixture(shape);
            FarseerBody.IsStatic = IsStatic;
        }

        public void ApplyForce(Vector2F force)
        {
            FarseerBody?.ApplyForce(force);
        }
    }
}