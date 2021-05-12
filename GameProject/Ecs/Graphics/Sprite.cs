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
        
        /// <summary>
        ///     True if sprite is background
        /// </summary>
        public bool IsBackground { get; }

        public GameEntity Entity { get; set; }

        private bool initialized;

        /// <summary>
        ///     Create a new <see cref="Sprite" /> component
        /// </summary>
        /// <param name="shapes">Instances of <see cref="IRenderShape" /> to connect to game object</param>
        /// <param name="isBackground">True if sprite is background</param>
        public Sprite(IEnumerable<IRenderShape> shapes, bool isBackground = false)
        {
            IsBackground = isBackground;
            Shapes = shapes.ToList();
        }

        /// <summary>
        ///     Create a new <see cref="Sprite" /> component
        /// </summary>
        /// <param name="shape">An instances of <see cref="IRenderShape" /> to connect to game object</param>
        /// <param name="isBackground">True if sprite is background</param>
        public Sprite(IRenderShape shape, bool isBackground = false)
        {
            Shapes = new List<IRenderShape>(4) {shape};
            IsBackground = isBackground;
        }

        public Sprite() : this(false)
        {
        }

        public Sprite(bool isBackground)
        {
            Shapes = new List<IRenderShape>(4);
            IsBackground = isBackground;
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
                state.RendererWrite.RemoveShape(shape);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Initialize(GameState state)
        {
            foreach (var shape in Shapes)
            {
                state.RendererWrite.AddShape(shape);
                shape.IsBackground = IsBackground;
                //shape.Initialize(state.RendererWrite.Device);
                initialized = true;
            }
        }
    }
}