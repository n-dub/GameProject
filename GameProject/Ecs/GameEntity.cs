﻿using System;
using System.Collections.Generic;
using System.Linq;
using GameProject.CoreEngine;
using GameProject.GameMath;

namespace GameProject.Ecs
{
    /// <summary>
    ///     Represents a game entity (game object)
    /// </summary>
    internal class GameEntity
    {
        /// <summary>
        ///     A collection of all components attached to this entity
        /// </summary>
        public IEnumerable<IGameComponent> Components => components.Select(x => x.Value);

        /// <summary>
        ///     A list of all children of this entity
        /// </summary>
        public IReadOnlyList<GameEntity> Children => children;

        /// <summary>
        ///     Global transformation matrix, is recalculated on-demand
        /// </summary>
        public Matrix3F GlobalTransform => Parent is null
            ? LocalTransform
            : Parent.GlobalTransform * LocalTransform;

        /// <summary>
        ///     Parent of this game entity
        /// </summary>
        public GameEntity Parent { get; private set; }

        /// <summary>
        ///     Local transformation of this entity, will be recalculated
        ///     if transformation parameters have been changed after the last time
        /// </summary>
        public Matrix3F LocalTransform
        {
            get => localTransform
                   ?? Matrix3F.CreateTranslation(Position)
                   * Matrix3F.CreateRotation(Rotation)
                   * Matrix3F.CreateScale(Scale);
            private set => localTransform = value;
        }

        /// <summary>
        ///     Position of this entity (in meters)
        /// </summary>
        public Vector2F Position
        {
            get => position;
            set
            {
                position = value;
                LocalTransform = null;
            }
        }

        /// <summary>
        ///     Rotation of this entity (in radians)
        /// </summary>
        public float Rotation
        {
            get => rotation;
            set
            {
                rotation = value;
                LocalTransform = null;
            }
        }

        /// <summary>
        ///     Scaling of this entity
        /// </summary>
        public Vector2F Scale
        {
            get => scale;
            set
            {
                scale = value;
                LocalTransform = null;
            }
        }

        private readonly List<GameEntity> children;

        private readonly Dictionary<Type, IGameComponent> components;

        private Matrix3F localTransform;

        private Vector2F position;
        private float rotation;
        private Vector2F scale = Vector2F.One;

        /// <summary>
        ///     Create a new game entity
        /// </summary>
        public GameEntity()
        {
            components = new Dictionary<Type, IGameComponent>();
            children = new List<GameEntity>();
        }

        /// <summary>
        ///     Create an entity and attach to parent
        /// </summary>
        /// <param name="parent">Parent of the entity being created</param>
        public GameEntity(GameEntity parent) : this()
        {
            ConnectEntities(this, parent);
        }

        /// <summary>
        ///     Connect two entities - a child and a parent
        /// </summary>
        /// <param name="child">A child entity</param>
        /// <param name="parent">A parent entity</param>
        /// <exception cref="ArgumentNullException">Thrown if child is null</exception>
        private static void ConnectEntities(GameEntity child, GameEntity parent)
        {
            if (child is null)
                throw new ArgumentNullException(nameof(child));
            child.Parent?.children.Remove(child);
            child.Parent = parent;
            parent?.children.Add(child);
        }

        /// <summary>
        ///     Internal helper function for adding/removing components.
        ///     Null will delete existing components of a certain type.
        ///     Non-null value will overwrite existing component.
        /// </summary>
        /// <param name="component">Component to set</param>
        /// <typeparam name="T">Type of the component</typeparam>
        private void SetComponent<T>(T component) where T : class, IGameComponent
        {
            if (components.TryGetValue(typeof(T), out var previous))
            {
                previous.Entity = null;
                components.Remove(typeof(T));
            }

            if (component is null)
                return;

            component.Entity?.components.Remove(typeof(T));
            component.Entity = this;
            components[typeof(T)] = component;
        }

        /// <summary>
        ///     Add child to this entity
        /// </summary>
        /// <param name="child">A child entity</param>
        public void AddChild(GameEntity child)
        {
            ConnectEntities(child, this);
        }

        /// <summary>
        ///     Remove a certain child from this entity
        /// </summary>
        /// <param name="child">The child to remove</param>
        /// <exception cref="ArgumentException">Thrown if this entity isn't the parent of specified child</exception>
        public void RemoveChild(GameEntity child)
        {
            if (child?.Parent != this)
                throw new ArgumentException(nameof(child) + " must be a child of this");
            ConnectEntities(child, null);
        }

        /// <summary>
        ///     Set parent of this entity, pass null to remove the parent
        /// </summary>
        /// <param name="parent">New parent</param>
        public void SetParent(GameEntity parent)
        {
            ConnectEntities(this, parent);
        }

        /// <summary>
        ///     Instantiate and add a component of certain type that has default constructor
        /// </summary>
        /// <typeparam name="T">Type of component to add</typeparam>
        /// <returns>The created and added component</returns>
        public T AddComponent<T>() where T : class, IGameComponent, new()
        {
            var component = new T();
            SetComponent(component);
            return component;
        }

        /// <summary>
        ///     Add an existing component to a game entity
        /// </summary>
        /// <param name="component">A component to add</param>
        /// <typeparam name="T"></typeparam>
        /// <exception cref="ArgumentNullException">Thrown if the component is null</exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if the component has already been attached to an entity
        /// </exception>
        public void AddComponent<T>(T component) where T : class, IGameComponent
        {
            if (component is null)
                throw new ArgumentNullException(nameof(component));
            if (component.Entity != null)
                throw new ArgumentException(nameof(component));
            SetComponent(component);
        }

        /// <summary>
        ///     Remove a component of certain type from this entity
        /// </summary>
        /// <typeparam name="T">Type of component to remove</typeparam>
        public void RemoveComponent<T>() where T : class, IGameComponent
        {
            SetComponent(null as T);
        }

        /// <summary>
        ///     Get a component of certain type
        /// </summary>
        /// <typeparam name="T">Type of component</typeparam>
        /// <returns>Component attached to this entity</returns>
        /// <exception cref="KeyNotFoundException">Thrown if there is no component of this type</exception>
        public T GetComponent<T>() where T : class, IGameComponent
        {
            return components.TryGetValue(typeof(T), out var value)
                ? value as T
                : throw new KeyNotFoundException(typeof(T).Name);
        }

        /// <summary>
        ///     Get all components of this type. Works only for base classes returning all subclasses
        /// </summary>
        /// <typeparam name="T">Type of components to return</typeparam>
        /// <returns>A lazy collection of all components of certain type</returns>
        public IEnumerable<T> GetComponentsOfType<T>()
        {
            return components.Values.OfType<T>();
        }

        /// <summary>
        ///     Try to get a component of certain type not throwing exceptions
        /// </summary>
        /// <param name="component">The component or null</param>
        /// <typeparam name="T">Type of component to get</typeparam>
        /// <returns>True on success, False otherwise</returns>
        public bool TryGetComponent<T>(out T component) where T : class, IGameComponent
        {
            var result = components.TryGetValue(typeof(T), out var comp);
            component = result ? comp as T : null;
            return result;
        }

        /// <summary>
        ///     Call `Initialize()` recursively for all children and components
        /// </summary>
        /// <param name="state">Current game state</param>
        public void Initialize(GameState state)
        {
            DoForAllChildren(c => c.Initialize(state), c => c.Initialize(state), false);
        }

        /// <summary>
        ///     Call `Update()` recursively for all children and components
        /// </summary>
        /// <param name="state">Current game state</param>
        public void Update(GameState state)
        {
            DoForAllChildren(c => c.Update(state), c => c.Update(state), false);
        }

        /// <summary>
        ///     Do a certain action for all children and components, components of children, etc.
        /// </summary>
        /// <param name="forChildren">An action to perform on child entities</param>
        /// <param name="forComponents">An action to perform on components</param>
        /// <param name="self">If true <see cref="forChildren" /> will be called on this</param>
        public void DoForAllChildren(Action<GameEntity> forChildren, Action<IGameComponent> forComponents,
            bool self = true)
        {
            if (self) forChildren(this);
            foreach (var component in components.Values)
                forComponents(component);
            foreach (var entity in children)
                forChildren(entity);
        }
    }
}