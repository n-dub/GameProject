using System.Collections.Generic;
using GameProject.CoreEngine;
using GameProject.Ecs;
using GameProject.GameLogic.Scripts;
using GameProject.GameMath;

namespace GameProject.GameLogic.Levels
{
    internal class WheelTest : ISceneFactory
    {
        private const string Path = "Resources/machine_parts/small_wheel.png";

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

            var machine = LevelUtility.CreatePhysicalRectangle(Vector2F.UnitY * -1.2f, new Vector2F(2.5f, 0.3f));
            entities.Add(machine);

            CreateWheel(entities, machine, -0.8f);
            CreateWheel(entities, machine, +0.8f);

            CreateWall(entities, 2);

            return new SceneData
            {
                Entities = entities
            };
        }

        private static void CreateWheel(ICollection<GameEntity> entities, GameEntity machine, float positionX)
        {
            var wheel = LevelUtility.CreateWheel(new Vector2F(positionX, -1), 0.2f, Path);
            wheel.AddComponent(new WheelControl {Torque = 0.005f, Machine = machine});
            entities.Add(wheel);
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