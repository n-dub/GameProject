using System.Collections.Generic;
using GameProject.CoreEngine;
using GameProject.Ecs.Graphics;
using GameProject.Ecs.Physics;
using GameProject.GameGraphics.RenderShapes;
using GameProject.GameMath;

namespace GameProject.GameLogic.Scripts
{
    internal class BrickWall : GameScript
    {
        public int RowCount { get; }
        
        public PartialBrickRow BottomPartialBrickRow { get; }

        public PartialBrickRow TopPartialBrickRow { get; }
        
        public bool StartWithTwoBricks { get; }

        private const string DetachedBrickPath = "Resources/bricks/detached_brick.png";
        
        public BrickWall(int rowCount,
            bool startWithTwoBricks = true,
            PartialBrickRow bottomPartialBrickRow = PartialBrickRow.None,
            PartialBrickRow topPartialBrickRow = PartialBrickRow.None)
        {
            RowCount = rowCount;
            BottomPartialBrickRow = bottomPartialBrickRow;
            TopPartialBrickRow = topPartialBrickRow;
            StartWithTwoBricks = startWithTwoBricks;
        }

        protected override void Initialize()
        {
            var body = Entity.AddComponent<PhysicsBody>();
            body.AddCollider(new BoxCollider
            {
                SizeX = LevelUtility.BrickSize.X * 2,
                SizeY = LevelUtility.BrickSize.Z * RowCount
            });
            
            Entity.AddComponent<Sprite>();
            
            StartCoroutine(CreateSprites);
        }

        private IEnumerable<Awaiter> CreateSprites()
        {
            yield return new Awaiter(1);
            
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