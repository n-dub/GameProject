﻿using GameProject.CoreEngine;
using GameProject.GameGraphics;

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
        public IRenderShape Shape { get; }

        public GameEntity Entity { get; set; }

        /// <summary>
        ///     Create a new <see cref="Sprite" /> component
        /// </summary>
        /// <param name="shape">An instance of <see cref="IRenderShape" /> to connect to game object</param>
        public Sprite(IRenderShape shape)
        {
            Shape = shape;
        }

        public void Initialize(GameState state)
        {
            state.Renderer.AddShape(Shape);
            Shape.Transform = Entity.GlobalTransform;
            Shape.Initialize(state.Renderer.Device);
        }

        public void Update(GameState state)
        {
            Shape.Transform = Entity.GlobalTransform;
        }
    }
}