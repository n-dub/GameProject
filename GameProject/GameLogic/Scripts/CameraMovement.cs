using System.Windows.Forms;
using GameProject.CoreEngine;
using GameProject.GameInput;
using GameProject.GameMath;

namespace GameProject.GameLogic.Scripts
{
    internal class CameraMovement : GameScript
    {
        private const float ZoomFactor = 0.0007f;
        private Vector2F previousMouse;
        private Vector2F previousCamera;

        protected override void Update()
        {
            var mousePosition = GameState.Mouse.ScreenPosition;
            var viewWidth = GameState.RendererWrite.Camera.ViewWidth;
            
            if (GameState.Mouse[MouseButtons.Right] == KeyState.Down)
            {
                GameProfiler.MakeEvent($"MR down {mousePosition}");
                previousMouse = mousePosition;
                previousCamera = GameState.RendererWrite.Camera.Position;
                return;
            }

            GameState.RendererWrite.Camera.ViewWidth -= ZoomFactor * GameState.Mouse.WheelDelta * viewWidth;

            if (GameState.Mouse[MouseButtons.Right] == KeyState.Pushing)
            {
                GameProfiler.MakeEvent($"MR push {mousePosition}");
                var delta = mousePosition - previousMouse;
                GameState.RendererWrite.Camera.Position = previousCamera - delta * viewWidth;
            }
        }
    }
}