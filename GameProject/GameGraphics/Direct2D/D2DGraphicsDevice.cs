using System;
using System.Windows.Forms;
using GameProject.GameMath;
using unvell.D2DLib;

namespace GameProject.GameGraphics.Direct2D
{
    internal class D2DGraphicsDevice : IGraphicsDevice
    {
        public D2DDevice Device { get; }
        private D2DGraphics Graphics { get; }
        private Form Form { get; }

        private InterpolationMode interpolationMode;

        public D2DGraphicsDevice(Form form)
        {
            Form = form;
            Device = D2DDevice.FromHwnd(Form.Handle);
            Form.Resize += (sender, args) => Device.Resize();
            Form.Resize += (sender, args) => Form.Invalidate(false);
            Form.HandleDestroyed += (sender, args) => Device.Dispose();
            Graphics = new D2DGraphics(Device);
            Graphics.SetDPI(96, 96);
        }

        public IBitmap CreateBitmap(byte[] imageData) => new D2DGraphicsBitmap(Device.LoadBitmap(imageData));

        public void SetTransform(Matrix3F transformMatrix) => Graphics.SetTransform(transformMatrix);

        public void BeginRender() => Graphics.BeginRender(D2DColor.FromGDIColor(Form.BackColor));

        public void EndRender() => Graphics.EndRender();

        public void SetInterpolationMode(InterpolationMode mode) => interpolationMode = mode;

        public void DrawBitmap(IBitmap bitmap)
        {
            if (!(bitmap is D2DGraphicsBitmap bm))
                return;
            var rect = new D2DRect(bm.Origin.X, bm.Origin.Y,
                bm.NativeBitmap.Width, bm.NativeBitmap.Height);
            switch (interpolationMode)
            {
                case InterpolationMode.Linear:
                    Graphics.DrawBitmap(bm.NativeBitmap, rect);
                    break;
                case InterpolationMode.Nearest:
                    Graphics.DrawBitmap(bm.NativeBitmap, rect, 1f, D2DBitmapInterpolationMode.NearestNeighbor);
                    break;
                default:
                    throw new NotImplementedException(
                        $"Interpolation mode '{interpolationMode}' is not implemented");
            }
        }
    }
}