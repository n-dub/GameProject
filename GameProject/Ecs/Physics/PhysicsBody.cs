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

        private Body body;

        private Shape shape;

        public void Initialize(GameState state)
        {
            body = new Body(state.PhysicsWorld, Entity.Position, Entity.Rotation);
            shape = Entity
                .GetComponentsOfType<Collider>()
                .Single()
                .GetFarseerShape();
            body.CreateFixture(shape);
            body.IsStatic = IsStatic;
        }

        public void Update(GameState state)
        {
            body.IsStatic = IsStatic;
            Entity.Position = new Vector2F(body.Position);
            Entity.Rotation = body.Rotation;
        }

        public void Destroy(GameState state)
        {
            state.PhysicsWorld.RemoveBody(body);
        }

        public void DrawDebugOverlay(DebugDraw debugDraw)
        {
            if (body is null)
                return;
            debugDraw.DrawVector(Entity.Position, new Vector2F(body.LinearVelocity), Color.Blue);
        }
    }
}