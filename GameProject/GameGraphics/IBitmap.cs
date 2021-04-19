using GameProject.GameMath;

namespace GameProject.GameGraphics
{
    /// <summary>
    ///     An abstract bitmap
    /// </summary>
    internal interface IBitmap
    {
        /// <summary>
        ///     Position of origin of the bitmap
        /// </summary>
        Vector2F Origin { get; set; }
    }
}