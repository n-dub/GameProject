using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using GameProject.CoreEngine;
using GameProject.GameGraphics.RenderShapes;

namespace GameProject.Ecs.Graphics
{
    /// <summary>
    ///     A component that connects physical representation of an object (<see cref="GameEntity" />)
    ///     with the graphical representation (<see cref="IRenderShape" />)
    /// </summary>
    internal class Sprite : IGameComponent
    {
        /// <summary>
        ///     An instance of <see cref="IRenderShape" /> used by the entity
        /// </summary>
        public List<IRenderShape> Shapes { get; }

        public GameEntity Entity { get; set; }

        private bool initialized;

        /// <summary>
        ///     Create a new <see cref="Sprite" /> component
        /// </summary>
        /// <param name="shapes">Instances of <see cref="IRenderShape" /> to connect to game object</param>
        public Sprite(IEnumerable<IRenderShape> shapes)
        {
            Shapes = shapes.ToList();
        }

        /// <summary>
        ///     Create a new <see cref="Sprite" /> component
        /// </summary>
        /// <param name="shape">An instances of <see cref="IRenderShape" /> to connect to game object</param>
        public Sprite(IRenderShape shape)
        {
            Shapes = new List<IRenderShape>(4) {shape};
        }

        /// <summary>
        ///     Create a new <see cref="Sprite" /> component
        /// </summary>
        public Sprite()
        {
            Shapes = new List<IRenderShape>(4);
        }

        public void Update(GameState state)
        {
            if (!initialized)
                Initialize(state);

            foreach (var shape in Shapes)
                shape.Transform = Entity.GlobalTransform;
        }

        public void Destroy(GameState state)
        {
            foreach (var shape in Shapes)
                state.Renderer.RemoveShape(shape);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Initialize(GameState state)
        {
            foreach (var shape in Shapes)
            {
                state.Renderer.AddShape(shape);
                shape.Initialize(state.Renderer.Device);
                initialized = true;
            }
        }
    }
}