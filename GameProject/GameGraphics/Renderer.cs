using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameProject.GameGraphics
{
    public class Renderer
    {
        private readonly HashSet<IRenderCommand> commands = new HashSet<IRenderCommand>();

        public void RenderAll(Graphics graphics)
        {
            foreach (var cmd in commands.OrderBy(x => x.Layer))
                cmd.Execute(graphics);
        }

        public void AddCommand(IRenderCommand command)
        {
            if (commands.Contains(command))
                command.IsActive = true;
            else
                commands.Add(command);
        }

        public void RemoveCommand(IRenderCommand command)
        {
            if (commands.Contains(command))
                command.IsActive = false;
        }
    }
}
