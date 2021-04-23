using GameProject.GameMath;
using unvell.D2DLib;

namespace GameProject.GameGraphics.Direct2D
{
    /// <summary>
    ///     An implementation of <see cref="IBitmap" /> for Direct2D render device
    /// </summary>
    internal class D2DGraphicsBitmap : IBitmap
    {
        /// <summary>
        ///     A native bitmap - an instance of <see cref="D2DBitmap" />
        /// </summary>
        public D2DBitmap NativeBitmap { get; }

        public Vector2F Origin { get; set; }

        /// <summary>
        ///     Create a new <see cref="D2DGraphicsBitmap" /> from a native bitmap
        /// </summary>
        /// <param name="nativeBitmap">A native bitmap to use</param>
        public D2DGraphicsBitmap(D2DBitmap nativeBitmap)
        {
            NativeBitmap = nativeBitmap;
            Origin = Vector2F.Zero;
        }
    }
}