using System.Windows.Forms;
using GameProject.GameMath;

namespace GameProject.GameInput
{
    internal class Mouse : Keyboard<MouseButtons>
    {
        public float WheelDelta { get; private set; }
        
        public Vector2F ScreenPosition { get; private set; }

        public Vector2F WorldPosition => ScreenPosition.TransformBy(View.Inversed);

        private Matrix3F View { get; set; } = Matrix3F.Identity;
        
        public void SetPositions(Vector2F screen, Matrix3F view)
        {
            ScreenPosition = screen;
            View = view;
        }
        
        public void SetWheel(float delta)
        {
            WheelDelta = delta;
        }

        public override void UpdateKeyStates()
        {
            base.UpdateKeyStates();
            WheelDelta = 0;
        }
    }
}