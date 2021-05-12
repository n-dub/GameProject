using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using GameProject.CoreEngine;
using GameProject.Ecs;
using GameProject.Ecs.Physics;
using GameProject.GameDebug;
using GameProject.GameInput;
using GameProject.GameMath;

namespace GameProject.GameLogic.Scripts
{
    internal class TestCannon : GameScript, IDebuggable
    {
        private GameEntity bullet;
        private GameEntity marker;
        private Vector2F position = new Vector2F(-8, -1f);
        private float impulse = 2f;

        protected override void Initialize()
        {
            marker = LevelUtility.CreateCircle(position, 0.2f, true);
            GameState.AddEntity(marker);
        }

        protected override void Update()
        {
            if (GameState.Keyboard[Keys.H] == KeyState.Down)
            {
                bullet = LevelUtility.CreateCircle(position, 0.15f);
                GameState.AddEntity(bullet);
                StartCoroutine(FinishBullet);
            }

            if (GameState.Keyboard[Keys.ControlKey] == KeyState.Pushing)
                impulse -= GameState.Mouse.WheelDelta / 500;
            else
                position = position.WithY(position.Y - GameState.Mouse.WheelDelta / 500);
            marker.Position = position;
            marker.Scale = Vector2F.One * impulse / 5;
        }

        private IEnumerable<Awaiter> FinishBullet()
        {
            var b = bullet;
            bullet = null;
            yield return Awaiter.WaitForFrames(2);
            b.GetComponent<PhysicsBody>().ApplyLinearImpulse(new Vector2F(impulse, 0));
            b.GetComponent<PhysicsBody>().FarseerBody.Position = position;
            b.Destroy(5);
        }

        public void DrawDebugOverlay(DebugDraw debugDraw)
        {
            debugDraw.DrawVector(position, Vector2F.UnitX, Color.Red);
        }
    }
}