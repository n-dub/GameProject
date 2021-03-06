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

        private readonly List<Joint> joints = new List<Joint>(4);

        public override void Destroy(GameState state)
        {
            foreach (var joint in joints) state.PhysicsWorld.RemoveJoint(joint);
        }

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

            wheel.Friction = 10;
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

        private void InitializeJoint(DistanceJoint joint)
        {
            joint.CollideConnected = false;
            joint.Frequency = 50;
            joints.Add(joint);
        }

        protected override void Update()
        {
            var body = Entity.GetComponent<PhysicsBody>();
            if (!body.ReadyToUse)
                return;

            if (GameState.Keyboard[Keys.A] == KeyState.Pushing)
                body.FarseerBody.ApplyTorque(-Torque * 0.02f);
            if (GameState.Keyboard[Keys.D] == KeyState.Pushing)
                body.FarseerBody.ApplyTorque(Torque * 0.02f);
        }
    }
}