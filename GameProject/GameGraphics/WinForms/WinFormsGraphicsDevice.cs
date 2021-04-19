using System;
using System.Drawing;
using System.IO;
using GameProject.GameMath;
using WFInterpolationMode = System.Drawing.Drawing2D.InterpolationMode;

namespace GameProject.GameGraphics.WinForms
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

        public void DrawBitmap(IBitmap bitmap)
        {
            if (bitmap is WinFormsBitmap bm)
                Graphics.DrawImage(bm.NativeImage, bm.Origin);
        }

        public void DrawRectangle(Vector2F location, Vector2F size, Color color)
        {
            Graphics.FillRectangle(new SolidBrush(color), new RectangleF(location, new SizeF(size)));
        }

        public void PushLayer()
        {
        }

        public void PopLayer()
        {
        }
    }
}