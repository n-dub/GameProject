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
            InitializeJoint(JointFactory.CreateDistanceJoint(GameState.PhysicsWorld,
                wheel.FarseerBody, machine.FarseerBody,
                wheel.FarseerBody.Position, machine.FarseerBody.Position + Vector2.UnitX * 0.3f));
            InitializeJoint(JointFactory.CreateDistanceJoint(GameState.PhysicsWorld,
                wheel.FarseerBody, machine.FarseerBody,
                wheel.FarseerBody.Position, machine.FarseerBody.Position - Vector2.UnitX * 0.3f));
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

            if (GameState.Keyboard[Keys.A] == KeyState.Pushing)
                body.FarseerBody.ApplyAngularImpulse(-Torque);
            if (GameState.Keyboard[Keys.D] == KeyState.Pushing)
                body.FarseerBody.ApplyAngularImpulse(Torque);
        }
    }
}