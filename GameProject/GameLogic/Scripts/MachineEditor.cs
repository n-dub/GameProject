using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using GameProject.CoreEngine;
using GameProject.Ecs;
using GameProject.Ecs.Graphics;
using GameProject.Ecs.Physics;
using GameProject.GameGraphics.RenderShapes;
using GameProject.GameInput;
using GameProject.GameMath;

namespace GameProject.GameLogic.Scripts
{
    internal class MachineEditor : GameScript
    {
        public const float CellSize = 0.5f;

        private int Rows { get; }
        private int Columns { get; }

        private Point selectedCell = new Point(-1, -1);
        private MachineMenuResponse menuResponse;
        private bool closed;

        private readonly IMachinePartFactory[,] parts;

        public MachineEditor(int rows, int columns)
        {
            Rows = rows;
            Columns = columns;
            parts = new IMachinePartFactory[columns, rows];
        }

        public override void Destroy(GameState state)
        {
            GameState.Time.TimeScale = 1;
        }

        protected override void Initialize()
        {
            GameState.Time.TimeScale = 0;

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
                shape.CopyDataFrom(oldShape);
                shape.ImagePath = factory?.TexturePath ?? "Resources/ui/machine_slot.png";
                shape.Image = null;
                sprite.ReplaceShape(GameState, selectedCell.ToLinearIndex(Columns), shape);
                parts[selectedCell.X, selectedCell.Y] = factory;
                menuResponse = null;
                selectedCell = new Point(-1, -1);
            }
            else if (menuResponse != null)
            {
                return;
            }

            if (GameState.Mouse[MouseButtons.Left] == KeyState.Down)
            {
                var worldClickPos = GameState.Mouse.WorldPosition;
                selectedCell = GetCellIndex(worldClickPos);
                if (InBounds(false))
                {
                    menuResponse = new MachineMenuResponse();
                    var entity = new GameEntity();
                    GameState.AddEntity(entity);
                    entity.AddComponent(new MachineMenu(menuResponse));
                }
                else if (InBounds(true))
                {
                    closed = true;
                    StartCoroutine(BuildMachineAndClose);
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

        private IEnumerable<Awaiter> BuildMachineAndClose()
        {
            var machine = new GameEntity{Position = Entity.Position};
            var body = machine.AddComponent<PhysicsBody>();
            machine.AddComponent<Sprite>();
            GameState.AddEntity(machine);
            yield return Awaiter.WaitForFrames(2);

            var collisionData = new bool[Columns, Rows];
            var cellCenter = new Vector2F(CellSize) * 0.5f;
            var center = new Vector2F(Columns, Rows) * CellSize * 0.5f - cellCenter;
            for (var i = 0; i < Columns; i++)
            for (var j = 0; j < Rows; j++)
            {
                if (parts[i, j] is null)
                    continue;
                
                var cellPosition = new Vector2F(i, j) * CellSize - center;
                if (parts[i, j].HasBoxCollision)
                    collisionData[i, j] = true;
                parts[i, j].CreatePart(cellPosition, GameState, machine);
            }

            GenerateCollision(collisionData, body);
            
            yield return Awaiter.WaitForNextFrame();
            Entity.Destroy();
        }

        private void GenerateCollision(bool[,] collisionData, PhysicsBody body)
        {
            var (x, y) = (collisionData.GetLength(0), collisionData.GetLength(1));
            var unvisited = GetUnvisited(collisionData, x, y);

            if (!unvisited.Any())
                return;

            var result = new List<(Point, Point)>();

            while (unvisited.Any())
            {
                var (i, j) = unvisited.First();
                var xSpan = GetXSpan(collisionData, i, j, x, unvisited);

                var ySpan = GetYSpan(collisionData, i, j, y, xSpan, unvisited);

                result.Add((new Point(i, j), new Point(i + xSpan - 1, j + ySpan - 1)));
            }

            foreach (var (p1, p2) in result)
            {
                var cellCenter = new Vector2F(CellSize) * 0.5f;
                var center = new Vector2F(Columns, Rows) * CellSize * 0.5f - cellCenter;
                var offset = new Vector2F(p1) * CellSize - center;
                var size = new Vector2F(p2.X - p1.X + 1, p2.Y - p1.Y + 1) * CellSize;
                
                body.AddCollider(new BoxCollider
                {
                    Offset = offset + size * 0.5f - Vector2F.One * CellSize * 0.5f,
                    Scaled = false,
                    Size = size
                });
            }
        }

        private static int GetYSpan(bool[,] collisionData, int initX, int initY, int height, int xSpan,
            ICollection<(int, int)> unvisited)
        {
            var ySpan = 1;
            for (var k = initY + 1; k < height; k++)
            {
                var rowHasCollision = true;
                for (var l = initX; l < initX + xSpan; l++)
                {
                    if (collisionData[l, k]) continue;
                    rowHasCollision = false;
                    break;
                }

                if (rowHasCollision)
                {
                    ++ySpan;
                    for (var l = initX; l < initX + xSpan; l++)
                        unvisited.Remove((l, k));
                }
                else break;
            }

            return ySpan;
        }

        private static int GetXSpan(bool[,] collisionData, int initX, int initY, int width,
            ICollection<(int, int)> unvisited)
        {
            var xSpan = 0;
            for (var k = initX; k < width; k++)
            {
                if (collisionData[k, initY])
                {
                    ++xSpan;
                    unvisited.Remove((k, initY));
                }
                else break;
            }

            return xSpan;
        }

        private static HashSet<(int, int)> GetUnvisited(bool[,] collisionData, int x, int y)
        {
            var unvisited = new HashSet<(int, int)>();

            for (var i = 0; i < x; i++)
            for (var j = 0; j < y; j++)
            {
                if (collisionData[i, j])
                    unvisited.Add((i, j));
            }

            return unvisited;
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