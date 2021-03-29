using GameProject.GameGraphics;
using GameProject.GameInput;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameProject
{
    public partial class MainWindow : Form
    {
        public Renderer Renderer { get; }
        public Keyboard Keyboard { get; }

        private Timer Timer { get; }

        public MainWindow()
        {
            InitializeComponent();
            Renderer = new Renderer();
            Keyboard = new Keyboard();
            Timer = new Timer { Interval = 1000 / 60 };
            Timer.Tick += (s, e) => UpdateGame();
            Timer.Start();
        }

        private void UpdateGame()
        {
            if (Keyboard[Keys.A] == KeyState.Down)
                Console.WriteLine("Down");
            else if (Keyboard[Keys.A] == KeyState.Up)
                Console.WriteLine("Up");
            else if (Keyboard[Keys.A] == KeyState.Pushing)
                Console.WriteLine("Push");

            Keyboard.UpdateKeyStates();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Renderer.RenderAll(e.Graphics);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            Keyboard.PushKey(e.KeyCode);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            Keyboard.ReleaseKey(e.KeyCode);
        }
    }
}
