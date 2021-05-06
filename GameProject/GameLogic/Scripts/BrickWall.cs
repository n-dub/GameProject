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

        private GameEntity partA;
        private GameEntity partB;

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

            var collision = body.Collisions
                .OrderByDescending(c => c.NormalImpulse)
                .Select(c => new CollisionInfo?(c))
                .FirstOrDefault();
            
            if (collision is null || collision.Value.NormalImpulse < Strength)
                return;
            
            Split(RowCount / 2, GameState);
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

            var detachedPosition = Entity.Position
                                   + Entity.Up * LevelUtility.BrickSize.Z * (rowIndex - 0.5f * RowCount + 0.5f);

            for (var i = 0; i < detachedCount * 2; i++)
                state.AddEntity(LevelUtility.CreateBrick(detachedPosition, true));

            if (rowIndex != 0)
            {
                partA = new GameEntity();
                state.AddEntity(partA);
                partA.AddComponent(new BrickWall(rowIndex, Strength, StartWithTwoBricks,
                    BottomPartialBrickRow, part1));
                partA.Rotation = Entity.Rotation;
                partA.Position = Entity.Position + 0.5f * LevelUtility.BrickSize.Z * (rowIndex - RowCount) * Entity.Up;
            }

            if (rowIndex != RowCount - 1)
            {
                partB = new GameEntity();
                state.AddEntity(partB);
                partB.AddComponent(new BrickWall(RowCount - rowIndex - 1, Strength, StartWithTwoBricks,
                    part2, TopPartialBrickRow));
                partB.Rotation = Entity.Rotation;
                partB.Position = Entity.Position + 0.5f * LevelUtility.BrickSize.Z * rowIndex * Entity.Up;
            }

            Entity.GetComponent<PhysicsBody>().FarseerBody.Enabled = false;
            StartCoroutine(FinishSplit);
        }

        private IEnumerable<Awaiter> FinishSplit()
        {
            yield return Awaiter.WaitForFrames(3);
            var farseerBody = Entity.GetComponent<PhysicsBody>().FarseerBody;
            CopyVelocity(farseerBody, partA?.GetComponent<PhysicsBody>().FarseerBody);
            CopyVelocity(farseerBody, partB?.GetComponent<PhysicsBody>().FarseerBody);
            yield return Awaiter.WaitForNextFrame();
            //partA?.GetComponent<PhysicsBody>().ApplyLinearImpulse(Entity.Up * -farseerBody.Mass);
            //partB?.GetComponent<PhysicsBody>().ApplyLinearImpulse(Entity.Up * +farseerBody.Mass);
            Entity.Destroy();
        }

        private static void CopyVelocity(Body source, Body destination)
        {
            if (destination is null || source is null)
                return;
            
            destination.AngularDamping = source.AngularDamping;
            destination.AngularVelocity = source.AngularVelocity;
            destination.LinearVelocity = source.LinearVelocity;
        }

        private (PartialBrickRow, PartialBrickRow, int) GenerateRandomPartialRows()
        {
            var totalDetached = 0;

            var left = PartialBrickRow.None + random.Next(3);
            if (left != PartialBrickRow.None)
                totalDetached++;

            var right = PartialBrickRow.None + random.Next(3);
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