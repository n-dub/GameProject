using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using GameProject.CoreEngine;
using GameProject.Ecs.Graphics;
using GameProject.GameGraphics.RenderShapes;
using GameProject.GameInput;
using GameProject.GameMath;

namespace GameProject.GameLogic.Scripts
{
    internal class MachineEditor : GameScript
    {
        private int Rows { get; set; }
        private int Columns { get; set; }

        private const float CellSize = 0.5f;

        private Point selectedCell = new Point(-1, -1);
        private MachineMenuResponse menuResponse;

        public MachineEditor(int rows, int columns)
        {
            Rows = rows;
            Columns = columns;
        }

        protected override void Initialize()
        {
            GameState.Time.TimeScale = 0;
            
            var shapes = new List<QuadRenderShape>(Rows * Columns);

            var cellCenter = new Vector2F(CellSize) * 0.5f;
            var center = new Vector2F(Columns, Rows) * CellSize * 0.5f - cellCenter;
            
            for (var i = 0; i < Rows; i++)
            for (var j = 0; j < Columns; j++)
            {
                var shape = new QuadRenderShape(2)
                {
                    ImagePath = "Resources/ui/machine_slot.png",
                    Scale = Vector2F.One * CellSize,
                    Offset = new Vector2F(j, i) * CellSize - center
                };
                shapes.Add(shape);
            }
            
            Entity.AddComponent(new Sprite(shapes));
        }

        protected override void Update()
        {
            if (GameState.Mouse[MouseButtons.Left] == KeyState.Down)
            {
                var worldClickPos = GameState.Mouse.WorldPosition;
                selectedCell = GetCellIndex(worldClickPos);
                if (InBounds(selectedCell))
                {
                    menuResponse = new MachineMenuResponse();
                    Entity.AddComponent(new MachineMenu(menuResponse));
                }
            }

            if (menuResponse?.TryGetResult(out var partIndex) ?? false)
            {
                // TODO
                menuResponse.Factory = null;
            }

            var shapes = Entity.GetComponent<Sprite>().Shapes;
            for (var i = 0; i < Rows; i++)
            for (var j = 0; j < Columns; j++)
            {
                if (!(shapes[i * Columns + j] is QuadRenderShape shape))
                    continue;
                shape.Scale = selectedCell == new Point(j, i)
                    ? Vector2F.One * CellSize * 0.9f
                    : Vector2F.One * CellSize;
            }
        }

        private bool InBounds(Point index)
        {
            return index.X >= 0 && index.Y >= 0 && index.X < Columns && index.Y < Rows;
        }

        private Point GetCellIndex(Vector2F worldClickPos)
        {
            var center = new Vector2F(Columns, Rows) * CellSize * 0.5f;
            var relative = worldClickPos - Entity.Position + center;
            var index = relative / CellSize;
            return new Point((int) MathF.Floor(index.X), (int) MathF.Floor(index.Y));
        }
    }
}