using GameProject.CoreEngine;
using GameProject.GameGraphics;

namespace GameProject.Ecs.Graphics
{
    internal class Sprite : IGameComponent
    {
        public GameEntity Entity { get; set; }
        
        public IRenderShape Shape { get; }

        public Sprite(IRenderShape shape) => Shape = shape;

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