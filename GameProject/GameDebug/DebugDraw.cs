using System;
using System.Drawing;
using GameProject.GameGraphics;
using GameProject.GameMath;

namespace GameProject.GameDebug
{
    /// <summary>
    ///     Debug drawing utility, unlike <see cref="Renderer"/> draws immediately and
    ///     doesn't store render shapes in queue, therefore is easier to use for debugging purposes
    /// </summary>
    internal class DebugDraw
    {
        /// <summary>
        ///     Currently used instance of <see cref="IGraphicsDevice" />
        /// </summary>
        public Renderer Renderer { get; }

        public DebugDraw(Renderer renderer)
        {
            Renderer = renderer;
        }

        public void DrawVector(Vector2F origin, Vector2F vector, Color color, Matrix3F transform = null)
        {
            DrawImpl(device =>
            {
                device.DrawLine(origin, origin + vector, color);
                var arrow1 = (vector * 2 + vector.Rotate(MathF.PI / 2)) / -10;
                var arrow2 = (vector * 2 + vector.Rotate(MathF.PI / -2)) / -10;
                device.DrawLine(vector + origin, vector + origin + arrow1, color);
                device.DrawLine(vector + origin, vector + origin + arrow2, color);
            });
        }

        public void DrawLine(Vector2F start, Vector2F end, Color color, float weight = 0.01f)
        {
            DrawImpl(device => device.DrawLine(start, end, color, weight));
        }

        private void DrawImpl(Action<IGraphicsDevice> drawAction)
        {
            var device = Renderer.Device;
            device.SetInterpolationMode(InterpolationMode.Linear);
            device.SetTransform(Renderer.Camera.GetViewMatrix());
            drawAction(device);
            device.SetTransform(Matrix3F.Identity);
        }
    }
}