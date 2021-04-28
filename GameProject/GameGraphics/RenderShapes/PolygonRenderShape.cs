using System.Collections.Generic;
using System.Drawing;
using GameProject.GameMath;

namespace GameProject.GameGraphics.RenderShapes
{
    internal class PolygonRenderShape : IRenderShape
    {
        public int Layer { get; }
        public Matrix3F Transform { get; set; }
        public bool IsActive { get; set; }
        
        public IEnumerable<Vector2F> Points { get; set; }
        
        /// <summary>
        ///     Color to use if <see cref="Image" /> is null
        /// </summary>
        public Color Color { get; set; } = Color.Black;

        public PolygonRenderShape(int layer)
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
            device.FillPolygon(Points, Color);
        }
    }
}