using System.Collections.Generic;
using System.Drawing;
using System.Runtime.CompilerServices;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
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
        ///     Indicates that the body is initialized and ready to use
        /// </summary>
        public bool ReadyToUse => FarseerBody != null;

        /// <summary>
        ///     A list of all colliders attached to this body
        /// </summary>
        public IReadOnlyList<Collider> Colliders => colliders;

        /// <summary>
        ///     List of all body collisions during the current frame
        /// </summary>
        public IReadOnlyList<CollisionInfo> Collisions => collisions;

        /// <summary>
        ///     If True this body's state won't be changed by any other bodies
        /// </summary>
        public bool IsStatic { get; set; }

        /// <summary>
        ///     Body's friction coefficient
        /// </summary>
        public float Friction { get; set; } = 1;

        /// <summary>
        ///     Body's angular damping
        /// </summary>
        public float AngularDamping { get; set; } = 1;

        /// <summary>
        ///     Body's linear damping
        /// </summary>
        public float LinearDamping { get; set; } = 1;

        public GameEntity Entity { get; set; }

        /// <summary>
        ///     The instance of Farseer physics native <see cref="Body" /> class
        /// </summary>
        public Body FarseerBody { get; private set; }

        private bool shapesDirty = true;
        private readonly List<Collider> colliders = new List<Collider>(1);
        private readonly List<CollisionInfo> collisions = new List<CollisionInfo>(8);

        public void Update(GameState state)
        {
            if (FarseerBody is null)
                FarseerBody = new Body(state.PhysicsWorld, Entity.Position, Entity.Rotation);

            FarseerBody.Friction = Friction;
            FarseerBody.AngularDamping = AngularDamping;
            FarseerBody.LinearDamping = LinearDamping;

            if (shapesDirty)
                InitializeShapes();

            ResolveCollisions(FarseerBody.ContactList);

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

        private void ResolveCollisions(ContactEdge contactList)
        {
            collisions.Clear();

            while (contactList != null)
            {
                var contact = contactList.Contact;
                if (contact.IsTouching() && contact.Enabled)
                {
                    contact.GetWorldManifold(out var normal, out var worldPoints);
                    for (var i = 0; i < contact.Manifold.PointCount; i++)
                        collisions.Add(new CollisionInfo(new Vector2F(worldPoints[i]),
                            new Vector2F(normal), contact.Manifold.Points[i].NormalImpulse));
                }

                contactList = contactList.Next;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void InitializeShapes()
        {
            shapesDirty = false;

            foreach (var fixture in FarseerBody.FixtureList)
                FarseerBody.DestroyFixture(fixture);

            foreach (var collider in colliders)
            {
                var fixture = FarseerBody.CreateFixture(collider.GetFarseerShape());
                fixture.UserData = collider;
            }

            FarseerBody.IsStatic = IsStatic;
        }
    }
}