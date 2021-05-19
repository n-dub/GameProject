using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using GameProject.CoreEngine;
using GameProject.Ecs;
using GameProject.Ecs.Graphics;
using GameProject.Ecs.Physics;
using GameProject.GameMath;

namespace GameProject.GameLogic.Scripts
{
    internal class SiegeMachine : GameScript
    {
        private readonly IMachinePartFactory[,] partsMatrix;

        private int Rows => partsMatrix.GetLength(1);
        private int Columns => partsMatrix.GetLength(0);

        private bool initialized;
        
        public SiegeMachine(IMachinePartFactory[,] matrix)
        {
            partsMatrix = matrix;
        }

        protected override void Initialize()
        {
            StartCoroutine(BuildMachine);
        }
        
        private IEnumerable<Awaiter> BuildMachine()
        {
            var body = Entity.AddComponent<PhysicsBody>();
            Entity.AddComponent<Sprite>();
            yield return Awaiter.WaitForFrames(2);

            var collisionData = new bool[Columns, Rows];
            var cellCenter = new Vector2F(MachineEditor.CellSize) * 0.5f;
            var center = new Vector2F(Columns, Rows) * MachineEditor.CellSize * 0.5f - cellCenter;
            for (var i = 0; i < Columns; i++)
            for (var j = 0; j < Rows; j++)
            {
                if (partsMatrix[i, j] is null)
                    continue;
                
                var cellPosition = new Vector2F(i, j) * MachineEditor.CellSize - center;
                if (partsMatrix[i, j].HasBoxCollision)
                    collisionData[i, j] = true;
                partsMatrix[i, j].CreatePart(cellPosition, GameState, Entity);
            }

            GenerateCollision(collisionData, body);
            initialized = true;
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
                var cellCenter = new Vector2F(MachineEditor.CellSize) * 0.5f;
                var center = new Vector2F(Columns, Rows) * MachineEditor.CellSize * 0.5f - cellCenter;
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
    }
}