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
            if (GameState.Keyboard[Keys.Left] == KeyState.Down)
                x -= 1;
            if (GameState.Keyboard[Keys.Right] == KeyState.Down)
                x += 1;
            if (GameState.Keyboard[Keys.Up] == KeyState.Down)
                y -= 1;
            if (GameState.Keyboard[Keys.Down] == KeyState.Down)
                y += 1;

            GameState.Renderer.Camera.Position = new Vector2F(x, y);
        }

        private void ControlRotationZoom()
        {
            if (GameState.Keyboard[Keys.W] == KeyState.Down)
                GameState.Renderer.Camera.ViewWidth -= 10;
            if (GameState.Keyboard[Keys.S] == KeyState.Down)
                GameState.Renderer.Camera.ViewWidth += 10;
            if (GameState.Keyboard[Keys.A] == KeyState.Down)
                GameState.Renderer.Camera.Rotation -= 0.1f;
            if (GameState.Keyboard[Keys.D] == KeyState.Down)
                GameState.Renderer.Camera.Rotation += 0.1f;
        }

        public override void Update(GameState state)
        {
            GameState = state;
            ControlRotationZoom();
            ControlPosition();
        }
    }
}