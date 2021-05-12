using System.Collections.Generic;
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
        /// <param name="location">Location of a bitmap to draw</param>
        /// <param name="scale"></param>
        void DrawBitmap(IBitmap bitmap, Vector2F location, Vector2F scale, float opacity);

        /// <summary>
        ///     Draw a rectangle of certain color
        /// </summary>
        /// <param name="location">Position of a rectangle to draw</param>
        /// <param name="size">Size of a rectangle to draw</param>
        /// <param name="color">Color of a rectangle to draw</param>
        void DrawRectangle(Vector2F location, Vector2F size, Color color);

        /// <summary>
        ///     Draw a line of certain color
        /// </summary>
        /// <param name="start">Start of a segment to draw</param>
        /// <param name="end">End of a segment to draw</param>
        /// <param name="color">Color of a segment to draw</param>
        /// <param name="weight">Weight of a segment to draw</param>
        void DrawLine(Vector2F start, Vector2F end, Color color, float weight = 0.01f);

        /// <summary>
        ///     Draw an ellipse and fill it with color
        /// </summary>
        /// <param name="location">Location of an ellipse to draw</param>
        /// <param name="size">Size of an ellipse to draw</param>
        /// <param name="color">Color of an ellipse to draw</param>
        void FillEllipse(Vector2F location, Vector2F size, Color color);

        /// <summary>
        ///     Draw an ellipse
        /// </summary>
        /// <param name="location">Location of an ellipse to draw</param>
        /// <param name="size">Size of an ellipse to draw</param>
        /// <param name="color">Color of an ellipse to draw</param>
        /// <param name="weight">Width of line</param>
        void DrawEllipse(Vector2F location, Vector2F size, Color color, float weight = 0.01f);

        /// <summary>
        ///     Draw a polygon and fill it with color
        /// </summary>
        /// <param name="points">Points of a polygon to draw</param>
        /// <param name="color">Color of a polygon to draw</param>
        void FillPolygon(IEnumerable<Vector2F> points, Color color);
    }
}