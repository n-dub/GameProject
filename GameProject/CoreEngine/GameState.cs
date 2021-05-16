using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
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
    internal sealed class GameState
    {
        /// <summary>
        ///     <see cref="Renderer" /> currently used for reading
        /// </summary>
        public Renderer RendererRead { get; }

        /// <summary>
        ///     <see cref="Renderer" /> currently used for writing
        /// </summary>
        public Renderer RendererWrite { get; }

        /// <summary>
        ///     An instance of Keyboard being updated by main form
        /// </summary>
        public Keyboard<Keys> Keyboard { get; }

        /// <summary>
        ///     An instance of <see cref="Mouse" /> being updated by main form
        /// </summary>
        public Mouse Mouse { get; }

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
        public GameState(Renderer renderer, World physicsWorld, List<GameEntity> entities)
        {
            RendererRead = new Renderer();
            RendererWrite = new Renderer();
            RendererRead.CopyDataFrom(renderer);
            RendererWrite.CopyDataFrom(renderer);

            Keyboard = new Keyboard<Keys>();
            Mouse = new Mouse();
            Time = new Time();
            PhysicsWorld = physicsWorld;
            newEntities = entities;
        }

        public void SwapRenderers()
        {
            RendererWrite.Camera.ScreenSize = RendererRead.Camera.ScreenSize;
            RendererRead.CopyDataFrom(RendererWrite);
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