namespace GameProject.CoreEngine
{
    internal class Time
    {
        /// <summary>
        /// Duration of last frame in milliseconds
        /// </summary>
        public float DeltaTime { get; private set; }

        /// <summary>
        /// Current framerate
        /// </summary>
        public float Fps => 1000.0f / DeltaTime;
        
        /// <summary>
        /// Index of the current frame
        /// </summary>
        public int FrameIndex { get; private set; }

        /// <summary>
        /// Set delta time, increment frame index
        /// Must be used in <see cref="MainWindow"/> only
        /// </summary>
        /// <param name="dt">New value of <see cref="DeltaTime"/></param>
        public void UpdateForNextFrame(float dt)
        {
            DeltaTime = dt;
            ++FrameIndex;
        }
    }
}