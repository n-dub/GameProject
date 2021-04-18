using System.Linq;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics;
using GameProject.CoreEngine;
using GameProject.GameMath;

namespace GameProject.Ecs.Physics
{
    internal class PhysicsBody : IGameComponent
    {
        public GameEntity Entity { get; set; }
        public bool IsStatic { get; set; }

        private Shape shape;
        private Body body;

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
    }
}