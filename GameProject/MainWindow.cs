using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using FarseerPhysics.Dynamics;
using GameProject.CoreEngine;
using GameProject.Ecs;
using GameProject.Ecs.Graphics;
using GameProject.Ecs.Physics;
using GameProject.GameGraphics;
using GameProject.GameGraphics.Direct2D;
using GameProject.GameGraphics.WinForms;
using GameProject.GameInput;
using GameProject.GameMath;
using GameProject.GameScripts;
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

        protected override void OnLoad(EventArgs e)
        {
            {
                // TODO: TEST CODE - TO BE REMOVED
                Entities.AddRange(Enumerable
                    .Range(0, 3)
                    .Select(_ => new GameEntity()));

                Entities.ForEach(entity => entity.AddComponent(new Sprite(new QuadRenderShape(1))));
                Entities.ForEach(entity => entity.AddComponent<PhysicsBody>());

                Entities[0].AddComponent<BoxCollider>();
                Entities[1].AddComponent<BoxCollider>();
                Entities[2].AddComponent<CircleCollider>();
                Entities[2].GetComponent<CircleCollider>().Radius = 0.5f;

                Entities[0].Scale = new Vector2F(7, 1);
                Entities[1].Scale = new Vector2F(5, 1);
                Entities[0].GetComponent<PhysicsBody>().IsStatic = true;

                Entities[1].Position = new Vector2F(0, -3);
                Entities[2].Position = new Vector2F(0.5f, -2);
                Entities[0].AddComponent<TestCamera>();
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

        public MainWindow()
        {
            InitializeComponent();

            Entities = new List<GameEntity>();
            var renderer = new Renderer();
            var physicsWorld = new World(new Vector2(0, MathF.Gravity));
            GameState = new GameState(renderer, new Keyboard(), new Time(), physicsWorld);

            Application.Idle += (s, e) => UpdateGame();

            Stopwatch = Stopwatch.StartNew();
            (Width, Height) = (800, 600); // TODO: don't hard-code this???
            renderer.Camera.ScreenSize = new Vector2F(Width, Height);
        }
    }
}