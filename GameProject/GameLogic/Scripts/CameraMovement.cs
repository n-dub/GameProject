using System;
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

            GameState.RendererWrite.Camera.ViewWidth -= ZoomFactor * GameState.Mouse.WheelDelta * viewWidth;
        }
    }
}