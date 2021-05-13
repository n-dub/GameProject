using System.Collections.Generic;
using GameProject.CoreEngine;
using GameProject.Ecs;
using GameProject.GameLogic.Scripts;
using GameProject.GameMath;

namespace GameProject.GameLogic.Levels
{
    internal class GameLevel1 : ISceneFactory
    {
        public SceneData CreateScene()
        {
            var ground = LevelUtility
                .CreatePhysicalRectangle(Vector2F.Zero, new Vector2F(50, 10), true);

            var entities = new List<GameEntity>
            {
                ground,
                LevelUtility.CreateBackground(new Vector2F(4592, 3056) / 1500,
                    Vector2F.UnitY * -0.5f, "Resources/background/bg0.png"),
                LevelUtility.CreateNoiseBackground()
            };

            var editor = new GameEntity();
            editor.AddComponent(new MachineEditor(5, 7));
            editor.Position = new Vector2F(-3, -6.25f);
            entities.Add(editor);
            
            CreateWall(entities, 0);
            CreateWall(entities, 3);
            
            return new SceneData {Entities = entities, Camera = new Camera{Position = new Vector2F(-3, -6)}};
        }
        
        private static void CreateWall(ICollection<GameEntity> entities, float positionX)
        {
            var wall = new GameEntity();
            wall.AddComponent(new BrickWall(25, 0.5f));
            wall.Position =
                Vector2F.UnitX * positionX - Vector2F.UnitY * (12.5f * LevelUtility.BrickSize.Z + 5f);

            entities.Add(wall);
        }
    }
}