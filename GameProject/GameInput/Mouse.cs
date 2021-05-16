﻿using System.Windows.Forms;
using GameProject.GameMath;

namespace GameProject.GameInput
{
    internal class Mouse : Keyboard<MouseButtons>
    {
        public Vector2F WorldPosition => RawPosition.TransformBy(View.Inversed);
        public float WheelDelta { get; private set; }

        public Vector2F ScreenPosition { get; private set; }

        private Matrix3F View { get; set; } = Matrix3F.Identity;
        private Vector2F RawPosition { get; set; }

        public void SetPositions(Vector2F screen, Vector2F size, Matrix3F view)
        {
            ScreenPosition = new Vector2F(screen.X / size.X - 0.5f, (screen.Y / size.Y - 0.5f) * size.Y / size.X);
            RawPosition = screen;
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