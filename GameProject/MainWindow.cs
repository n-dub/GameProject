using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using FarseerPhysics.Dynamics;
using GameProject.CoreEngine;
using GameProject.Ecs;
using GameProject.GameDebug;
using GameProject.GameGraphics;
using GameProject.GameGraphics.Backend.Direct2D;
using GameProject.GameGraphics.Backend.WinForms;
using GameProject.GameInput;
using GameProject.GameLogic.Scripts;
using GameProject.GameMath;
using Microsoft.Xna.Framework;
using unvell.D2DLib;

namespace GameProject
{
    /// <summary>
    ///     Represents main game window, holds main loop and entire ECS
    /// </summary>
    internal partial class MainWindow : Form
    {
        private Stopwatch Stopwatch { get; }
        private GameState GameState { get; }

#if DEBUG
        private bool DebugEnabled { get; set; } = true;
#else
        private bool DebugEnabled { get; set; }
#endif

        private float cachedFps;
        private float fpsShowTimer;

        private const int PhysicsSubsteps = 2;
        
        public MainWindow(IEnumerable<ISceneFactory> levels)
        {
            InitializeComponent();
            (Width, Height) = (800, 600);
            Application.Idle += (s, e) => MakeFrame();

            var renderer = new Renderer
            {
                Camera = {ScreenSize = new Vector2F(Width, Height), Position = Vector2F.UnitY * -1.5f}
            };
            var physicsWorld = new World(new Vector2(0, MathF.Gravity));

            GameState = new GameState(renderer, physicsWorld, CollectEntities(levels));

            Stopwatch = Stopwatch.StartNew();
        }

        private static List<GameEntity> CollectEntities(IEnumerable<ISceneFactory> levels)
        {
            var createdScenes = levels
                .Select(p => p.CreateScene())
                .ToList();

            var offset = float.NegativeInfinity;
            foreach (var scene in createdScenes)
            {
                if (float.IsInfinity(offset))
                    offset = scene.Offset;
                var currentOffset = offset;
                offset += scene.Width;
                scene.Entities.ForEach(e => e.DoForAllChildren(
                    x => x.Position += Vector2F.Zero.WithX(currentOffset - scene.Offset),
                    component => { }));
            }

            return createdScenes
                .SelectMany(p => p.Entities)
                .ToList();
        }

        protected override void OnLoad(EventArgs e)
        {
            GameState.RendererRead.Initialize(new D2DGraphicsDevice(this));
            if (GameState.RendererRead.Device is D2DGraphicsDevice d)
                d.Graphics.Antialias = true;
            
            DoubleBuffered = GameState.RendererWrite.Device is WinFormsGraphicsDevice;

            base.OnLoad(e);
        }

        private void MakeFrame(bool singleThread = false)
        {
            if (singleThread)
            {
                Render();
                UpdateGame();
            }
            else
            {
                var task = Task.Run(Render);
                UpdateGame();
                task.Wait();
            }
            GameState.SwapRenderers();
            Invalidate();
        }

        private void UpdateGame()
        {
            GameState.AddNewEntities();

            {
                // TODO: TEST CODE - TO BE REMOVED
                var first = GameState.Entities.FirstOrDefault();
                if (!(first?.HasComponent<TestCamera>() ?? false))
                    first?.AddComponent<TestCamera>();
                if (!(first?.HasComponent<TestCannon>() ?? false))
                    first?.AddComponent<TestCannon>();
                GameState.Time.TimeScale = GameState.Keyboard[Keys.Space] == KeyState.Pushing ? .1f : .01f;
            }

            GameState.RemoveDestroyed();
            GameState.Time.UpdateForNextFrame(GameState.Time.FrameIndex < 5 ? 1 : Stopwatch.ElapsedMilliseconds);
            Stopwatch.Restart();

            if (GameState.Keyboard[Keys.F3] == KeyState.Down)
                DebugEnabled = !DebugEnabled;

            foreach (var entity in GameState.Entities)
                entity.Update(GameState);

            foreach (var entity in GameState.Entities.Where(entity => entity.Destroyed))
                entity.DoForAllChildren(c => c.Destroy(), c => c.Destroy(GameState));

            for (var i = 0; i < PhysicsSubsteps; i++)
                GameState.PhysicsWorld.Step(GameState.Time.DeltaTime / PhysicsSubsteps);

            GameState.Keyboard.UpdateKeyStates();
            GameState.Mouse.UpdateKeyStates();
        }

        private void Render()
        {
            if (!GameState.RendererRead.Initialized)
                return;
            if (GameState.RendererRead.Device is WinFormsGraphicsDevice device)
                device.Graphics = CreateGraphics();

            GameState.RendererRead.Device.BeginRender();

            GameState.RendererRead.RenderAll();
            if (DebugEnabled)
                DrawDebug();
            ShowFps(GameState.RendererRead);

            GameState.RendererRead.Device.EndRender();
        }

        private void DrawDebug()
        {
            var debugger = new DebugDraw(GameState.RendererRead);
            
            foreach (var gameEntity in GameState.Entities)
                gameEntity.DoForAllChildren(
                    c => c.DrawDebugOverlay(debugger),
                    c =>
                    {
                        if (c is IDebuggable d)
                            d.DrawDebugOverlay(debugger);
                    });
        }

        private void ShowFps(Renderer renderer)
        {
            fpsShowTimer += GameState.Time.DeltaTimeUnscaled;
            if (fpsShowTimer > 1 || cachedFps == 0 || float.IsInfinity(cachedFps))
            {
                fpsShowTimer = 0;
                cachedFps = GameState.Time.Fps;
            }

            switch (renderer.Device)
            {
                case D2DGraphicsDevice device:
                    device.Graphics.ResetTransform();
                    device.Graphics.DrawText($"{Math.Round(cachedFps)} FPS",
                        D2DColor.Black,
                        "Arial",
                        10,
                        new D2DRect(5, 5, 250, 100));
                    break;
                case WinFormsGraphicsDevice device:
                    device.Graphics.ResetTransform();
                    device.Graphics.DrawString(
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
                    break;
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (GameState?.RendererRead?.Camera is null)
                return;
            
            GameState.RendererRead.Camera.ScreenSize = new Vector2F(Width, Height);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            GameState.Keyboard.PushKey(e.KeyCode);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            GameState.Mouse.PushKey(e.Button);
            GameState.Mouse.SetPositions(new Vector2F(e.Location), GameState.RendererRead.Camera.GetViewMatrix());
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
            GameState.Keyboard.ReleaseKey(e.KeyCode);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            GameState.Mouse.ReleaseKey(e.Button);
            GameState.Mouse.SetPositions(new Vector2F(e.Location), GameState.RendererRead.Camera.GetViewMatrix());
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            GameState.Mouse.SetWheel(e.Delta);
            GameState.Mouse.SetPositions(new Vector2F(e.Location), GameState.RendererRead.Camera.GetViewMatrix());
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            if (GameState.RendererRead.Device is WinFormsGraphicsDevice)
                base.OnPaintBackground(e);
        }
    }
}