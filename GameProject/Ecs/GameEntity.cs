using System;
using System.Collections.Generic;
using System.Linq;
using GameProject.CoreEngine;
using GameProject.GameMath;

namespace GameProject.Ecs
{
    internal class GameEntity
    {
        public GameEntity Parent { get; private set; }
        public IEnumerable<IGameComponent> Components => components.Select(x => x.Value);
        public IReadOnlyList<GameEntity> Children => children;

        public Matrix3F LocalTransform
        {
            get => localTransform
                   ?? Matrix3F.CreateTranslation(Position)
                   * Matrix3F.CreateRotation(Rotation)
                   * Matrix3F.CreateScale(Scale);
            private set => localTransform = value;
        }

        public Matrix3F GlobalTransform => Parent is null
            ? LocalTransform
            : Parent.GlobalTransform * LocalTransform;

        public Vector2F Position
        {
            get => position;
            set
            {
                position = value;
                LocalTransform = null;
            }
        }

        public float Rotation
        {
            get => rotation;
            set
            {
                rotation = value;
                LocalTransform = null;
            }
        }

        public Vector2F Scale
        {
            get => scale;
            set
            {
                scale = value;
                LocalTransform = null;
            }
        }

        private readonly Dictionary<Type, IGameComponent> components;
        private readonly List<GameEntity> children;

        private Matrix3F localTransform;

        private Vector2F position;
        private float rotation;
        private Vector2F scale;

        public GameEntity()
        {
            components = new Dictionary<Type, IGameComponent>();
            children = new List<GameEntity>();
        }

        public GameEntity(GameEntity parent) : this() => ConnectEntities(this, parent);

        public void AddChild(GameEntity child) => ConnectEntities(child, this);

        public void RemoveChild(GameEntity child)
        {
            if (child?.Parent != this)
                throw new ArgumentException(nameof(child) + " must be a child of this");
            ConnectEntities(child, null);
        }

        public void SetParent(GameEntity parent) => ConnectEntities(this, parent);

        public T AddComponent<T>() where T : class, IGameComponent, new()
        {
            var component = new T();
            SetComponent(component);
            return component;
        }

        public void AddComponent<T>(T component) where T : class, IGameComponent
        {
            if (component is null)
                throw new ArgumentNullException(nameof(component));
            SetComponent(component);
        }

        public void RemoveComponent<T>() where T : class, IGameComponent => SetComponent(null as T);

        public void SetComponent<T>(T component) where T : class, IGameComponent
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

        public T GetComponent<T>() where T : class, IGameComponent => components[typeof(T)] as T;

        public bool TryGetComponent<T>(out T component) where T : class, IGameComponent
        {
            var result = components.TryGetValue(typeof(T), out var comp);
            component = comp as T;
            return result;
        }

        public void Initialize(GameState state) =>
            DoForAllChildren(c => c.Initialize(state), c => c.Initialize(state));

        public void Update(GameState state) =>
            DoForAllChildren(c => c.Update(state), c => c.Update(state));

        public void DoForAllChildren(Action<GameEntity> forChildren, Action<IGameComponent> forComponents)
        {
            foreach (var component in components.Values)
                forComponents(component);
            foreach (var entity in children)
                forChildren(entity);
        }

        private static void ConnectEntities(GameEntity child, GameEntity parent)
        {
            if (child is null)
                throw new ArgumentNullException(nameof(child));
            child.Parent?.children.Remove(child);
            child.Parent = parent;
            parent?.children.Add(child);
        }
    }
}