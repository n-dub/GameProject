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
        
        public override void Update(GameState state)
        {
            if (bullet != null && bullet.GetComponent<PhysicsBody>().FarseerBody != null)
            {
                bullet.GetComponent<PhysicsBody>().ApplyForce(new Vector2F(1000, 0));
                bullet = null;
            }
            if (state.Keyboard[Keys.H] == KeyState.Down)
            {
                bullet = LevelUtility.CreateCircle(new Vector2F(-10, -3), 0.5f);
                state.AddEntity(bullet);
            }
        }
    }
}