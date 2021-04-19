using System.Drawing;
using System.IO;
using GameProject.CoreEngine;
using GameProject.GameMath;

namespace GameProject.GameGraphics
{
    /// <summary>
    ///     An implementation of <see cref="IRenderShape" /> that draws a quad
    /// </summary>
    internal class QuadRenderShape : IRenderShape
    {
        public int Layer { get; }

        /// <summary>
        ///     Image to use for drawing
        /// </summary>
        public IBitmap Image { get; set; }

        public bool IsActive { get; set; }

        public Matrix3F Transform { get; set; }

        // TODO: TEST CODE - TO BE REMOVED
        private static int colorId;

        // TODO: TEST CODE - TO BE REMOVED
        private Color? color;

        /// <summary>
        ///     Create a new render shape with certain layer
        /// </summary>
        /// <param name="layer"></param>
        public QuadRenderShape(int layer)
        {
            Layer = layer;
        }

        public void Initialize(IGraphicsDevice device)
        {
            var img = ResourceManager.LoadResource(File.ReadAllBytes, "Resources/test.png");
            //Image = device.CreateBitmap(img);
        }

        public void Draw(IGraphicsDevice device, Matrix3F viewMatrix)
        {
            device.SetInterpolationMode(InterpolationMode.Linear);
            device.SetTransform(viewMatrix * Transform);
            {
                // TODO: TEST CODE - TO BE REMOVED
                var c = new[] {Color.Aqua, Color.Brown, Color.Black, Color.Green};
                if (color is null)
                    color = c[colorId++];
            }
            if (Image is null)
                device.DrawRectangle(new Vector2F(), new Vector2F(1, 1), color.Value);
            else
                device.DrawBitmap(Image);
        }
    }
}