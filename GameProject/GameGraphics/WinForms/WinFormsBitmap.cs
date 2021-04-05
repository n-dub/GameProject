using System.Drawing;
using GameProject.GameMath;

namespace GameProject.GameGraphics.WinForms
{
    internal class WinFormsBitmap : IBitmap
    {
        public Vector2F Origin { get; set; }

        public Image NativeImage { get; }

        public WinFormsBitmap(Image image)
        {
            NativeImage = image;
            Origin = new Vector2F();
        }
    }
}