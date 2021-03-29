using System.Drawing;

namespace GameProject.GameGraphics
{
    public interface IRenderCommand
    {
        bool IsActive { get; set; }

        int Layer { get; set; }

        void Execute(Graphics graphics);
    }
}
