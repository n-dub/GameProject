namespace GameProject.CoreEngine
{
    internal class Time
    {
        /// <summary>
        /// Duration of last frame in milliseconds
        /// </summary>
        public double DeltaTime { get; private set; }

        /// <summary>
        /// Current framerate
        /// </summary>
        public double Fps => 1000.0 / DeltaTime;
        
        /// <summary>
        /// Index of the current frame
        /// </summary>
        public int FrameIndex { get; private set; }

        /// <summary>
        /// Set delta time, increment frame index
        /// Must be used in <see cref="MainWindow"/> only
        /// </summary>
        /// <param name="dt">New value of <see cref="DeltaTime"/></param>
        public void UpdateForNextFrame(double dt)
        {
            DeltaTime = dt;
            ++FrameIndex;
        }
    }
}