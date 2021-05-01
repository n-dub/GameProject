using GameProject.Ecs;
using GameProject.Ecs.Graphics;
using GameProject.Ecs.Physics;
using GameProject.GameGraphics.RenderShapes;
using GameProject.GameMath;

namespace GameProject.GameLogic
{
    internal static class LevelUtility
    {
        public static GameEntity CreateCircle(Vector2F position, float radius, bool isStatic = false)
        {
            var entity = new GameEntity();
            entity.AddComponent(new Sprite(new CircleRenderShape(1)));
            var body = entity.AddComponent<PhysicsBody>();
            body.IsStatic = isStatic;
            body.AddCollider(new CircleCollider{Radius = radius});
            entity.Position = position;
            entity.Scale = new Vector2F(radius) * 2;
            return entity;
        }

        public static GameEntity CreateRectangle(Vector2F position, Vector2F size, bool isStatic = false)
        {
            var entity = new GameEntity();
            entity.AddComponent(new Sprite(new QuadRenderShape(1)));
            var body = entity.AddComponent<PhysicsBody>();
            body.IsStatic = isStatic;
            body.AddCollider(new BoxCollider());
            entity.Position = position;
            entity.Scale = size;
            return entity;
        }

        public static GameEntity CreatePolygon(Vector2F position, bool isStatic, params Vector2F[] vertices)
        {
            var entity = new GameEntity();
            entity.AddComponent(new Sprite(new PolygonRenderShape(1) {Points = vertices}));
            var body = entity.AddComponent<PhysicsBody>();
            body.IsStatic = isStatic;
            body.AddCollider(new PolygonCollider{Vertices = vertices});
            entity.Position = position;
            return entity;
        }
    }
}