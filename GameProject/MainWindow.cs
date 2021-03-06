using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using FarseerPhysics.Dynamics;
using GameProject.CoreEngine;
using GameProject.GameDebug;
using GameProject.GameGraphics;
using GameProject.GameGraphics.Backend.Direct2D;
using GameProject.GameGraphics.Backend.WinForms;
using GameProject.GameInput;
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
            WindowState = FormWindowState.Maximized;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            Application.Idle += (s, e) => MakeFrame();

            var renderer = new Renderer
            {
                Camera = {ScreenSize = new Vector2F(Width, Height), Position = Vector2F.UnitY * -1.5f}
            };
            var physicsWorld = new World(new Vector2(0, MathF.Gravity));

            var level = levels.First().CreateScene();
            var entities = level.Entities;
            GameState = new GameState(renderer, physicsWorld, entities);
            if (level.Camera != null)
                GameState.RendererWrite.Camera.CopyDataFrom(level.Camera);

            Stopwatch = Stopwatch.StartNew();
        }

        protected override void OnLoad(EventArgs e)
        {
            GameState.RendererRead.Initialize(new D2DGraphicsDevice(this));
            if (GameState.RendererRead.Device is D2DGraphicsDevice d)
                d.Graphics.Antialias = true;

            DoubleBuffered = GameState.RendererWrite.Device is WinFormsGraphicsDevice;
            MinimumSize = MaximumSize = Size;

            base.OnLoad(e);
        }

        private void MakeFrame(bool singleThread = false)
        {
            using (new GameProfiler(nameof(MakeFrame)))
            {
                GameState.RendererRead.Camera.ScreenSize = new Vector2F(Width, Height);
                if (singleThread)
                {
                    Render(null);
                    UpdateGame();
                }
                else
                {
                    var task = Task.Run(UpdateGame);
                    Render(task.Wait);
                    task.Wait();
                }

                using (new GameProfiler(nameof(GameState.ExchangeRenderData)))
                {
                    GameState.ExchangeRenderData();
                    Invalidate();
                }
            }
        }

        private void UpdateGame()
        {
            using (new GameProfiler(nameof(UpdateGame)))
            {
                GameState.AddNewEntities();

                GameState.RemoveDestroyed();
                GameState.Time.UpdateForNextFrame(GameState.Time.FrameIndex < 5 ? 1 : Stopwatch.ElapsedMilliseconds);
                Stopwatch.Restart();

                if (GameState.Keyboard[Keys.F3] == KeyState.Down)
                    DebugEnabled = !DebugEnabled;

                foreach (var entity in GameState.Entities)
                    entity.Update(GameState);

                foreach (var entity in GameState.Entities.Where(entity => entity.Destroyed))
                    entity.DoForAllChildren(c => c.Destroy(), c => c.Destroy(GameState));

                using (new GameProfiler(nameof(World.Step)))
                {
                    for (var i = 0; i < PhysicsSubsteps; i++)
                        GameState.PhysicsWorld.Step(GameState.Time.DeltaTime / PhysicsSubsteps);
                }

                GameState.Keyboard.UpdateKeyStates();
                GameState.Mouse.UpdateKeyStates();
            }
        }

        private void Render(Action waitForUpdateGame)
        {
            using (new GameProfiler(nameof(Render)))
            {
                if (!GameState.RendererRead.Initialized)
                    return;
                if (GameState.RendererRead.Device is WinFormsGraphicsDevice device)
                    device.Graphics = CreateGraphics();

                GameState.RendererRead.Device.BeginRender();

                using (new GameProfiler(nameof(GameState.RendererRead.RenderAll)))
                {
                    GameState.RendererRead.RenderAll();
                }

                if (DebugEnabled)
                {
                    waitForUpdateGame?.Invoke();
                    DrawDebug();
                }

                using (new GameProfiler(nameof(ShowFps)))
                {
                    ShowFps(GameState.RendererRead);
                }

                using (new GameProfiler(nameof(GameState.RendererRead.Device.EndRender)))
                {
                    GameState.RendererRead.Device.EndRender();
                }
            }
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            GameState.Mouse.SetPositions(new Vector2F(e.Location), new Vector2F(Width, Height),
                GameState.RendererRead.Camera.GetViewMatrix());
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            GameState.Mouse.PushKey(e.Button);
            GameState.Mouse.SetPositions(new Vector2F(e.Location), new Vector2F(Width, Height),
                GameState.RendererRead.Camera.GetViewMatrix());
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
            GameState.Mouse.SetPositions(new Vector2F(e.Location), new Vector2F(Width, Height),
                GameState.RendererRead.Camera.GetViewMatrix());
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            GameState.Mouse.SetWheel(e.Delta);
            GameState.Mouse.SetPositions(new Vector2F(e.Location), new Vector2F(Width, Height),
                GameState.RendererRead.Camera.GetViewMatrix());
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            if (GameState.RendererRead.Device is WinFormsGraphicsDevice)
                base.OnPaintBackground(e);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            GameProfiler.Save("prof.json");
        }
    }
}