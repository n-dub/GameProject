using System.Windows.Forms;
using GameProject.CoreEngine;
using GameProject.Ecs;
using GameProject.Ecs.Physics;
using GameProject.GameInput;
using GameProject.GameMath;

namespace GameProject.GameLogic.Scripts
{
    internal class TestCannon : GameScript
    {
        private GameEntity bullet;
        
        protected override void Update()
        {
            if (bullet?.GetComponent<PhysicsBody>().FarseerBody != null)
            {
                bullet.GetComponent<PhysicsBody>().ApplyLinearImpulse(new Vector2F(2f, 0));
                bullet = null;
            }
            if (GameState.Keyboard[Keys.H] == KeyState.Down)
            {
                bullet = LevelUtility.CreateCircle(new Vector2F(-8, -2.5f), 0.15f);
                GameState.AddEntity(bullet);
            }
        }
    }
}