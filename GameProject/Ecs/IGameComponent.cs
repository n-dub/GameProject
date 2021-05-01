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
        ///     Update method, called recursively for all components once per frame
        /// </summary>
        /// <param name="state">Current game state</param>
        void Update(GameState state);

        /// <summary>
        ///     Destroy method, called if the entity this component is connected to
        ///     has been destroyed last frame
        /// </summary>
        void Destroy(GameState state);
    }
}