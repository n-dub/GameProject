using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using GameProject.GameMath;
using unvell.D2DLib;

namespace GameProject.GameGraphics.Backend.Direct2D
{
    /// <summary>
    ///     An implementation of <see cref="IGraphicsDevice" /> for Direct2D
    /// </summary>
    internal class D2DGraphicsDevice : IGraphicsDevice
    {
        private InterpolationMode interpolationMode;

        private D2DDevice Device { get; }
        private D2DGraphics Graphics { get; }
        private Form Form { get; }

        private D2DBitmapInterpolationMode D2DInterpolationMode
        {
            get
            {
                switch (interpolationMode)
                {
                    case InterpolationMode.Linear:
                        return D2DBitmapInterpolationMode.Linear;
                    case InterpolationMode.Nearest:
                        return D2DBitmapInterpolationMode.NearestNeighbor;
                    default:
                        throw new InvalidOperationException(
                            $"Interpolation mode '{interpolationMode}' is not implemented");
                }
            }
        }

        public D2DGraphicsDevice(Form form)
        {
            Form = form;
            Device = D2DDevice.FromHwnd(Form.Handle);
            Device.Resize();
            Form.Resize += (sender, args) => Device.Resize();
            Form.HandleDestroyed += (sender, args) => Device.Dispose();
            Graphics = new D2DGraphics(Device);
            Graphics.SetDPI(96, 96);
        }

        public IBitmap CreateBitmap(byte[] imageData)
        {
            return new D2DGraphicsBitmap(Device.LoadBitmap(imageData));
        }

        public void SetTransform(Matrix3F transformMatrix)
        {
            Graphics.SetTransform(transformMatrix);
        }

        public void BeginRender()
        {
            Graphics.BeginRender(D2DColor.FromGDIColor(Form.BackColor));
        }

        public void EndRender()
        {
            Graphics.EndRender();
        }

        public void SetInterpolationMode(InterpolationMode mode)
        {
            interpolationMode = mode;
        }

        public void DrawBitmap(IBitmap bitmap, Vector2F location, Vector2F scale)
        {
            if (!(bitmap is D2DGraphicsBitmap bm))
                return;

            var rect = new D2DRect(location, scale);
            Graphics.DrawBitmap(bm.NativeBitmap, rect, 1, D2DInterpolationMode);
        }

        public void DrawRectangle(Vector2F location, Vector2F size, Color color)
        {
            Graphics.ScaleTransform(0.5f, 0.5f);
            Graphics.DrawRectangle(location, size, D2DColor.FromGDIColor(color));
        }

        public void DrawLine(Vector2F start, Vector2F end, Color color, float weight)
        {
            Graphics.DrawLine(start, end, D2DColor.FromGDIColor(color), weight);
        }

        public void FillEllipse(Vector2F location, Vector2F size, Color color)
        {
            var radial = size / 2;
            var origin = location + radial;
            Graphics.ScaleTransform(0.5f, 0.5f);
            Graphics.DrawEllipse(origin, radial, D2DColor.FromGDIColor(color));
        }

        public void DrawEllipse(Vector2F location, Vector2F size, Color color, float weight = 0.01f)
        {
            var radial = size / 2;
            var origin = location + radial;
            Graphics.DrawEllipse(origin, radial, D2DColor.FromGDIColor(color), weight);
        }

        public void FillPolygon(IEnumerable<Vector2F> points, Color color)
        {
#pragma warning disable 618
            // the new method doesn't work properly, so I have to use the obsolete one here
            Graphics.FillPolygon(points.Select(x => (D2DPoint) x).ToArray(), D2DColor.FromGDIColor(color));
#pragma warning restore 618
        }
    }
}