using GameProject.Ecs;

namespace GameProject.CoreEngine
{
    internal abstract class GameScript : IGameComponent
    {
        public GameEntity Entity { get; set; }

        protected GameState GameState { get; private set; }
        
        public void Initialize(GameState state)
        {
            GameState = state;
            Initialize();
        }

        public void Update(GameState state)
        {
            GameState = state;
            Update();
        }

        protected abstract void Initialize();
        protected abstract void Update();
    }
}