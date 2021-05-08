﻿using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using GameProject.GameMath;

namespace GameProject.GameGraphics.RenderShapes
{
    internal class PolygonRenderShape : IRenderShape
    {
        public int Layer { get; }
        
        public int Id { get; }

        public Matrix3F Transform { get; set; }
        
        public Vector2F Offset { get; set; }

        public bool IsActive { get; set; } = true;
        
        public Vector2F[] Points { get; set; }
        
        /// <summary>
        ///     Color to use if <see cref="Image" /> is null
        /// </summary>
        public Color Color { get; set; } = Color.Black;

        public PolygonRenderShape(int layer)
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
            device.FillPolygon(Points.Select(x => x + Offset), Color);
        }

        public void CopyDataFrom(IRenderShape other)
        {
            if (!(other is PolygonRenderShape s))
                return;
            Color = s.Color;
            IsActive = s.IsActive;
            Offset = s.Offset;
            Points = s.Points.ToArray();
            Transform = s.Transform;
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