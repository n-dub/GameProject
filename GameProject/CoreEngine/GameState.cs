﻿using FarseerPhysics.Dynamics;
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

        /// <summary>
        ///     Create a copy of another instance of <see cref="GameState" />
        /// </summary>
        /// <param name="other">Instance to copy</param>
        public GameState(GameState other)
        {
            Renderer = other.Renderer;
            Keyboard = other.Keyboard;
            Time = other.Time;
            PhysicsWorld = other.PhysicsWorld;
        }

        /// <summary>
        ///     Create a new instance of <see cref="GameState" />
        /// </summary>
        public GameState(Renderer renderer, Keyboard keyboard, Time time, World physicsWorld)
        {
            Renderer = renderer;
            Keyboard = keyboard;
            Time = time;
            PhysicsWorld = physicsWorld;
        }
    }
}