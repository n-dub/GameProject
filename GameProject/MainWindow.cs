using GameProject.GameGraphics;
using GameProject.GameInput;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using GameProject.CoreEngine;
using GameProject.GameGraphics.Direct2D;
using GameProject.GameGraphics.WinForms;

namespace GameProject
{
    internal partial class MainWindow : Form
    {
        private Renderer Renderer { get; }
        private IGraphicsDevice Device { get; set; }
        private Keyboard Keyboard { get; }
        private Stopwatch Stopwatch { get; }

        public MainWindow()
        {
            InitializeComponent();

            Renderer = new Renderer(Enumerable
                .Range(0, 1000)
                .Select(_ => new RenderQuadCommand(0) {IsActive = true}));
            Keyboard = new Keyboard();

            Application.Idle += (s, e) => UpdateGame();

            Stopwatch = Stopwatch.StartNew();
        }

        protected override void OnLoad(EventArgs e)
        {
            Device = new D2DGraphicsDevice(this);
            DoubleBuffered = Device is WinFormsGraphicsDevice;
            base.OnLoad(e);
        }

        private void UpdateGame()
        {
            Time.UpdateForNextFrame(Stopwatch.ElapsedMilliseconds);
            Stopwatch.Restart();
            // TODO: Update game components here
            Keyboard.UpdateKeyStates();
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (Device is WinFormsGraphicsDevice d)
                d.Graphics = e.Graphics;
            if (!Renderer.Initialized)
                Renderer.Initialize(Device);
            Renderer.RenderAll(Device);
            ShowFps(e.Graphics);
        }

        private static void ShowFps(Graphics graphics)
        {
            graphics.ResetTransform();
            graphics.DrawString(
                $"{Time.Fps:F3} FPS",
                new Font("Arial", 16),
                Brushes.Black,
                new Rectangle(0, 0, 250, 100),
                new StringFormat
                {
                    Alignment = StringAlignment.Near,
                    LineAlignment = StringAlignment.Center,
                    FormatFlags = StringFormatFlags.FitBlackBox
                }
            );
        }

        protected override void OnKeyDown(KeyEventArgs e) => Keyboard.PushKey(e.KeyCode);

        protected override void OnKeyUp(KeyEventArgs e) => Keyboard.ReleaseKey(e.KeyCode);

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            if (Device is WinFormsGraphicsDevice)
                base.OnPaintBackground(e);
        }
    }
}