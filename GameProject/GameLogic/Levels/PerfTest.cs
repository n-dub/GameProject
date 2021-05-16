using System.Collections.Generic;
using GameProject.CoreEngine;
using GameProject.Ecs;
using GameProject.GameLogic.Scripts;
using GameProject.GameMath;

namespace GameProject.GameLogic.Levels
{
    internal class PerfTest : ISceneFactory
    {
        public SceneData CreateScene()
        {
            const float width = 15f;
            const float height = 10f;

            var entities = new List<GameEntity>
            {
                LevelUtility.CreatePhysicalRectangle(Vector2F.Zero, Vector2F.One.WithX(width), true),
                LevelUtility.CreatePhysicalRectangle(new Vector2F(width / 2, height / -2),
                    Vector2F.One.WithY(height), true)
            };

            for (var i = -2; i <= 2; i++)
                CreateWall(entities, i * LevelUtility.BrickSize.X * 2.1f);

            return new SceneData
            {
                Entities = entities
            };
        }

        private static void CreateWall(ICollection<GameEntity> entities, float positionX)
        {
            var wall = new GameEntity();
            wall.AddComponent(new BrickWall(25, 0.2f));
            wall.Position =
                Vector2F.UnitX * positionX - Vector2F.UnitY * (12.5f * LevelUtility.BrickSize.Z + 0.5f);

            entities.Add(wall);
        }
    }
}