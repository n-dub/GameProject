using System.Runtime.CompilerServices;

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
        public float Fps => 1f / DeltaTimeUnscaled;

        /// <summary>
        ///     Duration of last frame in seconds effected by <see cref="TimeScale" />
        /// </summary>
        public float DeltaTime => DeltaTimeUnscaled * TimeScale;

        /// <summary>
        ///     Duration of last frame in seconds
        /// </summary>
        public float DeltaTimeUnscaled { get; private set; }

        /// <summary>
        ///     Scale of time, may be used for slow-motion effects
        /// </summary>
        public float TimeScale { get; set; } = 1f;

        /// <summary>
        ///     Index of the current frame
        /// </summary>
        public int FrameIndex { get; private set; }

        /// <summary>
        ///     Set delta time, increment frame index
        ///     Must be used in <see cref="MainWindow" /> only
        /// </summary>
        /// <param name="dt">New value of <see cref="DeltaTime" /> in milliseconds</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void UpdateForNextFrame(float dt)
        {
            DeltaTimeUnscaled = dt / 1000;
            ++FrameIndex;
        }
    }
}