namespace GameProject.GameDebug
{
    /// <summary>
    ///     Interface for debuggable objects
    /// </summary>
    internal interface IDebuggable
    {
        /// <summary>
        ///     Draw debug overlay, e.i. any drawings which can be helpful for debugging
        ///     of a certain object
        /// </summary>
        /// <param name="debugDraw">An instance of <see cref="DebugDraw" /> class that'll be used for drawing</param>
        void DrawDebugOverlay(DebugDraw debugDraw);
    }
}