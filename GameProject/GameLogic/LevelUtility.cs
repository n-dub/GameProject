using System.Linq;
using System.Runtime.CompilerServices;
using GameProject.CoreEngine;
using GameProject.Ecs;
using GameProject.Ecs.Graphics;
using GameProject.Ecs.Physics;
using GameProject.GameGraphics.RenderShapes;
using GameProject.GameMath;

namespace GameProject.GameLogic
{
    internal static class LevelUtility
    {
        public static readonly Vector3F BrickSize = new Vector3F(0.25f, 0.12f, 0.065f);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GameEntity CreateCircle(Vector2F position, float radius, bool isStatic = false)
        {
            var entity = new GameEntity();
            entity.AddComponent(new Sprite(new CircleRenderShape(1)));
            var body = entity.AddComponent<PhysicsBody>();
            body.IsStatic = isStatic;
            body.AddCollider(new CircleCollider{Radius = radius});
            entity.Position = position;
            entity.Scale = new Vector2F(radius) * 2;
            entity.FlushComponents();
            return entity;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GameEntity CreateBrick(Vector2F position, bool width120, float rotation = 0)
        {
            var brickSize = new Vector2F(width120 ? BrickSize.Y : BrickSize.X, BrickSize.Z);
            var rect = CreatePhysicalRectangle(position, brickSize);
            rect.Rotation = rotation;
            if (rect.GetComponent<Sprite>().Shapes.First() is QuadRenderShape shape)
                shape.ImagePath = "Resources/bricks/detached_brick.png";
            rect.FlushComponents();
            return rect;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GameEntity CreatePhysicalRectangle(Vector2F position, Vector2F size, bool isStatic = false)
        {
            var entity = new GameEntity();
            entity.AddComponent(new Sprite(new QuadRenderShape(1)));
            var body = entity.AddComponent<PhysicsBody>();
            body.IsStatic = isStatic;
            body.AddCollider(new BoxCollider());
            entity.Position = position;
            entity.Scale = size;
            entity.FlushComponents();
            return entity;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GameEntity CreateBackground(Vector2F size, Vector2F position, string imagePath)
        {
            var entity = new GameEntity();
            var sprite = new Sprite(new QuadRenderShape(-1){ImagePath = imagePath}, true);
            entity.AddComponent(sprite);
            entity.Position = position;
            entity.Scale = size;
            entity.FlushComponents();
            return entity;
        }

        public static GameEntity CreateWheel(Vector2F position, float radius, string imagePath)
        {
            var entity = new GameEntity();
            entity.AddComponent(new Sprite(new QuadRenderShape(1)
            {
                ImagePath = imagePath
            }));
            var body = entity.AddComponent<PhysicsBody>();
            body.AddCollider(new CircleCollider{Radius = radius});
            entity.Position = position;
            entity.Scale = new Vector2F(radius) * 2;
            entity.FlushComponents();
            return entity;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GameEntity CreatePolygon(Vector2F position, bool isStatic, params Vector2F[] vertices)
        {
            var entity = new GameEntity();
            entity.AddComponent(new Sprite(new PolygonRenderShape(1)
            {
                Points = vertices
            }));
            var body = entity.AddComponent<PhysicsBody>();
            body.IsStatic = isStatic;
            body.AddCollider(new PolygonCollider{Vertices = vertices});
            entity.Position = position;
            entity.FlushComponents();
            return entity;
        }
    }
}