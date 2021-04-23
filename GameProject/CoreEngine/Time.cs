namespace GameProject.CoreEngine
{
    /// <summary>
    ///     Class that holds everything connected to game time
    /// </summary>
    internal class Time
    {
        /// <summary>
        ///     Current framerate
        /// </summary>
        public float Fps => 1f / DeltaTime;

        /// <summary>
        ///     Duration of last frame in seconds
        /// </summary>
        public float DeltaTime { get; private set; }

        /// <summary>
        ///     Index of the current frame
        /// </summary>
        public int FrameIndex { get; private set; }

        /// <summary>
        ///     Set delta time, increment frame index
        ///     Must be used in <see cref="MainWindow" /> only
        /// </summary>
        /// <param name="dt">New value of <see cref="DeltaTime" /> in milliseconds</param>
        public void UpdateForNextFrame(float dt)
        {
            DeltaTime = dt / 1000;
            ++FrameIndex;
        }
    }
}