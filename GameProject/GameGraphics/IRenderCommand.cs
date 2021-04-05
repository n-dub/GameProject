using System.Drawing;

namespace GameProject.GameGraphics
{
    internal interface IRenderCommand
    {
        bool IsActive { get; set; }

        int Layer { get; }

        void Initialize(IGraphicsDevice device);
        
        void Execute(IGraphicsDevice device);
    }
}
