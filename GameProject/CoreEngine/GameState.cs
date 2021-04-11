using GameProject.GameGraphics;
using GameProject.GameInput;

namespace GameProject.CoreEngine
{
    /// <summary>
    /// Represents general game state, immutable
    /// </summary>
    internal class GameState
    {
        /// <summary>
        /// Currently used instance of <see cref="Renderer"/>
        /// </summary>
        public Renderer Renderer { get; }

        /// <summary>
        /// An instance of <see cref="Keyboard"/> being updated by main form
        /// </summary>
        public Keyboard Keyboard { get; }

        /// <summary>
        /// An instance of <see cref="Time"/> being updated by main form
        /// </summary>
        public Time Time { get; }

        /// <summary>
        /// Create a copy of another instance of <see cref="GameState"/>
        /// </summary>
        /// <param name="other">Instance to copy</param>
        public GameState(GameState other)
        {
            Renderer = other.Renderer;
            Keyboard = other.Keyboard;
            Time = other.Time;
        }

        /// <summary>
        /// Create a new instance of <see cref="GameState"/>
        /// </summary>
        public GameState(Renderer renderer, Keyboard keyboard, Time time)
        {
            Renderer = renderer;
            Keyboard = keyboard;
            Time = time;
        }
    }
}