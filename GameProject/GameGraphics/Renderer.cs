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
        private readonly HashSet<IRenderCommand> commands;

        public Renderer() => commands = new HashSet<IRenderCommand>();

        public Renderer(IEnumerable<IRenderCommand> commands) => this.commands = commands.ToHashSet();

        public void Initialize(IGraphicsDevice graphicsDevice)
        {
            foreach (var cmd in commands)
                cmd.Initialize(graphicsDevice);
            Initialized = true;
        }

        public void RenderAll(IGraphicsDevice graphicsDevice)
        {
            graphicsDevice.BeginRender();
            foreach (var cmd in commands
                .OrderBy(x => x.Layer)
                .Where(x => x.IsActive))
                cmd.Execute(graphicsDevice);
            graphicsDevice.EndRender();
        }

        public void AddCommand(IRenderCommand command)
        {
            command.IsActive = true;
            if (!commands.Contains(command))
                commands.Add(command);
        }

        public void RemoveCommand(IRenderCommand command)
        {
            if (commands.Contains(command))
                command.IsActive = false;
            else
                throw new ArgumentException();
        }
    }
}