using System;
using System.Drawing;
using System.Runtime.CompilerServices;
using GameProject.GameGraphics;
using GameProject.GameMath;

namespace GameProject.GameDebug
{
    /// <summary>
    ///     Debug drawing utility, unlike <see cref="Renderer" /> draws immediately and
    ///     doesn't store render shapes in queue, therefore is easier to use for debugging purposes
    /// </summary>
    internal class DebugDraw
    {
        /// <summary>
        ///     Currently used instance of <see cref="IGraphicsDevice" />
        /// </summary>
        private Renderer Renderer { get; }

        private float LineWeightFactor
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Renderer.Camera.ViewWidth / 30;
        }

        /// <summary>
        ///     Create a new <see cref="DebugDraw"/> using a certain <see cref="GameProject.GameGraphics.Renderer"/>
        /// </summary>
        /// <param name="renderer">An instance of <see cref="GameProject.GameGraphics.Renderer"/> to use</param>
        public DebugDraw(Renderer renderer)
        {
            Renderer = renderer;
        }

        /// <summary>
        ///     Draw a vector on the screen
        /// </summary>
        /// <param name="origin">Location of the vector to draw</param>
        /// <param name="vector">Vector to draw itself</param>
        /// <param name="color">Color of the vector to draw</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DrawVector(Vector2F origin, Vector2F vector, Color color)
        {
            if (vector.IsZero)
                return;
            DrawImpl(device =>
            {
                device.DrawLine(origin, origin + vector, color, 0.01f * LineWeightFactor);
                var angle = vector.Angle;
                var arrow1 = new Vector2F(-0.15f, -0.1f).Rotate(angle);
                var arrow2 = new Vector2F(-0.15f, +0.1f).Rotate(angle);
                device.DrawLine(origin + vector, origin + vector + arrow1, color, 0.01f * LineWeightFactor);
                device.DrawLine(origin + vector, origin + vector + arrow2, color, 0.01f * LineWeightFactor);
            });
        }

        /// <summary>
        ///     Draw a straight line of the screen
        /// </summary>
        /// <param name="start">First point of the line to draw</param>
        /// <param name="end">Second point of the line to draw</param>
        /// <param name="color">Color of the line to draw</param>
        /// <param name="weight">Width of the line to draw</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DrawLine(Vector2F start, Vector2F end, Color color, float weight = 0.01f)
        {
            DrawImpl(device => device.DrawLine(start, end, color, weight * LineWeightFactor));
        }

        /// <summary>
        ///     Draw an ellipse of the screen
        /// </summary>
        /// <param name="location">Location of the ellipse to draw</param>
        /// <param name="size">Size of the ellipse to draw</param>
        /// <param name="color">Color of the ellipse to draw</param>
        /// <param name="weight">Stroke width of the ellipse to draw</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DrawEllipse(Vector2F location, Vector2F size, Color color, float weight = 0.01f)
        {
            DrawImpl(device => device.DrawEllipse(location, size, color, weight * LineWeightFactor));
        }

        /// <summary>
        ///     Draw an ellipse of the screen
        /// </summary>
        /// <param name="location">Location of the ellipse to draw</param>
        /// <param name="size">Size of the ellipse to draw</param>
        /// <param name="color">Color of the ellipse to draw</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void FillEllipse(Vector2F location, Vector2F size, Color color)
        {
            DrawImpl(device => device.FillEllipse(location, size, color));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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