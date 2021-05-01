using FarseerPhysics.Collision.Shapes;

namespace GameProject.Ecs.Physics
{
    /// <summary>
    ///     Represents an abstract collision model for an entity
    /// </summary>
    internal abstract class Collider
    {
        public GameEntity Entity { get; set; }

        /// <summary>
        ///     Convert this collision model to FarseerPhysics' <see cref="Shape" />
        /// </summary>
        /// <returns>An instance of <see cref="Shape" /> that corresponds to this collider</returns>
        public abstract Shape GetFarseerShape();
    }
}