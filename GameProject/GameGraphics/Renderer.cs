using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameProject.GameGraphics
{
    internal class Renderer
    {
        public bool Initialized { get; private set; }
        public IGraphicsDevice Device { get; private set; }

        private readonly HashSet<IRenderShape> renderShapes;
        
        public Renderer() => renderShapes = new HashSet<IRenderShape>();

        public Renderer(IEnumerable<IRenderShape> shapes) => renderShapes = shapes.ToHashSet();

        public void Initialize(IGraphicsDevice graphicsDevice)
        {
            Device = graphicsDevice;
            Initialized = true;
        }

        public void RenderAll()
        {
            Device.BeginRender();
            foreach (var grouping in renderShapes
                .GroupBy(x => x.Layer)
                .OrderBy(g => g.Key))
            {
                Device.PushLayer();
                foreach (var shape in grouping)
                    shape.Draw(Device);
                Device.PopLayer();
            }

            Device.EndRender();
        }

        public void AddShape(IRenderShape shape)
        {
            shape.IsActive = true;
            if (!renderShapes.Contains(shape))
                renderShapes.Add(shape);
        }

        public void RemoveShape(IRenderShape shape)
        {
            if (renderShapes.Contains(shape))
                shape.IsActive = false;
            else
                throw new ArgumentException();
        }
    }
}