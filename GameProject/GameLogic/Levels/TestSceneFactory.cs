using System.Collections.Generic;
using GameProject.CoreEngine;
using GameProject.Ecs;
using GameProject.GameMath;

namespace GameProject.GameLogic.Levels
{
    internal class TestSceneFactory : ISceneFactory
    {
        public SceneData CreateScene()
        {
            return new SceneData
            {
                Entities = new List<GameEntity>
                {
                    LevelUtility.CreateRectangle(Vector2F.Zero, Vector2F.One.WithX(7), true),
                    LevelUtility.CreateRectangle(new Vector2F(0, -3), Vector2F.One.WithX(5)),
                    LevelUtility.CreateCircle(new Vector2F(0.5f, -2), 0.5f)
                },
                Width = 7f,
                Offset = -3.5f
            };
        }
    }
}