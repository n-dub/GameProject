namespace GameProject.CoreEngine
{
    /// <summary>
    ///     Used to suspend execution of a coroutine and wait certain time or number of frames to continue
    /// </summary>
    internal sealed class Awaiter
    {
        /// <summary>
        ///     True if waited required time
        /// </summary>
        public bool Completed => Seconds <= 0 && Frames <= 0;

        /// <summary>
        ///     True if time should be scaled with <see cref="Time.TimeScale" />
        /// </summary>
        public bool TimeScaled { get; }

        /// <summary>
        ///     Seconds left to wait
        /// </summary>
        public float Seconds { get; private set; }

        /// <summary>
        ///     Frames left to wait
        /// </summary>
        public int Frames { get; private set; }

        /// <summary>
        ///     True if this awaiter was last in the coroutine, so the coroutine itself can be safely removed
        /// </summary>
        public bool IsLast { get; set; }

        private Awaiter(int frames)
        {
            Frames = frames;
        }

        private Awaiter(float seconds, bool timeScaled)
        {
            Seconds = seconds;
            TimeScaled = timeScaled;
        }

        /// <summary>
        ///     Wait a single frame
        /// </summary>
        /// <returns>The created instance of awaiter</returns>
        public static Awaiter WaitForNextFrame()
        {
            return new Awaiter(1);
        }

        /// <summary>
        ///     Wait for certain number of frames to continue
        /// </summary>
        /// <param name="count">Number of frames to wait</param>
        /// <returns>The created instance of awaiter</returns>
        public static Awaiter WaitForFrames(int count)
        {
            return new Awaiter(count);
        }

        /// <summary>
        ///     Wait a certain time (in seconds) to continue
        /// </summary>
        /// <param name="seconds">Number of seconds to wait</param>
        /// <param name="timeScaled">True if time should be scaled with <see cref="Time.TimeScale" /></param>
        /// <returns>The created instance of awaiter</returns>
        public static Awaiter WaitForSeconds(float seconds, bool timeScaled = true)
        {
            return new Awaiter(seconds, timeScaled);
        }

        /// <summary>
        ///     Update this awaiter for the next frame
        /// </summary>
        /// <param name="deltaTime">Scaled delta time in seconds</param>
        /// <param name="deltaTimeUnscaled">Unscaled delta time in seconds</param>
        public void Update(float deltaTime, float deltaTimeUnscaled)
        {
            Seconds -= TimeScaled ? deltaTime : deltaTimeUnscaled;
            --Frames;
        }
    }
}