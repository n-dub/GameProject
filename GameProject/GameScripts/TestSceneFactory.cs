using System.Collections.Generic;
using System.Linq;
using FarseerPhysics.Collision;
using FarseerPhysics.Dynamics;
using GameProject.CoreEngine;
using GameProject.Ecs;
using GameProject.Ecs.Graphics;
using GameProject.Ecs.Physics;
using GameProject.GameGraphics;
using GameProject.GameMath;

namespace GameProject.GameScripts
{
    internal class TestSceneFactory : ISceneFactory
    {
        private readonly List<GameEntity> physicsTestScene = new List<GameEntity>
        {
            CreateRectangle(new Vector2F(), new Vector2F(7, 1), true),
            CreateRectangle(new Vector2F(0, -3), new Vector2F(5, 1)),
            CreateCircle(new Vector2F(0.5f, -2), 0.5f)
        };
        
        public List<GameEntity> CreateScene()
        {
            return physicsTestScene;
        }

        private static GameEntity CreateCircle(Vector2F position, float radius)
        {
            var entity = new GameEntity();
            entity.AddComponent(new Sprite(new QuadRenderShape(1)));
            entity.AddComponent<PhysicsBody>();
            entity.AddComponent<CircleCollider>().Radius = radius;
            entity.Position = position;
            entity.Scale = new Vector2F(radius * 2, radius * 2);
            return entity;
        }

        private static GameEntity CreateRectangle(Vector2F position, Vector2F size, bool isStatic = false)
        {
            var entity = new GameEntity();
            entity.AddComponent(new Sprite(new QuadRenderShape(1)));
            entity.AddComponent<PhysicsBody>().IsStatic = isStatic;
            entity.AddComponent<BoxCollider>();
            entity.Position = position;
            entity.Scale = size;
            return entity;
        }
    }
}