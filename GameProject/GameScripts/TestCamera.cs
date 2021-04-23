using System.Windows.Forms;
using GameProject.CoreEngine;
using GameProject.GameInput;
using GameProject.GameMath;

namespace GameProject.GameScripts
{
    internal class TestCamera : GameScript
    {
        private GameState GameState;

        private void ControlPosition()
        {
            var p = GameState.Renderer.Camera.Position;
            var (x, y) = (p.X, p.Y);
            if (GameState.Keyboard[Keys.Left] == KeyState.Pushing)
                x -= 0.1f;
            if (GameState.Keyboard[Keys.Right] == KeyState.Pushing)
                x += 0.1f;
            if (GameState.Keyboard[Keys.Up] == KeyState.Pushing)
                y -= 0.1f;
            if (GameState.Keyboard[Keys.Down] == KeyState.Pushing)
                y += 0.1f;

            GameState.Renderer.Camera.Position = new Vector2F(x, y);
        }

        private void ControlRotationZoom()
        {
            if (GameState.Keyboard[Keys.W] == KeyState.Pushing)
                GameState.Renderer.Camera.ViewWidth /= 1.01f;
            if (GameState.Keyboard[Keys.S] == KeyState.Pushing)
                GameState.Renderer.Camera.ViewWidth *= 1.01f;
            if (GameState.Keyboard[Keys.A] == KeyState.Pushing)
                GameState.Renderer.Camera.Rotation -= 0.01f;
            if (GameState.Keyboard[Keys.D] == KeyState.Pushing)
                GameState.Renderer.Camera.Rotation += 0.01f;
        }

        public override void Update(GameState state)
        {
            GameState = state;
            ControlRotationZoom();
            ControlPosition();
        }
    }
}