using System;
using System.Collections.Generic;
using GameProject.Ecs;
using GameProject.GameDebug;

namespace GameProject.CoreEngine
{
    /// <summary>
    ///     Base class for all game scripts
    /// </summary>
    internal abstract class GameScript : IGameComponent
    {
        public GameEntity Entity { get; set; }

        protected GameState GameState { get; private set; }

        private bool initialized;

        private readonly List<IEnumerator<Awaiter>> coroutines = new List<IEnumerator<Awaiter>>();

        public void Update(GameState state)
        {
            GameState = state;
            if (!initialized)
            {
                Initialize();
                initialized = true;
                return;
            }
            Update();
            UpdateCoroutines(state);
        }

        private void UpdateCoroutines(GameState state)
        {
            for (var i = 0; i < coroutines.Count; i++)
            {
                var current = coroutines[i].Current;
                if (current is null)
                {
                    coroutines[i].MoveNext();
                    continue;
                }

                current.Update(state.Time.DeltaTime, state.Time.DeltaTimeUnscaled);
                if (current.Completed && !coroutines[i].MoveNext())
                    current.IsLast = true;
            }

            coroutines.RemoveAll(c => c.Current?.IsLast ?? false);
        }

        protected void StartCoroutine(Func<IEnumerable<Awaiter>> coroutine)
        {
            coroutines.Add(coroutine().GetEnumerator());
        }
        
        protected virtual void Update()
        {
        }

        protected virtual void Initialize()
        {
        }

        public virtual void Destroy(GameState state)
        {
        }
    }
}