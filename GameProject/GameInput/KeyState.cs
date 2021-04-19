namespace GameProject.GameInput
{
    /// <summary>
    ///     State of keyboard key
    /// </summary>
    public enum KeyState
    {
        /// <summary>
        ///     Key have been pushed down at this frame
        /// </summary>
        Down,

        /// <summary>
        ///     Key have been released at this frame
        /// </summary>
        Up,

        /// <summary>
        ///     Key is being pushed during more than one frame
        /// </summary>
        Pushing,

        /// <summary>
        ///     Key hasn't been touched for at least two frames (the current and the last one)
        /// </summary>
        None
    }
}