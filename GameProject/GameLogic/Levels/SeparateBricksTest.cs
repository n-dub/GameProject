using System.Collections.Generic;
using GameProject.CoreEngine;
using GameProject.Ecs;
using GameProject.GameMath;

namespace GameProject.GameLogic.Levels
{
    internal class SeparateBricksTest : ISceneFactory
    {
        public SceneData CreateScene()
        {
            const float width = 7f;

            var entities = new List<GameEntity>
            {
                LevelUtility.CreatePhysicalRectangle(Vector2F.Zero, Vector2F.One.WithX(width), true)
            };

            var position = new Vector2F(0, LevelUtility.BrickSize.Z / 2 - 0.5f);
            for (var i = 0; i < 16; i++)
            {
                position -= Vector2F.UnitY * LevelUtility.BrickSize.Z;
                entities.Add(LevelUtility.CreateBrick(position.WithX(-LevelUtility.BrickSize.X / 2), false));
                entities.Add(LevelUtility.CreateBrick(position.WithX(+LevelUtility.BrickSize.X / 2), false));

                position -= Vector2F.UnitY * LevelUtility.BrickSize.Z;
                entities.Add(LevelUtility.CreateBrick(position, false));
                entities.Add(LevelUtility.CreateBrick(position.WithX(-LevelUtility.BrickSize.X / 2), true));
                entities.Add(LevelUtility.CreateBrick(position.WithX(+LevelUtility.BrickSize.X / 2), true));
            }

            return new SceneData
            {
                Entities = entities
            };
        }
    }
}