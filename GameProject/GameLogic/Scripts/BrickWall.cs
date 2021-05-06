using System;
using System.Collections.Generic;
using System.Linq;
using FarseerPhysics.Dynamics;
using GameProject.CoreEngine;
using GameProject.Ecs;
using GameProject.Ecs.Graphics;
using GameProject.Ecs.Physics;
using GameProject.GameGraphics.RenderShapes;
using GameProject.GameMath;

namespace GameProject.GameLogic.Scripts
{
    internal class BrickWall : GameScript
    {
        private float Strength { get; }

        private readonly Random random = new Random();

        private int RowCount { get; }

        private PartialBrickRow BottomPartialBrickRow { get; }

        private PartialBrickRow TopPartialBrickRow { get; }

        private bool StartWithTwoBricks { get; }

        private const string DetachedBrickPath = "Resources/bricks/detached_brick.png";
        private const int MaxBrokenRows = 3;

        private readonly List<GameEntity> parts = new List<GameEntity>(8);

        public BrickWall(int rowCount,
            float strength = 1f,
            bool startWithTwoBricks = true,
            PartialBrickRow bottomPartialBrickRow = PartialBrickRow.None,
            PartialBrickRow topPartialBrickRow = PartialBrickRow.None)
        {
            RowCount = rowCount;
            Strength = strength;
            BottomPartialBrickRow = bottomPartialBrickRow;
            TopPartialBrickRow = topPartialBrickRow;
            StartWithTwoBricks = startWithTwoBricks;
        }

        protected override void Initialize()
        {
            var body = Entity.AddComponent<PhysicsBody>();

            var rows = RowCount;
            var offset = 0f;

            if (BottomPartialBrickRow != PartialBrickRow.None)
            {
                --rows;
                offset -= 0.5f * LevelUtility.BrickSize.Z;
            }

            if (TopPartialBrickRow != PartialBrickRow.None)
            {
                --rows;
                offset += 0.5f * LevelUtility.BrickSize.Z;
            }

            body.AddCollider(new BoxCollider
            {
                Size = new Vector2F(LevelUtility.BrickSize.X * 2, LevelUtility.BrickSize.Z * rows),
                Offset = new Vector2F(0, offset)
            });

            Entity.AddComponent<Sprite>();

            StartCoroutine(CreateSprites);
        }

        protected override void Update()
        {
            var body = Entity.GetComponent<PhysicsBody>();
            if (!body.ReadyToUse)
                return;

            var collision = FindContact(body);

            if (collision.NormalImpulse < Strength)
                return;

            var localPoint = collision.Point.TransformBy(Entity.GlobalTransform.Inversed);
            var count = MathF.Clamp(RowCount - 0.5f * (localPoint.Y + 1) * RowCount, 0, RowCount);

            Split((int) MathF.Round(count), GameState);
        }

        private static CollisionInfo FindContact(PhysicsBody body)
        {
            return body.Collisions.Aggregate((0f, null as CollisionInfo?),
                (t, x) => t.Item1 < x.NormalImpulse
                    ? (x.NormalImpulse, x)
                    : t).Item2.GetValueOrDefault();
            // var result = null as CollisionInfo?;
            // var impulse = 0f;
            // 
            // foreach (var collision in body.Collisions)
            // {
            //     if (collision.NormalImpulse > impulse)
            //     {
            //         impulse = collision.NormalImpulse;
            //         result = collision;
            //     }
            // }
            // 
            // return result;
        }

        private IEnumerable<Awaiter> CreateSprites()
        {
            yield return Awaiter.WaitForNextFrame();

            var height = 0.5f * LevelUtility.BrickSize.Z * (RowCount - 1);
            for (var i = 0; i < RowCount; i++)
            {
                var part = PartialBrickRow.None;
                if (i == 0)
                    part = BottomPartialBrickRow;
                else if (i == RowCount - 1)
                    part = TopPartialBrickRow;

                if (StartWithTwoBricks == (i % 2 == 0))
                    CreateTwoBrickRow(height, part);
                else
                    CreateThreeBrickRow(height, part);
                height -= LevelUtility.BrickSize.Z;
            }
        }

        private void Split(int rowIndex, GameState state)
        {
            if (rowIndex < 0 || rowIndex >= RowCount)
                return;

            var (part1, part2, detachedCount) = GenerateRandomPartialRows();

            var detachedPosition =
                Entity.Position + Entity.Up * LevelUtility.BrickSize.Z * (rowIndex - 0.5f * RowCount + 0.5f);

            DetachBricks(detachedCount, detachedPosition);

            if (rowIndex != 0)
            {
                var partPosition =
                    Entity.Position + 0.5f * LevelUtility.BrickSize.Z * (rowIndex - RowCount) * Entity.Up;
                if (rowIndex <= MaxBrokenRows)
                {
                    DetachBricks(rowIndex, partPosition);
                }
                else
                {
                    var part = new GameEntity();
                    state.AddEntity(part);
                    part.AddComponent(new BrickWall(rowIndex, Strength, StartWithTwoBricks,
                        BottomPartialBrickRow, part1));
                    part.Rotation = Entity.Rotation;
                    part.Position = partPosition;
                    parts.Add(part);
                }
            }

            if (rowIndex != RowCount - 1)
            {
                var partPosition = Entity.Position + 0.5f * LevelUtility.BrickSize.Z * rowIndex * Entity.Up;
                if (RowCount - rowIndex - 1 <= MaxBrokenRows)
                {
                    DetachBricks(RowCount - rowIndex - 1, partPosition);
                }
                else
                {
                    var part = new GameEntity();
                    state.AddEntity(part);
                    part.AddComponent(new BrickWall(RowCount - rowIndex - 1, Strength, StartWithTwoBricks,
                        part2, TopPartialBrickRow));
                    part.Rotation = Entity.Rotation;
                    part.Position = partPosition;
                    parts.Add(part);
                }
            }

            Entity.GetComponent<PhysicsBody>().FarseerBody.Enabled = false;
            StartCoroutine(FinishSplit);
        }

        private void DetachBricks(int detachedCount, Vector2F position)
        {
            var offset = Entity.Right * LevelUtility.BrickSize.Y;
            for (var i = 0; i < detachedCount; i++)
                GameState.AddEntity(CreateDisposableBrick(position + offset));
            for (var i = 0; i < detachedCount; i++)
                GameState.AddEntity(CreateDisposableBrick(position - offset));
            
            for (var i = 0; i < detachedCount; i++)
                GameState.AddEntity(CreateDisposableBrick(position + offset * 2));
            for (var i = 0; i < detachedCount; i++)
                GameState.AddEntity(CreateDisposableBrick(position - offset * 2));
        }

        private GameEntity CreateDisposableBrick(Vector2F position)
        {
            var entity = LevelUtility.CreateBrick(position, true, Entity.Rotation);
            parts.Add(entity);
            
            if (random.Next(2) == 0)
                entity.Destroy(float.PositiveInfinity);
            else
                entity.Destroy(random.Next(5, 30));

            return entity;
        }

        private IEnumerable<Awaiter> FinishSplit()
        {
            yield return Awaiter.WaitForFrames(3);
            var farseerBody = Entity.GetComponent<PhysicsBody>().FarseerBody;
            foreach (var part in parts)
                CopyVelocity(farseerBody, part.GetComponent<PhysicsBody>().FarseerBody);
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

        private (PartialBrickRow, PartialBrickRow, int) GenerateRandomPartialRows()
        {
            var totalDetached = 0;

            var left = PartialBrickRow.Left + random.Next(2);
            if (left != PartialBrickRow.None)
                totalDetached++;

            var right = PartialBrickRow.Left + random.Next(2);
            if (right != PartialBrickRow.None)
                totalDetached++;

            return (left, right, totalDetached);
        }

        private void CreateThreeBrickRow(float height, PartialBrickRow partialBrickRow)
        {
            var sprite = Entity.GetComponent<Sprite>();
            if (partialBrickRow == PartialBrickRow.None)
                sprite.Shapes.Add(GenerateWideBrick(Vector2F.UnitY * height));
            var offset = LevelUtility.BrickSize.X + LevelUtility.BrickSize.Y;
            if (partialBrickRow != PartialBrickRow.Right)
                sprite.Shapes.Add(GenerateNarrowBrick(new Vector2F(offset / -2, height)));
            if (partialBrickRow != PartialBrickRow.Left)
                sprite.Shapes.Add(GenerateNarrowBrick(new Vector2F(offset / +2, height)));
        }

        private void CreateTwoBrickRow(float height, PartialBrickRow partialBrickRow)
        {
            var sprite = Entity.GetComponent<Sprite>();
            if (partialBrickRow != PartialBrickRow.Right)
                sprite.Shapes.Add(GenerateWideBrick(new Vector2F(LevelUtility.BrickSize.X / -2, height)));
            if (partialBrickRow != PartialBrickRow.Left)
                sprite.Shapes.Add(GenerateWideBrick(new Vector2F(LevelUtility.BrickSize.X / +2, height)));
        }

        private static QuadRenderShape GenerateNarrowBrick(Vector2F position)
        {
            return new QuadRenderShape(1)
            {
                ImagePath = DetachedBrickPath,
                Offset = position,
                Scale = LevelUtility.BrickSize.Yz
            };
        }

        private static QuadRenderShape GenerateWideBrick(Vector2F position)
        {
            return new QuadRenderShape(1)
            {
                ImagePath = DetachedBrickPath,
                Offset = position,
                Scale = LevelUtility.BrickSize.Xz
            };
        }
    }
}