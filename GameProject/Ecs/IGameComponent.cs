using GameProject.CoreEngine;

namespace GameProject.Ecs
{
    internal interface IGameComponent
    {
        GameEntity Entity { get; set; }

        void Initialize(GameState state);
        void Update( GameState state);
    }
}