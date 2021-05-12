using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using GameProject.GameMath;
using WFInterpolationMode = System.Drawing.Drawing2D.InterpolationMode;

namespace GameProject.GameGraphics.Backend.WinForms
{
    /// <summary>
    ///     An implementation of <see cref="IGraphicsDevice" /> for WinForms
    /// </summary>
    // ReSharper disable once ClassNeverInstantiated.Global
    internal class WinFormsGraphicsDevice : IGraphicsDevice
    {
        public Graphics Graphics { get; set; }

        public IBitmap CreateBitmap(byte[] imageData)
        {
            using (var stream = new MemoryStream(imageData))
            {
                return new WinFormsBitmap(new Bitmap(stream));
            }
        }

        public void SetTransform(Matrix3F transformMatrix)
        {
            Graphics.ResetTransform();
            Graphics.MultiplyTransform(transformMatrix);
        }

        public void BeginRender()
        {
        }

        public void EndRender()
        {
        }

        public void SetInterpolationMode(InterpolationMode mode)
        {
            switch (mode)
            {
                case InterpolationMode.Linear:
                    Graphics.InterpolationMode = WFInterpolationMode.Bilinear;
                    break;
                case InterpolationMode.Nearest:
                    Graphics.InterpolationMode = WFInterpolationMode.NearestNeighbor;
                    break;
                default:
                    throw new NotImplementedException($"Interpolation mode '{mode}' is not implemented");
            }
        }

        public void DrawBitmap(IBitmap bitmap, Vector2F location, Vector2F scale, float opacity)
        {
            if (bitmap is WinFormsBitmap bm)
                Graphics.DrawImage(bm.NativeImage, bm.Origin + location);
        }

        public void DrawRectangle(Vector2F location, Vector2F size, Color color)
        {
            Graphics.FillRectangle(new SolidBrush(color), new RectangleF(location, size));
        }

        public void DrawLine(Vector2F start, Vector2F end, Color color, float weight)
        {
            Graphics.DrawLine(new Pen(color, weight), start, end);
        }

        public void FillEllipse(Vector2F location, Vector2F size, Color color)
        {
            Graphics.FillEllipse(new SolidBrush(color), new RectangleF(location, size));
        }

        public void DrawEllipse(Vector2F location, Vector2F size, Color color, float weight = 0.01f)
        {
            Graphics.DrawEllipse(new Pen(color, weight), new RectangleF(location, size));
        }

        public void FillPolygon(IEnumerable<Vector2F> points, Color color)
        {
            Graphics.FillPolygon(new SolidBrush(color), points.Select(x => (PointF)x).ToArray());
        }
    }
}