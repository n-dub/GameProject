using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using FarseerPhysics.Dynamics;
using GameProject.CoreEngine;
using GameProject.Ecs;
using GameProject.Ecs.Graphics;
using GameProject.Ecs.Physics;
using GameProject.GameMath;

namespace GameProject.GameLogic.Scripts
{
    internal class SiegeMachine : GameScript
    {
        private RectangleF WinRect { get; }

        private const float BreakImpulse = 3f;

        private readonly IMachinePartFactory[,] partsMatrix;
        private readonly float[,] partRotations;
        private readonly Dictionary<Vector2F, Point> partOffsets;

        private int Rows => partsMatrix.GetLength(1);
        private int Columns => partsMatrix.GetLength(0);

        private bool initialized;
        private List<GameEntity> createdMachines;
        private CollisionInfo breakCollision;

        private SiegeMachine(IMachinePartFactory[,] matrix, float[,] rotations, RectangleF winRect)
        {
            WinRect = winRect;
            partsMatrix = matrix;
            partRotations = rotations;
            partOffsets = new Dictionary<Vector2F, Point>(matrix.Length);
        }

        public static IEnumerable<GameEntity> CreateMachines(IMachinePartFactory[,] parts,
            float[,] rotations, Vector2F position, RectangleF winRect, float rotation = 0)
        {
            var visited = new HashSet<Point>();

            for (var i = 0; i < parts.GetLength(0); i++)
            for (var j = 0; j < parts.GetLength(1); j++)
            {
                var start = new Point(i, j);
                if (parts[i, j] is null || visited.Contains(start))
                    continue;

                var p = CoreUtils.RunBreadthFirstSearch(parts, start,
                    factory => factory is null || !factory.Connectible, visited);
                var machine = new GameEntity {Position = position, Rotation = rotation};
                machine.AddComponent(new SiegeMachine(p, rotations, winRect));
                yield return machine;
            }
        }

        protected override void Update()
        {
            if (!initialized)
                return;

            if (WinRect.Contains(Entity.Position))
            {
                if (MessageBox.Show(@"You won!") == DialogResult.OK) Application.Exit();
                initialized = false;
                return;
            }

            breakCollision = CoreUtils.FindMaximumImpulseContact(Entity.GetComponent<PhysicsBody>());
            if (breakCollision.NormalImpulse < BreakImpulse)
                return;

            var minDistance = float.PositiveInfinity;
            var minDistancePart = partOffsets.First().Value;
            var transform = Entity.GlobalTransform;

            foreach (var (position, part) in partOffsets.Select(pair => (pair.Key, pair.Value)))
            {
                var pos = position.TransformBy(transform);
                var distance = (breakCollision.Point - pos).LengthSquared;
                if (distance > minDistance) continue;
                minDistance = distance;
                minDistancePart = part;
            }

            var newParts = new IMachinePartFactory[Columns, Rows];
            for (var i = 0; i < Columns; i++)
            for (var j = 0; j < Rows; j++)
                if (new Point(i, j) != minDistancePart)
                {
                    newParts[i, j] = partsMatrix[i, j];
                }
                else
                {
                    var cellPosition = GetLocalCellPosition(i, j).TransformBy(Entity.GlobalTransform);
                    var destroyed = partsMatrix[i, j].CreateDestroyedPart(cellPosition, Entity.Rotation);
                    GameState.AddEntity(destroyed);
                }

            createdMachines = CreateMachines(newParts, partRotations, Entity.Position, WinRect, Entity.Rotation)
                .ToList();
            foreach (var machine in createdMachines)
                GameState.AddEntity(machine);

            foreach (var part in partsMatrix)
                part?.CleanUp();

            StartCoroutine(FinishMachineBreak);
            GameState.Time.TimeScale = 0f;
            initialized = false;
        }

        private IEnumerable<Awaiter> FinishMachineBreak()
        {
            yield return Awaiter.WaitForFrames(6);
            var body = Entity.GetComponent<PhysicsBody>();
            foreach (var machine in createdMachines)
                CopyVelocity(body.FarseerBody, machine.GetComponent<PhysicsBody>().FarseerBody);

            yield return Awaiter.WaitForNextFrame();
            Entity.Destroy();
        }

        private static void CopyVelocity(Body source, Body destination)
        {
            if (destination is null || source is null)
                return;

            destination.AngularDamping = source.AngularDamping;
            destination.AngularVelocity = source.AngularVelocity;
            destination.LinearVelocity = source.LinearVelocity * 1.5f;
        }

        protected override void Initialize()
        {
            StartCoroutine(BuildMachine);
        }

        private IEnumerable<Awaiter> BuildMachine()
        {
            var body = Entity.AddComponent<PhysicsBody>();
            Entity.AddComponent<Sprite>();
            yield return Awaiter.WaitForNextFrame();

            var collisionData = new bool[Columns, Rows];
            for (var i = 0; i < Columns; i++)
            for (var j = 0; j < Rows; j++)
            {
                if (partsMatrix[i, j] is null)
                    continue;

                if (partsMatrix[i, j].HasBoxCollision)
                    collisionData[i, j] = true;
                var cellPosition = GetLocalCellPosition(i, j);
                partsMatrix[i, j].CreatePart(cellPosition, partRotations[i, j], GameState, Entity);
                partOffsets.Add(cellPosition, new Point(i, j));
            }

            GenerateCollision(collisionData, body, Rows, Columns);
            initialized = true;
            yield return Awaiter.WaitForFrames(3);
            GameState.Time.TimeScale = 1;
        }

        private Vector2F GetLocalCellPosition(int i, int j)
        {
            var cellCenter = new Vector2F(MachineEditor.CellSize) * 0.5f;
            var center = new Vector2F(Columns, Rows) * MachineEditor.CellSize * 0.5f - cellCenter;
            return new Vector2F(i, j) * MachineEditor.CellSize - center;
        }

        private static void GenerateCollision(bool[,] collisionData, PhysicsBody body, int rows, int columns)
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
                var cellCenter = new Vector2F(MachineEditor.CellSize) * 0.5f;
                var center = new Vector2F(columns, rows) * MachineEditor.CellSize * 0.5f - cellCenter;
                var offset = new Vector2F(p1) * MachineEditor.CellSize - center;
                var size = new Vector2F(p2.X - p1.X + 1, p2.Y - p1.Y + 1) * MachineEditor.CellSize;

                body.AddCollider(new BoxCollider
                {
                    Offset = offset + size * 0.5f - Vector2F.One * MachineEditor.CellSize * 0.5f,
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
                else
                {
                    break;
                }
            }

            return ySpan;
        }

        private static int GetXSpan(bool[,] collisionData, int initX, int initY, int width,
            ICollection<(int, int)> unvisited)
        {
            var xSpan = 0;
            for (var k = initX; k < width; k++)
                if (collisionData[k, initY])
                {
                    ++xSpan;
                    unvisited.Remove((k, initY));
                }
                else
                {
                    break;
                }

            return xSpan;
        }

        private static HashSet<(int, int)> GetUnvisited(bool[,] collisionData, int x, int y)
        {
            var unvisited = new HashSet<(int, int)>();

            for (var i = 0; i < x; i++)
            for (var j = 0; j < y; j++)
                if (collisionData[i, j])
                    unvisited.Add((i, j));

            return unvisited;
        }
    }
}