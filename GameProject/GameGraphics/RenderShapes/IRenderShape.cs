using GameProject.GameMath;

namespace GameProject.GameGraphics.RenderShapes
{
    /// <summary>
    ///     Graphical representation of an entity
    /// </summary>
    internal interface IRenderShape
    {
        /// <summary>
        ///     An index of layer
        /// </summary>
        int Layer { get; }
        
        /// <summary>
        ///     Unique ID of the shape
        /// </summary>
        int Id { get; }

        /// <summary>
        ///     True if sprite is background
        /// </summary>
        bool IsBackground { get; set; }
        
        /// <summary>
        ///     Global transformation matrix of entity being drawn
        /// </summary>
        Matrix3F Transform { get; set; }
        
        /// <summary>
        ///     Global offset of the shape
        /// </summary>
        Vector2F Offset { get; set; }

        /// <summary>
        ///     The shape will only be drawn if it's active
        /// </summary>
        bool IsActive { get; set; }

        /// <summary>
        ///     Initialize this shape
        /// </summary>
        /// <param name="device">An instance of graphics device to use</param>
        void Initialize(IGraphicsDevice device);

        /// <summary>
        ///     Draw this shape
        /// </summary>
        /// <param name="device">An instance of graphics device to use</param>
        /// <param name="viewMatrix">Camera's view matrix</param>
        void Draw(IGraphicsDevice device, Matrix3F viewMatrix);

        /// <summary>
        ///     Make a full deep copy of a shape
        /// </summary>
        /// <param name="other"></param>
        /// <returns>The copied shape</returns>
        void CopyDataFrom(IRenderShape other);
    }
}