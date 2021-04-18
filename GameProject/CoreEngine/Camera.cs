using GameProject.GameMath;

namespace GameProject.CoreEngine
{
    /// <summary>
    /// Represents game camera
    /// </summary>
    internal class Camera
    {
        /// <summary>
        /// Position of camera's center point
        /// </summary>
        public Vector2F Position { get; set; } = new Vector2F(0, 0);
        
        /// <summary>
        /// Camera's rotation
        /// </summary>
        public float Rotation { get; set; }

        /// <summary>
        /// Width and Height of screen resolution
        /// </summary>
        public Vector2F ScreenSize { get; set; } = new Vector2F(800, 600);

        /// <summary>
        /// How wide is camera's field of view in meters
        /// </summary>
        public float ViewWidth { get; set; } = 80f;

        /// <summary>
        /// Get matrix to apply to all sprite vertices to correctly project them on the screen
        /// </summary>
        /// <returns>View matrix</returns>
        public Matrix3F GetViewMatrix()
        {
            var position = Position - ScreenSize / 2;
            var rot = Matrix3F.CreateRotation(Rotation);
            var pos = Matrix3F.CreateTranslation(-position);
            var scale = Matrix3F.CreateScale(new Vector2F(ScreenSize.X / ViewWidth));

            return pos * rot * scale;
        }
    }
}