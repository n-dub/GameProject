using GameProject.GameMath;
using unvell.D2DLib;

namespace GameProject.GameGraphics.Direct2D
{
    internal class D2DGraphicsBitmap : IBitmap
    {
        public Vector2F Origin { get; set; }
        public D2DBitmap NativeBitmap { get; }

        internal D2DGraphicsBitmap(D2DBitmap nativeBitmap)
        {
            NativeBitmap = nativeBitmap;
            Origin = new Vector2F();
        }
    }
}