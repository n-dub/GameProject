using GameProject.Ecs;

namespace GameProject.CoreEngine
{
    /// <summary>
    ///     Base class for all game scripts
    /// </summary>
    internal abstract class GameScript : IGameComponent
    {
        public GameEntity Entity { get; set; }

        public virtual void Update(GameState state)
        {
        }

        public virtual void Destroy(GameState state)
        {
        }
    }
}