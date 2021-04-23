using System.Windows.Forms;
using GameProject.CoreEngine;
using GameProject.GameInput;
using GameProject.GameMath;

namespace GameProject.GameLogic.Scripts
{
    internal class TestCamera : GameScript
    {
        private GameState GameState;
        private float SpeedFactor => GameState.Renderer.Camera.ViewWidth;

        private void ControlPosition()
        {
            var p = GameState.Renderer.Camera.Position;
            var (x, y) = (p.X, p.Y);
            const float speed = .3f;
            if (GameState.Keyboard[Keys.Left] == KeyState.Pushing)
                x -= speed * GameState.Time.DeltaTime * SpeedFactor;
            if (GameState.Keyboard[Keys.Right] == KeyState.Pushing)
                x += speed * GameState.Time.DeltaTime * SpeedFactor;
            if (GameState.Keyboard[Keys.Up] == KeyState.Pushing)
                y -= speed * GameState.Time.DeltaTime * SpeedFactor;
            if (GameState.Keyboard[Keys.Down] == KeyState.Pushing)
                y += speed * GameState.Time.DeltaTime * SpeedFactor;

            GameState.Renderer.Camera.Position = new Vector2F(x, y);
        }

        private void ControlRotationZoom()
        {
            const float rotSpeed = MathF.PI / 4;
            if (GameState.Keyboard[Keys.W] == KeyState.Pushing)
                GameState.Renderer.Camera.ViewWidth -= GameState.Time.DeltaTime * SpeedFactor;
            if (GameState.Keyboard[Keys.S] == KeyState.Pushing)
                GameState.Renderer.Camera.ViewWidth += GameState.Time.DeltaTime * SpeedFactor;
            if (GameState.Keyboard[Keys.A] == KeyState.Pushing)
                GameState.Renderer.Camera.Rotation -= rotSpeed * GameState.Time.DeltaTime;
            if (GameState.Keyboard[Keys.D] == KeyState.Pushing)
                GameState.Renderer.Camera.Rotation += rotSpeed * GameState.Time.DeltaTime;
        }

        public override void Update(GameState state)
        {
            GameState = state;
            ControlRotationZoom();
            ControlPosition();
        }
    }
}