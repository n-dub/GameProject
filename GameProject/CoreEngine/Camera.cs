using GameProject.GameMath;

namespace GameProject.CoreEngine
{
    /// <summary>
    ///     Represents game camera
    /// </summary>
    internal class Camera
    {
        /// <summary>
        ///     Aspect ratio of screen's resolution
        /// </summary>
        public float AspectRatio => ScreenSize.X / ScreenSize.Y;

        /// <summary>
        ///     Position of camera's center point
        /// </summary>
        public Vector2F Position { get; set; }

        /// <summary>
        ///     Camera's rotation
        /// </summary>
        public float Rotation { get; set; }

        /// <summary>
        ///     Width and Height of screen resolution
        /// </summary>
        public Vector2F ScreenSize { get; set; }

        /// <summary>
        ///     How wide is camera's field of view in meters
        /// </summary>
        public float ViewWidth { get; set; } = 20f;

        /// <summary>
        ///     Get matrix to apply to all sprite vertices to correctly project them on the screen
        /// </summary>
        /// <returns>View matrix</returns>
        public Matrix3F GetViewMatrix()
        {
            var center = Matrix3F.CreateTranslation(ScreenSize / 2);
            var scale = Matrix3F.CreateScale(new Vector2F(ScreenSize.X / ViewWidth));
            var rot = Matrix3F.CreateRotation(Rotation);
            var pos = Matrix3F.CreateTranslation(-Position);

            return center * scale * rot * pos;
        }
    }
}