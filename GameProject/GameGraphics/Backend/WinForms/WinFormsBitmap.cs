using System.Drawing;
using GameProject.GameMath;

namespace GameProject.GameGraphics.Backend.WinForms
{
    /// <summary>
    ///     An implementation of <see cref="IBitmap" /> for WinForms render device
    /// </summary>
    internal class WinFormsBitmap : IBitmap
    {
        /// <summary>
        ///     A native image - an instance of <see cref="Image" />
        /// </summary>
        public Image NativeImage { get; }

        public Vector2F Origin { get; set; }

        /// <summary>
        ///     Create a new <see cref="WinFormsBitmap" /> from a native bitmap
        /// </summary>
        /// <param name="image">A native image to use</param>
        public WinFormsBitmap(Image image)
        {
            NativeImage = image;
            Origin = Vector2F.Zero;
        }
    }
}