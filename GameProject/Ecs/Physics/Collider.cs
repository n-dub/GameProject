using FarseerPhysics.Collision.Shapes;
using GameProject.CoreEngine;

namespace GameProject.Ecs.Physics
{
    internal abstract class Collider : IGameComponent
    {
        public GameEntity Entity { get; set; }

        /// <summary>
        /// Colliders are initialized by <see cref="PhysicsBody"/>
        /// </summary>
        /// <param name="state"></param>
        public void Initialize(GameState state)
        {
        }

        /// <summary>
        /// Colliders are updated by <see cref="PhysicsBody"/>
        /// </summary>
        /// <param name="state"></param>
        public void Update(GameState state)
        {
        }

        public abstract Shape GetFarseerShape();
    }
}