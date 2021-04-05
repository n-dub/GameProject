using System;
using System.Drawing;
using System.IO;
using GameProject.GameMath;
using WFInterpolationMode = System.Drawing.Drawing2D.InterpolationMode;

namespace GameProject.GameGraphics.WinForms
{
    internal class WinFormsGraphicsDevice : IGraphicsDevice
    {
        public Graphics Graphics { get; set; }

        internal WinFormsGraphicsDevice(Graphics graphics) =>
            Graphics = graphics;

        public IBitmap CreateBitmap(byte[] imageData)
        {
            using (var stream = new MemoryStream(imageData))
                return new WinFormsBitmap(new Bitmap(stream));
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
    }
}