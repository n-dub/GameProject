using System.Windows.Forms;
using GameProject.CoreEngine;
using GameProject.Ecs.Physics;
using GameProject.GameInput;

namespace GameProject.GameLogic.Scripts
{
    internal class WheelControl : GameScript
    {
        public float Torque { get; set; }
        
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