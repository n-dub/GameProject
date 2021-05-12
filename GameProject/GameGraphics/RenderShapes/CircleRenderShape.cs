using System.Drawing;
using GameProject.CoreEngine;
using GameProject.Ecs.Graphics;
using GameProject.GameMath;

namespace GameProject.GameGraphics.RenderShapes
{
    internal class CircleRenderShape : IRenderShape
    {
        public int Layer { get; }
        
        public int Id { get; }

        public RenderLayer RenderLayer { get; set; }

        public Matrix3F Transform { get; set; }
        
        public Vector2F Offset { get; set; }

        /// <summary>
        ///     Color to use if <see cref="Image" /> is null
        /// </summary>
        public Color Color { get; set; } = Color.Black;

        public bool IsActive { get; set; } = true;

        /// <summary>
        ///     Create a new render shape with certain layer
        /// </summary>
        /// <param name="layer">Layer index of the shape</param>
        public CircleRenderShape(int layer)
        {
            Layer = layer;
            Id = RenderShapeIdGenerator.GetId();
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

        public void CopyDataFrom(IRenderShape other)
        {
            if (other is CircleRenderShape s)
                this.CopyPropertiesFrom(s);
        }
        
        public override int GetHashCode()
        {
            return Id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((IRenderShape) obj);
        }
        
        private bool Equals(IRenderShape other)
        {
            return Id == other.Id;
        }
    }
}