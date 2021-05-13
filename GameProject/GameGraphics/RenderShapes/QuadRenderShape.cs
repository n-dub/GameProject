using System;
using System.Drawing;
using System.IO;
using System.Linq;
using GameProject.CoreEngine;
using GameProject.Ecs.Graphics;
using GameProject.GameMath;

namespace GameProject.GameGraphics.RenderShapes
{
    /// <summary>
    ///     An implementation of <see cref="IRenderShape" /> that draws a quad
    /// </summary>
    internal class QuadRenderShape : IRenderShape
    {
        public int Layer { get; }
        
        public int Id { get; }

        public RenderLayer RenderLayer { get; set; }

        /// <summary>
        ///     Image to use for drawing
        /// </summary>
        public IBitmap Image { get; set; }

        /// <summary>
        ///     Color to use if <see cref="Image" /> is null
        /// </summary>
        public Color Color { get; set; } = Color.Black;

        public Vector2F Offset { get; set; }
        
        public Vector2F Scale { get; set; } = Vector2F.One;

        public bool IsActive { get; set; } = true;

        public Matrix3F Transform { get; set; }
        
        public string ImagePath { get; set; }

        /// <summary>
        ///     Create a new render shape with certain layer
        /// </summary>
        /// <param name="layer">Layer index of the shape</param>
        public QuadRenderShape(int layer)
        {
            Layer = layer;
            Id = RenderShapeIdGenerator.GetId();
        }

        public void Initialize(IGraphicsDevice device)
        {
            if (ImagePath is null || Image != null)
                return;
            
            var img = ResourceManager.LoadResource(File.ReadAllBytes, ImagePath);
            Image = device.CreateBitmap(img.Resource);
        }

        public void Draw(IGraphicsDevice device, Matrix3F viewMatrix)
        {
            device.SetInterpolationMode(InterpolationMode.Linear);
            device.SetTransform(viewMatrix * Transform);
            if (Image is null)
                device.DrawRectangle(Vector2F.Zero, Vector2F.One, Color);
            else
                device.DrawBitmap(Image, Offset, Scale, Color.A / 255f);
        }

        public void CopyDataFrom(IRenderShape other)
        {
            if (other is QuadRenderShape s)
                this.CopyPropertiesFrom(s);
        }
        
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((IRenderShape) obj);
        }

        public override int GetHashCode()
        {
            return Id;
        }

        private bool Equals(IRenderShape other)
        {
            return Id == other.Id;
        }
    }
}