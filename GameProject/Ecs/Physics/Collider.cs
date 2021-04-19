using FarseerPhysics.Collision.Shapes;
using GameProject.CoreEngine;

namespace GameProject.Ecs.Physics
{
    /// <summary>
    ///     Represents an abstract collision model for an entity
    /// </summary>
    internal abstract class Collider : IGameComponent
    {
        public GameEntity Entity { get; set; }

        /// <summary>
        ///     Colliders are initialized by <see cref="PhysicsBody" />
        /// </summary>
        /// <param name="state"></param>
        public void Initialize(GameState state)
        {
        }

        /// <summary>
        ///     Colliders are updated by <see cref="PhysicsBody" />
        /// </summary>
        /// <param name="state"></param>
        public void Update(GameState state)
        {
        }

        /// <summary>
        ///     Convert this collision model to FarseerPhysics' <see cref="Shape" />
        /// </summary>
        /// <returns>An instance of <see cref="Shape" /> that corresponds to this collider</returns>
        public abstract Shape GetFarseerShape();
    }
}