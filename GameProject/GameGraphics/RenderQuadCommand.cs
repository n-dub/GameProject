using GameProject.CoreEngine;
using GameProject.GameMath;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameProject.GameGraphics
{
    internal class RenderQuadCommand : IRenderCommand
    {
        public bool IsActive { get; set; }
        public int Layer { get; }
        public Matrix3F Transform { get; set; }
        public IBitmap Image { get; set; }

        public RenderQuadCommand(int layer)
        {
            Layer = layer;
            Transform = Matrix3F.Identity;
            Transform *= Matrix3F.CreateScale(new Vector2F(2, 2));
            Transform *= Matrix3F.CreateTranslation(new Vector2F(100, 50));
            Transform *= Matrix3F.CreateRotation(MathF.PI / 10);
        }

        public void Initialize(IGraphicsDevice device)
        {
            var img = ResourceManager.LoadResource(File.ReadAllBytes, "test.png");
            Image = device.CreateBitmap(img);
        }

        public void Execute(IGraphicsDevice device)
        {
            device.SetTransform(Transform);
            device.DrawBitmap(Image);
        }
    }
}
