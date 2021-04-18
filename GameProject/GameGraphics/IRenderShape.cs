using GameProject.GameMath;

namespace GameProject.GameGraphics
{
    internal interface IRenderShape
    {
        Matrix3F Transform { get; set; }
        bool IsActive { get; set; }
        int Layer { get; }
        
        void Initialize(IGraphicsDevice device);
        
        void Draw(IGraphicsDevice device, Matrix3F viewMatrix);
    }
}
