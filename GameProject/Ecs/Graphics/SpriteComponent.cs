using GameProject.CoreEngine;
using GameProject.GameGraphics;

namespace GameProject.Ecs.Graphics
{
    internal class SpriteComponent : IGameComponent
    {
        public GameEntity Entity { get; set; }
        
        public IRenderShape Shape { get; }

        public SpriteComponent(IRenderShape shape) => Shape = shape;

        public void Initialize(GameState state)
        {
            state.Renderer.AddShape(Shape);
            Shape.Initialize(state.Renderer.Device);
        }

        public void Update(GameState state)
        {
        }
    }
}