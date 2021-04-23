using System.Collections.Generic;
using GameProject.CoreEngine;
using GameProject.Ecs;
using GameProject.GameMath;

namespace GameProject.GameLogic.Levels
{
    internal class GameLevel1 : ISceneFactory
    {
        public SceneData CreateScene()
        {
            var ground = LevelUtility
                .CreateRectangle(Vector2F.Zero, Vector2F.One.WithX(50f), true);

            return new SceneData {Entities = new List<GameEntity> {ground}, Width = 50f, Offset = -25f};
        }
    }
}