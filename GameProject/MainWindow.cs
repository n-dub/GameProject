﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
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

namespace GameProject
{
    /// <summary>
    ///     Represents main game window, holds main loop and entire ECS
    /// </summary>
    internal partial class MainWindow : Form
    {
        private Stopwatch Stopwatch { get; }
        private GameState GameState { get; }

        private bool DebugEnabled { get; set; }

        private float cachedFps;
        private float fpsShowTimer;

        public MainWindow(IEnumerable<ISceneFactory> levels)
        {
            InitializeComponent();
            (Width, Height) = (800, 600);
            Application.Idle += (s, e) => UpdateGame();

            var renderer = new Renderer();
            renderer.Camera.ScreenSize = new Vector2F(Width, Height);
            var physicsWorld = new World(new Vector2(0, MathF.Gravity));

            GameState = new GameState(renderer, new Keyboard(), new Time(), physicsWorld, CollectEntities(levels));

            {
                // TODO: TEST CODE - TO BE REMOVED
                GameState.Entities.First().AddComponent<TestCamera>();
            }

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
            GameState.Renderer.Initialize(new D2DGraphicsDevice(this));
            DoubleBuffered = GameState.Renderer.Device is WinFormsGraphicsDevice;
            base.OnLoad(e);
        }

        private void UpdateGame()
        {
            GameState.InitializeAdded();
            GameState.RemoveDestroyed();
            GameState.Time.UpdateForNextFrame(Stopwatch.ElapsedMilliseconds);
            Stopwatch.Restart();

            if (GameState.Keyboard[Keys.F3] == KeyState.Down)
                DebugEnabled = !DebugEnabled;

            foreach (var entity in GameState.Entities)
                entity.Update(GameState);

            foreach (var entity in GameState.Entities.Where(entity => entity.Destroyed))
                entity.DoForAllChildren(c => c.Destroy(), c => c.Destroy(GameState));

            GameState.PhysicsWorld.Step(GameState.Time.DeltaTime);
            GameState.Keyboard.UpdateKeyStates();
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (!GameState.Renderer.Initialized)
                return;
            if (GameState.Renderer.Device is WinFormsGraphicsDevice winFormsGraphicsDevice)
                winFormsGraphicsDevice.Graphics = e.Graphics;

            GameState.Renderer.Device.BeginRender();

            GameState.Renderer.RenderAll();
            if (DebugEnabled)
                DrawDebug(e.Graphics);

            GameState.Renderer.Device.EndRender();
        }

        private void DrawDebug(Graphics g)
        {
            var debugger = new DebugDraw(GameState.Renderer);
            //debugger.BeginRender();
            foreach (var gameEntity in GameState.Entities)
                gameEntity.DoForAllChildren(
                    c => c.DrawDebugOverlay(debugger),
                    c =>
                    {
                        if (c is IDebuggable d)
                            d.DrawDebugOverlay(debugger);
                    });
            ShowFps(g);
        }

        private void ShowFps(Graphics graphics)
        {
            fpsShowTimer += GameState.Time.DeltaTime * 1;
            if (fpsShowTimer > 1 || cachedFps == 0 || float.IsInfinity(cachedFps))
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