using GameProject.GameMath;

namespace GameProject.GameGraphics
{
    internal interface IGraphicsDevice
    {
        IBitmap CreateBitmap(byte[] imageData);
        void SetTransform(Matrix3F transformMatrix);
        void BeginRender();
        void EndRender();
        void SetInterpolationMode(InterpolationMode mode);
        void DrawBitmap(IBitmap bitmap);
    }
}