using GameProject.Ecs.Graphics;
using GameProject.GameMath;

namespace GameProject.CoreEngine
{
    /// <summary>
    ///     Represents game camera
    /// </summary>
    internal sealed class Camera
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
        public float ViewWidth { get; set; } = 7f;

        /// <summary>
        ///     Indicates how mush slower the background should move compared to foreground entities
        /// </summary>
        public float BackgroundMoveFactor { get; set; } = 20f;

        /// <summary>
        ///     Copy data from another instance of <see cref="Camera" />
        /// </summary>
        /// <param name="camera"></param>
        public void CopyDataFrom(Camera camera)
        {
            this.CopyPropertiesFrom(camera);
        }

        /// <summary>
        ///     Get matrix to apply to all sprite vertices to correctly project them on the screen
        /// </summary>
        /// <param name="renderLayer">Render layer of the sprite sprite</param>
        /// <returns>View matrix</returns>
        public Matrix3F GetViewMatrix(RenderLayer renderLayer = RenderLayer.Normal)
        {
            var unscaled = renderLayer == RenderLayer.Background || renderLayer == RenderLayer.Interface;
            var center = Matrix3F.CreateTranslation(ScreenSize / 2);
            var scale = Matrix3F.CreateScale(new Vector2F(ScreenSize.X / (unscaled ? 1 : ViewWidth)));
            var rot = Matrix3F.CreateRotation(Rotation);
            var moveFactor = renderLayer == RenderLayer.Background ? BackgroundMoveFactor * ViewWidth : 1;
            var pos = Matrix3F.CreateTranslation(-Position / moveFactor);

            return renderLayer == RenderLayer.Interface
                ? center * scale
                : center * scale * rot * pos;
        }
    }
}