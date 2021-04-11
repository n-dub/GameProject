using GameProject.CoreEngine;
using GameProject.GameMath;
using System;
using System.IO;

namespace GameProject.GameGraphics
{
    internal class QuadRenderShape : IRenderShape
    {
        public bool IsActive { get; set; }
        public int Layer { get; }
        public Matrix3F Transform { get; set; }
        public IBitmap Image { get; set; }

        private static readonly Random rand = new Random();
        
        public QuadRenderShape(int layer)
        {
            Layer = layer;
            Transform = Matrix3F.Identity;
            //Transform *= Matrix3F.CreateTranslation(new Vector2F(rand.Next(0, 300), rand.Next(0, 300)));
            Transform *= Matrix3F.CreateTranslation(new Vector2F(0, 0));
            Transform *= Matrix3F.CreateRotation(MathF.PI / 4);
            //Transform *= Matrix3F.CreateScale(new Vector2F(2, 2));
        }

        public void Initialize(IGraphicsDevice device)
        {
            var img = ResourceManager.LoadResource(File.ReadAllBytes, "Resources/test.png");
            Image = device.CreateBitmap(img);
        }

        public void Draw(IGraphicsDevice device)
        {
            device.SetInterpolationMode(InterpolationMode.Linear);
            device.SetTransform(Transform);
            device.DrawBitmap(Image);
        }
    }
}
