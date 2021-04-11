using GameProject.GameGraphics;
using GameProject.GameInput;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using GameProject.CoreEngine;
using GameProject.Ecs;
using GameProject.Ecs.Graphics;
using GameProject.GameGraphics.Direct2D;
using GameProject.GameGraphics.WinForms;

namespace GameProject
{
    internal partial class MainWindow : Form
    {
        private Stopwatch Stopwatch { get; }
        private GameState GameState { get; }

        private List<GameEntity> Entities { get; }

        public MainWindow()
        {
            InitializeComponent();

            var renderer = new Renderer(Enumerable
                .Range(0, 1000)
                .Select(_ => new QuadRenderShape(0) {IsActive = true}));
            var keyboard = new Keyboard();
            var time = new Time();
            Entities = new List<GameEntity>();
            GameState = new GameState(renderer, keyboard, time);

            Application.Idle += (s, e) => UpdateGame();

            Stopwatch = Stopwatch.StartNew();
            (Width, Height) = (800, 600);
        }

        protected override void OnLoad(EventArgs e)
        {
            {
                // TODO: TEST CODE - TO BE REMOVED
                Entities.AddRange(Enumerable
                    .Range(1, 1000)
                    .Select(_ => new GameEntity()));
                Entities.ForEach(entity => entity.AddComponent(new SpriteComponent(new QuadRenderShape(1))));
            }
            GameState.Renderer.Initialize(new D2DGraphicsDevice(this));
            DoubleBuffered = GameState.Renderer.Device is WinFormsGraphicsDevice;
            Entities.ForEach(entity => entity.Initialize(GameState));
            base.OnLoad(e);
        }

        private void UpdateGame()
        {
            GameState.Time.UpdateForNextFrame(Stopwatch.ElapsedMilliseconds);
            Stopwatch.Restart();

            Entities.ForEach(entity => entity.Update(GameState));

            GameState.Keyboard.UpdateKeyStates();
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (!GameState.Renderer.Initialized)
                return;
            if (GameState.Renderer.Device is WinFormsGraphicsDevice winFormsGraphicsDevice)
                winFormsGraphicsDevice.Graphics = e.Graphics;

            GameState.Renderer.RenderAll();
            {
                // TODO: TEST CODE - TO BE REMOVED
                ShowFps(e.Graphics);
            }
        }

        private void ShowFps(Graphics graphics)
        {
            graphics.ResetTransform();
            graphics.DrawString(
                $"{GameState.Time.Fps:F3} FPS",
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

        protected override void OnKeyDown(KeyEventArgs e) => GameState.Keyboard.PushKey(e.KeyCode);

        protected override void OnKeyUp(KeyEventArgs e) => GameState.Keyboard.ReleaseKey(e.KeyCode);

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            if (GameState.Renderer.Device is WinFormsGraphicsDevice)
                base.OnPaintBackground(e);
        }
    }
}