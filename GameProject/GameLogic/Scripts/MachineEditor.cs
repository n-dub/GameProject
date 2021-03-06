using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using GameProject.CoreEngine;
using GameProject.Ecs;
using GameProject.Ecs.Graphics;
using GameProject.GameGraphics.RenderShapes;
using GameProject.GameInput;
using GameProject.GameMath;

namespace GameProject.GameLogic.Scripts
{
    internal class MachineEditor : GameScript
    {
        public const float CellSize = 0.5f;
        public static bool HasInstance { get; private set; }

        public RectangleF WinRect { get; set; }

        private int Rows { get; }
        private int Columns { get; }

        private Point selectedCell = new Point(-1, -1);
        private MachineMenuResponse menuResponse;
        private bool closed;

        private readonly IMachinePartFactory[,] parts;
        private readonly float[,] partRotations;

        public MachineEditor(int rows, int columns)
        {
            HasInstance = true;
            Rows = rows;
            Columns = columns;
            parts = new IMachinePartFactory[columns, rows];
            partRotations = new float[columns, rows];
        }

        public override void Destroy(GameState state)
        {
            GameState.Time.TimeScale = 1f;
            HasInstance = false;
        }

        protected override void Initialize()
        {
            GameState.Time.TimeScale = 0f;

            var shapes = new List<IRenderShape>(Rows * Columns);

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

            shapes.Add(new QuadRenderShape(2)
            {
                Color = Color.DarkSlateGray,
                Scale = new Vector2F(1, Rows) * CellSize,
                Offset = Vector2F.UnitX * (Columns * CellSize * 0.5f + CellSize * 0.5f)
            });
            shapes.Add(new PolygonRenderShape(3)
            {
                Color = Color.White,
                Points = GeometryUtils
                    .GenerateRegularPolygon(Vector2F.Zero, Vector2F.One * CellSize * 0.7f,
                        MathF.PI * -0.5f, 3)
                    .ToArray(),
                Offset = Vector2F.UnitX * (Columns * CellSize * 0.5f + CellSize * 0.5f)
            });

            Entity.AddComponent(new Sprite(shapes));
        }

        protected override void Update()
        {
            if (closed)
                return;

            if (menuResponse != null && menuResponse.TryGetResult(out var factory))
            {
                var sprite = Entity.GetComponent<Sprite>();
                var oldShape = sprite.Shapes[selectedCell.ToLinearIndex(Columns)];
                var shape = new QuadRenderShape(1);
                partRotations[selectedCell.X, selectedCell.Y] += menuResponse.Rotation;
                shape.CopyDataFrom(oldShape);
                shape.Rotation += menuResponse.Rotation;
                shape.ImagePath = factory?.TexturePath ?? "Resources/ui/machine_slot.png";
                shape.Image = null;
                sprite.ReplaceShape(GameState, selectedCell.ToLinearIndex(Columns), shape);
                parts[selectedCell.X, selectedCell.Y] = factory;
                if (Math.Abs(menuResponse.Rotation - float.Epsilon) <= float.Epsilon)
                {
                    menuResponse = null;
                    selectedCell = new Point(-1, -1);
                }
                else
                {
                    RunMenu();
                }
            }
            else if (menuResponse != null)
            {
                return;
            }

            if (GameState.Mouse[MouseButtons.Left] == KeyState.Down)
            {
                var worldClickPos = GameState.Mouse.WorldPosition;
                selectedCell = GetCellIndex(worldClickPos);
                if (InBounds(false)) RunMenu();
                if (InBounds(true))
                {
                    closed = true;
                    foreach (var machine in SiegeMachine
                        .CreateMachines(parts, partRotations, Entity.Position, WinRect))
                        GameState.AddEntity(machine);
                    Entity.Destroy();
                    return;
                }
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

        private void RunMenu()
        {
            menuResponse = new MachineMenuResponse
            {
                Factory = parts[selectedCell.X, selectedCell.Y],
                Rotation = partRotations[selectedCell.X, selectedCell.Y]
            };
            var entity = new GameEntity();
            GameState.AddEntity(entity);
            entity.AddComponent(new MachineMenu(menuResponse));
        }

        private bool InBounds(bool playButton)
        {
            return selectedCell.Y >= 0
                   && selectedCell.Y < Rows
                   && (playButton
                       ? selectedCell.X == Columns
                       : selectedCell.X >= 0 && selectedCell.X < Columns);
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