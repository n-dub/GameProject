using System.Collections.Generic;
using System.Runtime.CompilerServices;
using FarseerPhysics.Dynamics;
using GameProject.Ecs;
using GameProject.GameGraphics;
using GameProject.GameInput;

namespace GameProject.CoreEngine
{
    /// <summary>
    ///     Represents general game state, is immutable, therefore must be not changed by
    ///     any game classes but the <see cref="MainWindow" />
    /// </summary>
    internal class GameState
    {
        /// <summary>
        ///     Currently used instance of <see cref="Renderer" />
        /// </summary>
        public Renderer Renderer { get; }

        /// <summary>
        ///     An instance of <see cref="Keyboard" /> being updated by main form
        /// </summary>
        public Keyboard Keyboard { get; }

        /// <summary>
        ///     An instance of <see cref="Time" /> being updated by main form
        /// </summary>
        public Time Time { get; }

        /// <summary>
        ///     An instance of <see cref="World" /> being used for physics simulation in the game.
        /// </summary>
        public World PhysicsWorld { get; }

        public IEnumerable<GameEntity> Entities => entities;

        private readonly List<GameEntity> entities = new List<GameEntity>();
        private readonly List<GameEntity> newEntities;
        
        /// <summary>
        ///     Create a new instance of <see cref="GameState" />
        /// </summary>
        public GameState(Renderer renderer, Keyboard keyboard, Time time, World physicsWorld, List<GameEntity> entities)
        {
            Renderer = renderer;
            Keyboard = keyboard;
            Time = time;
            PhysicsWorld = physicsWorld;
            newEntities = entities;
        }

        /// <summary>
        ///     Add a new game entity
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddEntity(GameEntity entity)
        {
            newEntities.Add(entity);
        }

        /// <summary>
        ///     Execute deferred entity adding commands
        /// </summary>
        public void AddNewEntities()
        {
            entities.AddRange(newEntities);
            newEntities.Clear();
        }

        /// <summary>
        ///     Remove all destroyed entities
        /// </summary>
        public void RemoveDestroyed()
        {
            entities.RemoveAll(e => e.Destroyed);
        }
    }
}