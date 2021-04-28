using System;
using System.Collections.Generic;
using System.Linq;
using GameProject.CoreEngine;
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

        private readonly HashSet<IRenderShape> renderShapes;

        /// <summary>
        ///     Create a renderer
        /// </summary>
        public Renderer()
        {
            renderShapes = new HashSet<IRenderShape>();
        }

        /// <summary>
        ///     Create a renderer that holds certain shaped
        /// </summary>
        /// <param name="shapes">A collection of shapes</param>
        public Renderer(IEnumerable<IRenderShape> shapes)
        {
            renderShapes = shapes.ToHashSet();
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

            foreach (var layerGrouping in renderShapes
                .GroupBy(x => x.Layer)
                .OrderBy(g => g.Key))
            foreach (var shape in layerGrouping.Where(s => s.IsActive))
                shape.Draw(Device, viewMatrix);
        }

        /// <summary>
        ///     Add a new shape to render queue
        /// </summary>
        /// <param name="shape">A shape to add</param>
        public void AddShape(IRenderShape shape)
        {
            shape.IsActive = true;
            if (!renderShapes.Contains(shape))
                renderShapes.Add(shape);
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
    }
}