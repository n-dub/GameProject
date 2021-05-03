using System.Drawing;
using GameProject.GameMath;

namespace GameProject.GameGraphics.RenderShapes
{
    internal class CircleRenderShape : IRenderShape
    {
        public int Layer { get; }
        
        public Matrix3F Transform { get; set; }
        
        public Vector2F Offset { get; set; }

        /// <summary>
        ///     Color to use if <see cref="Image" /> is null
        /// </summary>
        public Color Color { get; set; } = Color.Black;

        public bool IsActive { get; set; }

        /// <summary>
        ///     Create a new render shape with certain layer
        /// </summary>
        /// <param name="layer">Layer index of the shape</param>
        public CircleRenderShape(int layer)
        {
            Layer = layer;
        }

        public void Initialize(IGraphicsDevice device)
        {
        }

        public void Draw(IGraphicsDevice device, Matrix3F viewMatrix)
        {
            device.SetInterpolationMode(InterpolationMode.Linear);
            device.SetTransform(viewMatrix * Transform);
            device.FillEllipse(Offset - new Vector2F(0.5f), Vector2F.One, Color);
        }
    }
}