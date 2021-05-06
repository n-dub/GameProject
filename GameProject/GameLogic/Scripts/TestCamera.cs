using System.Windows.Forms;
using GameProject.CoreEngine;
using GameProject.GameInput;
using GameProject.GameMath;

namespace GameProject.GameLogic.Scripts
{
    internal class TestCamera : GameScript
    {
        private static void ControlPosition(GameState gameState, float speedFactor)
        {
            var p = gameState.Renderer.Camera.Position;
            var (x, y) = (p.X, p.Y);
            const float speed = .3f;
            if (gameState.Keyboard[Keys.Left] == KeyState.Pushing)
                x -= speed * gameState.Time.DeltaTimeUnscaled * speedFactor;
            if (gameState.Keyboard[Keys.Right] == KeyState.Pushing)
                x += speed * gameState.Time.DeltaTimeUnscaled * speedFactor;
            if (gameState.Keyboard[Keys.Up] == KeyState.Pushing)
                y -= speed * gameState.Time.DeltaTimeUnscaled * speedFactor;
            if (gameState.Keyboard[Keys.Down] == KeyState.Pushing)
                y += speed * gameState.Time.DeltaTimeUnscaled * speedFactor;

            gameState.Renderer.Camera.Position = new Vector2F(x, y);
        }

        private static void ControlRotationZoom(GameState gameState, float speedFactor)
        {
            const float rotSpeed = MathF.PI / 4;
            if (gameState.Keyboard[Keys.W] == KeyState.Pushing)
                gameState.Renderer.Camera.ViewWidth -= gameState.Time.DeltaTimeUnscaled * speedFactor;
            if (gameState.Keyboard[Keys.S] == KeyState.Pushing)
                gameState.Renderer.Camera.ViewWidth += gameState.Time.DeltaTimeUnscaled * speedFactor;
            // if (gameState.Keyboard[Keys.A] == KeyState.Pushing)
            //     gameState.Renderer.Camera.Rotation -= rotSpeed * gameState.Time.DeltaTimeUnscaled;
            // if (gameState.Keyboard[Keys.D] == KeyState.Pushing)
            //     gameState.Renderer.Camera.Rotation += rotSpeed * gameState.Time.DeltaTimeUnscaled;
        }

        protected override void Update()
        {
            var speedFactor = GameState.Renderer.Camera.ViewWidth;
            ControlRotationZoom(GameState, speedFactor);
            ControlPosition(GameState, speedFactor);

            if (GameState.Keyboard[Keys.Q] == KeyState.Down)
                Entity.Destroy();
        }
    }
}