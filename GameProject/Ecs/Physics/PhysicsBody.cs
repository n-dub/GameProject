using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using FarseerPhysics.Dynamics;
using GameProject.CoreEngine;
using GameProject.GameDebug;
using GameProject.GameMath;

namespace GameProject.Ecs.Physics
{
    /// <summary>
    ///     Represents a dynamic or static physical rigid body
    /// </summary>
    internal class PhysicsBody : IGameComponent, IDebuggable
    {
        /// <summary>
        ///     If True this body's state won't be changed by any other bodies
        /// </summary>
        public bool IsStatic { get; set; }

        public GameEntity Entity { get; set; }
        
        /// <summary>
        ///     The instance of Farseer physics native <see cref="Body"/> class
        /// </summary>
        public Body FarseerBody { get; private set; }
        
        /// <summary>
        ///     A list of all colliders attached to this body
        /// </summary>
        public IList<Collider> Colliders => colliders;

        private bool shapesDirty = true;
        private readonly List<Collider> colliders = new List<Collider>(1);

        public void Update(GameState state)
        {
            if (FarseerBody is null)
                FarseerBody = new Body(state.PhysicsWorld, Entity.Position, Entity.Rotation);
            if (shapesDirty)
                InitializeShapes();

            FarseerBody.IsStatic = IsStatic;
            Entity.Position = new Vector2F(FarseerBody.Position);
            Entity.Rotation = FarseerBody.Rotation;
        }

        public void Destroy(GameState state)
        {
            state.PhysicsWorld.RemoveBody(FarseerBody);
        }

        public void DrawDebugOverlay(DebugDraw debugDraw)
        {
            if (FarseerBody is null)
                return;
            debugDraw.DrawVector(Entity.Position, new Vector2F(FarseerBody.LinearVelocity), Color.Blue);

            foreach (var collider in colliders)
                if (collider is IDebuggable d)
                    d.DrawDebugOverlay(debugDraw);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ApplyLinearImpulse(Vector2F impulse)
        {
            FarseerBody?.ApplyLinearImpulse(impulse);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddCollider(Collider collider)
        {
            colliders.Add(collider);
            collider.Entity = Entity;
            shapesDirty = true;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void InitializeShapes()
        {
            shapesDirty = false;

            foreach (var fixture in FarseerBody.FixtureList)
                FarseerBody.DestroyFixture(fixture);
            
            foreach (var shape in colliders.Select(c => c.GetFarseerShape()))
                FarseerBody.CreateFixture(shape);
            
            FarseerBody.IsStatic = IsStatic;
        }
    }
}