using FarseerPhysics.Collision.Shapes;

namespace GameProject.Ecs.Physics
{
    /// <summary>
    ///     Represents an abstract collision model for an entity
    /// </summary>
    internal abstract class Collider
    {
        public GameEntity Entity { get; set; }
        public float Density { get; set; } = 1;

        /// <summary>
        ///     Convert this collision model to FarseerPhysics' <see cref="Shape" />
        /// </summary>
        /// <returns>An instance of <see cref="Shape" /> that corresponds to this collider</returns>
        public Shape GetFarseerShape()
        {
            var shape = GetFarseerShapeImpl();
            shape.Density = Density;
            return shape;
        }

        protected abstract Shape GetFarseerShapeImpl();
    }
}