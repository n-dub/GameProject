using System;
using System.Collections.Generic;
using System.Windows.Forms;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Factories;
using GameProject.CoreEngine;
using GameProject.Ecs;
using GameProject.Ecs.Physics;
using GameProject.GameInput;
using Microsoft.Xna.Framework;

namespace GameProject.GameLogic.Scripts
{
    internal class WheelControl : GameScript
    {
        public float Torque { get; set; }
        public GameEntity Machine { get; set; }

        protected override void Initialize()
        {
            StartCoroutine(FinishInit);
        }

        private IEnumerable<Awaiter> FinishInit()
        {
            yield return Awaiter.WaitForFrames(2);

            var wheel = Entity.GetComponent<PhysicsBody>();
            var machine = Machine.GetComponent<PhysicsBody>();
            if (!machine.ReadyToUse || !wheel.ReadyToUse)
                throw new Exception();

            wheel.Friction = 5;
            const float f = 0.1f;
            var wheelPosition = wheel.FarseerBody.Position;
            var machinePosition = machine.FarseerBody.Position;
            // var wheelPosition = Entity.Position;
            // var machinePosition = Machine.Position;
            
            InitializeJoint(JointFactory.CreateDistanceJoint(GameState.PhysicsWorld,
                wheel.FarseerBody, machine.FarseerBody,
                wheelPosition, machinePosition + Vector2.UnitX * f));
            InitializeJoint(JointFactory.CreateDistanceJoint(GameState.PhysicsWorld,
                wheel.FarseerBody, machine.FarseerBody,
                wheelPosition, machinePosition + Vector2.UnitY * f));
            InitializeJoint(JointFactory.CreateDistanceJoint(GameState.PhysicsWorld,
                wheel.FarseerBody, machine.FarseerBody,
                wheelPosition, machinePosition - Vector2.UnitX * f));
            InitializeJoint(JointFactory.CreateDistanceJoint(GameState.PhysicsWorld,
                wheel.FarseerBody, machine.FarseerBody,
                wheelPosition, machinePosition - Vector2.UnitY * f));
        }

        private static void InitializeJoint(DistanceJoint joint)
        {
            joint.CollideConnected = false;
            joint.Frequency = 50;
        }

        protected override void Update()
        {
            var body = Entity.GetComponent<PhysicsBody>();
            if (!body.ReadyToUse)
                return;

            var dt = GameState.Time.DeltaTime;
            if (GameState.Keyboard[Keys.A] == KeyState.Pushing)
                body.FarseerBody.ApplyAngularImpulse(-Torque * dt);
            if (GameState.Keyboard[Keys.D] == KeyState.Pushing)
                body.FarseerBody.ApplyAngularImpulse(Torque * dt);
        }
    }
}