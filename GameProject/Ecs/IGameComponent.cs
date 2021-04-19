using GameProject.CoreEngine;

namespace GameProject.Ecs
{
    /// <summary>
    ///     Interface for all game components
    /// </summary>
    internal interface IGameComponent
    {
        /// <summary>
        ///     The entity the component is attached to
        /// </summary>
        GameEntity Entity { get; set; }

        /// <summary>
        ///     Initialization method, called recursively for all components
        ///     once when they're created
        /// </summary>
        /// <param name="state">Current game state</param>
        void Initialize(GameState state);

        /// <summary>
        ///     Update method, called recursively for all components once per frame
        /// </summary>
        /// <param name="state">Current game state</param>
        void Update(GameState state);
    }
}