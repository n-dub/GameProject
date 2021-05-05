using System.Collections.Generic;
using GameProject.CoreEngine;
using GameProject.Ecs;
using GameProject.GameLogic.Scripts;
using GameProject.GameMath;

namespace GameProject.GameLogic.Levels
{
    internal class BrickWallTest : ISceneFactory
    {
        public SceneData CreateScene()
        {
            const float width = 7f;
            
            var entities = new List<GameEntity>
            {
                LevelUtility.CreateRectangle(Vector2F.Zero, Vector2F.One.WithX(width), true),
            };

            var wall = new GameEntity();
            wall.AddComponent(new BrickWall(25));
            wall.Position = -Vector2F.UnitY * (12.5f * LevelUtility.BrickSize.Z + 0.5f);
            
            entities.Add(wall);
            
            return new SceneData
            {
                Entities = entities,
                Width = width,
                Offset = width / -2
            };
        }
    }
}