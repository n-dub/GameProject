using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using FarseerPhysics.Dynamics;
using GameProject.CoreEngine;
using GameProject.Ecs;
using GameProject.GameGraphics;
using GameProject.GameGraphics.Direct2D;
using GameProject.GameGraphics.WinForms;
using GameProject.GameInput;
using GameProject.GameMath;
using Microsoft.Xna.Framework;

namespace GameProject
{
    /// <summary>
    ///     Represents main game window, holds main loop and entire ECS
    /// </summary>
    internal partial class MainWindow : Form
    {
        private Stopwatch Stopwatch { get; }
        private GameState GameState { get; }

        private List<GameEntity> Entities { get; }

        private float cachedFps;
        private float fpsShowTimer;

        public MainWindow(ISceneFactory sceneFactory)
        {
            InitializeComponent();
            (Width, Height) = (800, 600);
            Application.Idle += (s, e) => UpdateGame();
            
            Entities = sceneFactory.CreateScene();
            var renderer = new Renderer();
            var physicsWorld = new World(new Vector2(0, MathF.Gravity));
            GameState = new GameState(renderer, new Keyboard(), new Time(), physicsWorld);

            Stopwatch = Stopwatch.StartNew();
            renderer.Camera.ScreenSize = new Vector2F(Width, Height);
        }

        protected override void OnLoad(EventArgs e)
        {
            GameState.Renderer.Initialize(new D2DGraphicsDevice(this));
            Entities.ForEach(entity => entity.Initialize(GameState));
            DoubleBuffered = GameState.Renderer.Device is WinFormsGraphicsDevice;
            base.OnLoad(e);
        }

        private void UpdateGame()
        {
            GameState.Time.UpdateForNextFrame(Stopwatch.ElapsedMilliseconds);
            Stopwatch.Restart();

            Entities.ForEach(entity => entity.Update(GameState));

            GameState.PhysicsWorld.Step(GameState.Time.DeltaTime / 1000);
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
            fpsShowTimer += GameState.Time.DeltaTime;
            if (fpsShowTimer > 1000 || cachedFps == 0 || float.IsInfinity(cachedFps))
            {
                fpsShowTimer = 0;
                cachedFps = GameState.Time.Fps;
            }

            graphics.ResetTransform();
            graphics.DrawString(
                $"{Math.Round(cachedFps)} FPS",
                new Font("Arial", 10),
                Brushes.Black,
                new Rectangle(5, 5, 250, 100),
                new StringFormat
                {
                    Alignment = StringAlignment.Near,
                    LineAlignment = StringAlignment.Near,
                    FormatFlags = StringFormatFlags.FitBlackBox
                }
            );
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            GameState.Keyboard.PushKey(e.KeyCode);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
            GameState.Keyboard.ReleaseKey(e.KeyCode);
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            if (GameState.Renderer.Device is WinFormsGraphicsDevice)
                base.OnPaintBackground(e);
        }
    }
}