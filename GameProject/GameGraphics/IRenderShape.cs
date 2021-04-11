namespace GameProject.GameGraphics
{
    internal interface IRenderShape
    {
        bool IsActive { get; set; }

        int Layer { get; }

        void Initialize(IGraphicsDevice device);
        
        void Draw(IGraphicsDevice device);
    }
}
