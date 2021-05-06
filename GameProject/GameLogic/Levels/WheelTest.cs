using System.Collections.Generic;
using GameProject.CoreEngine;
using GameProject.Ecs;
using GameProject.GameLogic.Scripts;
using GameProject.GameMath;

namespace GameProject.GameLogic.Levels
{
    internal class WheelTest : ISceneFactory
    {
        public SceneData CreateScene()
        {
            const float width = 15f;
            const float height = 10f;
            const string path = "Resources/machine_parts/small_wheel.png";

            var entities = new List<GameEntity>
            {
                LevelUtility.CreateRectangle(Vector2F.Zero, Vector2F.One.WithX(width), true),
                LevelUtility.CreateRectangle(new Vector2F(width / 2, height / -2),
                    Vector2F.One.WithY(height), true)
            };

            var wheel = LevelUtility.CreateWheel(new Vector2F(0, -1), 0.2f, path);
            wheel.AddComponent<WheelControl>().Torque = 0.01f;

            entities.Add(wheel);
            
            CreateWall(entities, 2);
            
            return new SceneData
            {
                Entities = entities,
                Width = width,
                Offset = width / -2
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