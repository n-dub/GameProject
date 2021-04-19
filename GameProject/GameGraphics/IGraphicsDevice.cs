using System.Drawing;
using GameProject.GameMath;

namespace GameProject.GameGraphics
{
    /// <summary>
    ///     An abstract graphics device
    /// </summary>
    internal interface IGraphicsDevice
    {
        /// <summary>
        ///     Load an instance of <see cref="IBitmap" />
        /// </summary>
        /// <param name="imageData">Bytes that represent image to create a bitmap from</param>
        /// <returns>The loaded bitmap</returns>
        IBitmap CreateBitmap(byte[] imageData);

        /// <summary>
        ///     Set transformation matrix in the device. Will be applied to all drawings
        /// </summary>
        /// <param name="transformMatrix">Matrix to set</param>
        void SetTransform(Matrix3F transformMatrix);

        /// <summary>
        ///     Begin rendering session, start recording command buffer is needed
        /// </summary>
        void BeginRender();

        /// <summary>
        ///     End rendering session, flush commands
        /// </summary>
        void EndRender();

        /// <summary>
        ///     Set interpolation mode used by the device
        /// </summary>
        /// <param name="mode">An <see cref="InterpolationMode" /> to use</param>
        void SetInterpolationMode(InterpolationMode mode);

        /// <summary>
        ///     Draw a bitmap using internal transformation matrix
        /// </summary>
        /// <param name="bitmap">An instance of <see cref="IBitmap" /> to draw</param>
        void DrawBitmap(IBitmap bitmap);

        /// <summary>
        ///     Draw rectangle of certain color
        /// </summary>
        /// <param name="location">Position of a rectangle to draw</param>
        /// <param name="size">Size of a rectangle to draw</param>
        /// <param name="color">Color of a rectangle to draw</param>
        void DrawRectangle(Vector2F location, Vector2F size, Color color);

        /// <summary>
        ///     Push a new layer<br />
        ///     <seealso cref="PopLayer" />
        /// </summary>
        void PushLayer();

        /// <summary>
        ///     Pop previously pushed layer<br />
        ///     <seealso cref="PushLayer" />
        /// </summary>
        void PopLayer();
    }
}