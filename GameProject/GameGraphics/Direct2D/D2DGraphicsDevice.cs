using System;
using System.Windows.Forms;
using GameProject.GameMath;
using unvell.D2DLib;

namespace GameProject.GameGraphics.Direct2D
{
    internal class D2DGraphicsDevice : IGraphicsDevice
    {
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
        
        private InterpolationMode interpolationMode;

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

        public IBitmap CreateBitmap(byte[] imageData) => new D2DGraphicsBitmap(Device.LoadBitmap(imageData));

        public void SetTransform(Matrix3F transformMatrix) => Graphics.SetTransform(transformMatrix);

        public void BeginRender() => Graphics.BeginRender(D2DColor.FromGDIColor(Form.BackColor));

        public void EndRender() => Graphics.EndRender();

        public void SetInterpolationMode(InterpolationMode mode) => interpolationMode = mode;

        public void DrawBitmap(IBitmap bitmap)
        {
            if (!(bitmap is D2DGraphicsBitmap bm))
                return;
            
            var imageCenter = new Vector2F(bm.NativeBitmap.Width / 2, bm.NativeBitmap.Height / 2);
            var imageOrigin = bm.Origin - imageCenter;
            var rect = new D2DRect(imageOrigin.X, imageOrigin.Y, bm.NativeBitmap.Width, bm.NativeBitmap.Height);
            
            Graphics.TranslateTransform(imageCenter.X, imageCenter.Y);
            Graphics.ScaleTransform(1 / bm.NativeBitmap.Width, 1 / bm.NativeBitmap.Height);
            Graphics.DrawBitmap(bm.NativeBitmap, rect, 1f, D2DInterpolationMode);
            Graphics.TranslateTransform(-imageCenter.X, -imageCenter.Y);
        }

        public void PushLayer() => Graphics.PushLayer();

        public void PopLayer() => Graphics.PopLayer();
    }
}