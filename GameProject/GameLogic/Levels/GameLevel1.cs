using System.Collections.Generic;
using System.Drawing;
using GameProject.CoreEngine;
using GameProject.Ecs;
using GameProject.Ecs.Graphics;
using GameProject.GameGraphics.RenderShapes;
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

            ground.AddComponent<PlayerControl>();

            var editor = new GameEntity();
            var winRect = new RectangleF(8, -8, 4, 4);
            editor.AddComponent(new MachineEditor(5, 7) {WinRect = winRect});
            editor.Position = new Vector2F(-3, -6.25f);
            entities.Add(editor);

            var winZone = new GameEntity {Position = winRect.GetCenter(), Scale = winRect.GetSize()};
            winZone.AddComponent(new Sprite(new QuadRenderShape(0) {Color = Color.Aqua}));
            entities.Add(winZone);

            CreateWall(entities, 0);
            // CreateWall(entities, 3);

            return new SceneData {Entities = entities, Camera = new Camera {Position = new Vector2F(-3, -6)}};
        }

        private static void CreateWall(ICollection<GameEntity> entities, float positionX)
        {
            var wall = new GameEntity();
            const int rows = 20;
            wall.AddComponent(new BrickWall(rows, 1.5f));
            wall.Position = Vector2F.UnitX * positionX - Vector2F.UnitY * (rows * 0.5f * LevelUtility.BrickSize.Z + 5f);

            entities.Add(wall);
        }
    }
}