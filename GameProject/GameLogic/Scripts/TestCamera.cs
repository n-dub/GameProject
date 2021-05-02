using System.Windows.Forms;
using GameProject.CoreEngine;
using GameProject.GameInput;
using GameProject.GameMath;

namespace GameProject.GameLogic.Scripts
{
    internal class TestCamera : GameScript
    {
        private void ControlPosition(GameState gameState, float speedFactor)
        {
            var p = gameState.Renderer.Camera.Position;
            var (x, y) = (p.X, p.Y);
            const float speed = .3f;
            if (gameState.Keyboard[Keys.Left] == KeyState.Pushing)
                x -= speed * gameState.Time.DeltaTime * speedFactor;
            if (gameState.Keyboard[Keys.Right] == KeyState.Pushing)
                x += speed * gameState.Time.DeltaTime * speedFactor;
            if (gameState.Keyboard[Keys.Up] == KeyState.Pushing)
                y -= speed * gameState.Time.DeltaTime * speedFactor;
            if (gameState.Keyboard[Keys.Down] == KeyState.Pushing)
                y += speed * gameState.Time.DeltaTime * speedFactor;

            gameState.Renderer.Camera.Position = new Vector2F(x, y);
        }

        private void ControlRotationZoom(GameState gameState, float speedFactor)
        {
            const float rotSpeed = MathF.PI / 4;
            if (gameState.Keyboard[Keys.W] == KeyState.Pushing)
                gameState.Renderer.Camera.ViewWidth -= gameState.Time.DeltaTime * speedFactor;
            if (gameState.Keyboard[Keys.S] == KeyState.Pushing)
                gameState.Renderer.Camera.ViewWidth += gameState.Time.DeltaTime * speedFactor;
            if (gameState.Keyboard[Keys.A] == KeyState.Pushing)
                gameState.Renderer.Camera.Rotation -= rotSpeed * gameState.Time.DeltaTime;
            if (gameState.Keyboard[Keys.D] == KeyState.Pushing)
                gameState.Renderer.Camera.Rotation += rotSpeed * gameState.Time.DeltaTime;
        }

        public override void Update(GameState state)
        {
            var speedFactor = state.Renderer.Camera.ViewWidth;
            ControlRotationZoom(state, speedFactor);
            ControlPosition(state, speedFactor);

            if (state.Keyboard[Keys.Q] == KeyState.Down)
                Entity.Destroy();
        }
    }
}