using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using GameProject.CoreEngine;
using GameProject.GameDebug;
using GameProject.GameMath;

namespace GameProject.Ecs
{
    /// <summary>
    ///     Represents a game entity (game object)
    /// </summary>
    internal class GameEntity : IDebuggable
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
        public Matrix3F GlobalTransform => /*Parent is null
            ? LocalTransform
            : Parent.GlobalTransform * */LocalTransform;

        public Vector2F Right => Vector2F.UnitX.Rotate(Rotation);

        public Vector2F Up => -Vector2F.UnitY.Rotate(Rotation);

        /// <summary>
        ///     Parent of this game entity
        /// </summary>
        public GameEntity Parent { get; private set; }

        /// <summary>
        ///     Position of this entity (in meters)
        /// </summary>
        public Vector2F Position
        {
            get => position;
            set
            {
                position = value;
                localTransform = null;
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
                localTransform = null;
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
                localTransform = null;
            }
        }

        internal bool Destroyed { get; private set; }

        /// <summary>
        ///     Local transformation of this entity, will be recalculated
        ///     if transformation parameters have been changed after the last time
        /// </summary>
        private Matrix3F LocalTransform =>
            localTransform
            ?? Matrix3F.CreateTranslation(Position)
            * Matrix3F.CreateRotation(Rotation)
            * Matrix3F.CreateScale(Scale);

        private float timeLeft = float.PositiveInfinity;

        private readonly List<GameEntity> children;

        private readonly Dictionary<Type, IGameComponent> components;

        private readonly List<Action> componentCommands;

        private Matrix3F? localTransform;

        private Vector2F position;
        private float rotation;
        private Vector2F scale = Vector2F.One;

        /// <summary>
        ///     Create a new game entity
        /// </summary>
        public GameEntity()
        {
            components = new Dictionary<Type, IGameComponent>(8);
            componentCommands = new List<Action>(4);
            children = new List<GameEntity>(4);
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
            if (HasComponent<T>())
                throw new ArgumentException($"component of type \"{typeof(T).Name}\" already existed");
            var component = new T();
            SetComponent(typeof(T), component);
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
            if (HasComponent(component.GetType()))
                throw new ArgumentException($"component of type {component.GetType().Name} already existed");
            if (component is null)
                throw new ArgumentNullException(nameof(component));
            if (component.Entity != null)
                throw new ArgumentException(nameof(component));
            SetComponent(component.GetType(), component);
        }

        /// <summary>
        ///     Remove a component of certain type from this entity
        /// </summary>
        /// <typeparam name="T">Type of component to remove</typeparam>
        public void RemoveComponent<T>() where T : class, IGameComponent
        {
            SetComponent(typeof(T), null);
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
        ///     Call `Update()` recursively for all children and components
        /// </summary>
        /// <param name="state">Current game state</param>
        public void Update(GameState state)
        {
            FlushComponents();
            timeLeft -= state.Time.DeltaTime;
            Destroyed = timeLeft <= 0;
            if (Destroyed)
                return;

            DoForAllChildren(c => c.Update(state), c => c.Update(state), false);
        }

        /// <summary>
        ///     Execute all add/remove component commands
        /// </summary>
        public void FlushComponents()
        {
            componentCommands.InvokeAll();
        }

        /// <summary>
        ///     Do a certain action for all children and components, components of children, etc.
        /// </summary>
        /// <param name="forEntities">An action to perform on child entities</param>
        /// <param name="forComponents">An action to perform on components</param>
        /// <param name="self">If true <see cref="forEntities" /> will be called on this</param>
        public void DoForAllChildren(Action<GameEntity> forEntities, Action<IGameComponent> forComponents,
            bool self = true)
        {
            if (self) forEntities(this);
            foreach (var component in components.Values)
                forComponents(component);
            foreach (var entity in children)
                entity.DoForAllChildren(forEntities, forComponents);
        }

        /// <summary>
        ///     Defer destruction of this entity, it will be actually deleted when all update logic is finished
        /// </summary>
        /// <param name="time">Time in seconds to wait before destroying</param>
        public void Destroy(float time = 0)
        {
            timeLeft = time;
        }

        public void DrawDebugOverlay(DebugDraw debugDraw)
        {
            debugDraw.DrawVector(Position, Right, Color.Red);
            debugDraw.DrawVector(Position, Up, Color.Red);
        }

        public bool HasComponent<T>() where T : class, IGameComponent
        {
            return HasComponent(typeof(T));
        }

        public T GetOrAddComponent<T>() where T : class, IGameComponent, new()
        {
            return HasComponent<T>() ? GetComponent<T>() : AddComponent<T>();
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
        /// <param name="type">Type of component</param>
        /// <param name="component">Component to set</param>
        private void SetComponent(Type type, IGameComponent component)
        {
            if (components.TryGetValue(type, out var previous))
            {
                previous.Entity = null;
                componentCommands.Add(() => components.Remove(type));
            }

            if (component is null)
                return;

            var prevEntity = component.Entity;
            component.Entity = this;
            componentCommands.Add(() =>
            {
                prevEntity?.components.Remove(type);
                components[type] = component;
            });
        }

        private bool HasComponent(Type componentType)
        {
            return components.TryGetValue(componentType, out _);
        }
    }
}