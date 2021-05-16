using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using GameProject.CoreEngine;
using GameProject.Ecs.Graphics;
using GameProject.GameGraphics.RenderShapes;
using GameProject.GameInput;
using GameProject.GameLogic.Scripts.MachineParts;
using GameProject.GameMath;

namespace GameProject.GameLogic.Scripts
{
    internal class MachineMenu : GameScript
    {
        private static readonly IMachinePartFactory[] factories =
        {
            null,
            new WoodenBoxFactory(),
            new SmallWheelFactory()
        };

        private MachineMenuResponse Response { get; }

        private static readonly Vector2F size = new Vector2F(0.3f, 1f);
        private const float Padding = 0.05f;
        private const int Columns = 3;

        private const float CellSize = 0.1f;

        public MachineMenu(MachineMenuResponse response)
        {
            Response = response;
        }

        protected override void Initialize()
        {
            Entity.Scale = Vector2F.One;
            Entity.Position = Vector2F.Zero;

            var shapes = new List<QuadRenderShape>(factories.Length + 1);
            var aspectRatio = GameState.RendererWrite.Camera.AspectRatio;
            shapes.Add(new QuadRenderShape(2)
            {
                Color = Color.FromArgb(140, Color.Black),
                Scale = new Vector2F(size.X, size.Y / aspectRatio - Padding * 2),
                Offset = new Vector2F(0.5f - Padding - size.X * 0.5f, 0)
            });
            for (var i = 0; i < factories.Length; i++)
            {
                var coords = new Point(i % Columns, i / Columns);
                shapes.Add(new QuadRenderShape(4)
                {
                    ImagePath = factories[i]?.TexturePath ?? "Resources/ui/machine_slot.png",
                    Offset = CellIndexToOffset(coords),
                    Scale = Vector2F.One * CellSize * 0.85f
                });
                shapes.Add(new QuadRenderShape(3)
                {
                    Color = Color.FromArgb(100, Color.White),
                    Offset = CellIndexToOffset(coords),
                    Scale = Vector2F.One * CellSize * 0.9f
                });
            }

            Entity.AddComponent(new Sprite(shapes, RenderLayer.Interface));
        }

        protected override void Update()
        {
            if (!Entity.HasComponent<Sprite>() || GameState.Mouse[MouseButtons.Left] != KeyState.Down)
                return;

            var cursor = (GameState.Mouse.ScreenPosition - CellIndexToOffset(Point.Empty)) / CellSize
                         + Vector2F.One * 0.5f;
            var index = new Point((int) MathF.Floor(cursor.X), (int) MathF.Floor(cursor.Y))
                .ToLinearIndex(Columns);
            if (index >= factories.Length || index < 0)
                return;

            Response.Factory = factories[index];
            Response.IsComplete = true;
            Entity.Destroy();
        }

        private Vector2F CellIndexToOffset(Point coords)
        {
            var aspectRatio = GameState.RendererWrite.Camera.AspectRatio;
            var x = 0.5f - size.X + CellSize * 0.5f - Padding;
            var y = CellSize * 1.5f - size.Y * 0.5f / aspectRatio - Padding;
            return new Vector2F(x, y) + new Vector2F(coords) * CellSize;
        }
    }
}