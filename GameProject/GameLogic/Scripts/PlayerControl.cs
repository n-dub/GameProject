using System.Windows.Forms;
using GameProject.CoreEngine;
using GameProject.GameInput;
using GameProject.GameMath;

namespace GameProject.GameLogic.Scripts
{
    internal class PlayerControl : GameScript
    {
        private const float ZoomFactor = 0.0007f;
        private Vector2F previousMouse;
        private Vector2F previousCamera;

        protected override void Update()
        {
            if (GameState.Keyboard[Keys.R] == KeyState.Down && !MachineEditor.HasInstance)
                Application.Restart();

            var mousePosition = GameState.Mouse.ScreenPosition;
            var viewWidth = GameState.RendererWrite.Camera.ViewWidth;

            switch (GameState.Mouse[MouseButtons.Right])
            {
                case KeyState.Down:
                    previousMouse = mousePosition;
                    previousCamera = GameState.RendererWrite.Camera.Position;
                    return;
                case KeyState.Pushing:
                    var delta = mousePosition - previousMouse;
                    GameState.RendererWrite.Camera.Position = previousCamera - delta * viewWidth;
                    break;
            }

            var v = GameState.RendererWrite.Camera.ViewWidth - ZoomFactor * GameState.Mouse.WheelDelta * viewWidth;
            GameState.RendererWrite.Camera.ViewWidth = MathF.Clamp(v, 1.5f, 30f);
        }
    }
}