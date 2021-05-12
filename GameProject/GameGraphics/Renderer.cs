using System;
using System.Collections.Generic;
using System.Linq;
using GameProject.CoreEngine;
using GameProject.Ecs.Graphics;
using GameProject.GameGraphics.RenderShapes;

namespace GameProject.GameGraphics
{
    /// <summary>
    ///     The view of game according to the MVC pattern
    /// </summary>
    internal class Renderer
    {
        /// <summary>
        ///     An active instance of <see cref="CoreEngine.Camera" />
        /// </summary>
        public Camera Camera { get; } = new Camera();

        /// <summary>
        ///     If false a call to initialize is needed
        /// </summary>
        public bool Initialized { get; private set; }

        /// <summary>
        ///     Currently used instance of <see cref="IGraphicsDevice" />
        /// </summary>
        public IGraphicsDevice Device { get; private set; }

        private readonly HashSet<IRenderShape> renderShapes = new HashSet<IRenderShape>();
        private readonly List<IRenderShape> shapesToAdd = new List<IRenderShape>();

        public void CopyDataFrom(Renderer renderer)
        {
            Camera.CopyDataFrom(renderer.Camera);
            renderer.AddInitialize(Device);
            
            foreach (var renderShape in renderer.renderShapes)
            {
                if (renderShapes.TryGetValue(renderShape, out var shape))
                    shape.CopyDataFrom(renderShape);
                else
                    renderShapes.Add(renderShape);
            }
        }

        /// <summary>
        ///     Initialize the renderer
        /// </summary>
        /// <param name="graphicsDevice">Graphics device to use</param>
        public void Initialize(IGraphicsDevice graphicsDevice)
        {
            Device = graphicsDevice;
            Initialized = true;
        }

        /// <summary>
        ///     Render all containing shapes
        /// </summary>
        public void RenderAll()
        {
            var viewMatrix = Camera.GetViewMatrix();
            var bgViewMatrix = Camera.GetViewMatrix(RenderLayer.Background);
            var uiViewMatrix = Camera.GetViewMatrix(RenderLayer.Interface);

            foreach (var layerGrouping in renderShapes
                .GroupBy(x => x.Layer)
                .OrderBy(g => g.Key))
            foreach (var shape in layerGrouping.Where(s => s.IsActive))
            {
                switch (shape.RenderLayer)
                {
                    case RenderLayer.Background:
                        shape.Draw(Device, bgViewMatrix);
                        break;
                    case RenderLayer.Interface:
                        shape.Draw(Device, uiViewMatrix);
                        break;
                    case RenderLayer.Normal:
                        shape.Draw(Device, viewMatrix);
                        break;
                    default:
                        throw new Exception();
                }
            }
        }

        /// <summary>
        ///     Add a new shape to render queue
        /// </summary>
        /// <param name="shape">A shape to add</param>
        public void AddShape(IRenderShape shape)
        {
            shapesToAdd.Add(shape);
        }

        /// <summary>
        ///     Remove a certain shape from render queue
        /// </summary>
        /// <param name="shape">A shape to remove</param>
        /// <exception cref="ArgumentException">Thrown if shape doesn't exist</exception>
        public void RemoveShape(IRenderShape shape)
        {
            if (renderShapes.Contains(shape))
                shape.IsActive = false;
            else
                throw new ArgumentException();
        }

        private void AddInitialize(IGraphicsDevice device)
        {
            foreach (var shape in shapesToAdd)
            {
                shape.IsActive = true;
                shape.Initialize(device);
                if (!renderShapes.Contains(shape))
                    renderShapes.Add(shape);
            }
            shapesToAdd.Clear();
        }
    }
}