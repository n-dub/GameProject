using GameProject.CoreEngine;
using GameProject.Ecs;
using GameProject.Ecs.Graphics;
using GameProject.Ecs.Physics;
using GameProject.GameGraphics.RenderShapes;
using GameProject.GameMath;

namespace GameProject.GameLogic.Scripts.MachineParts
{
    internal class OpenWoodenPipeFactory : IMachinePartFactory
    {
        public string TexturePath => "Resources/machine_parts/wooden_pipe_open.png";
        public bool HasBoxCollision => false;
        public bool Connectible => true;

        public void CreatePart(Vector2F cellPosition, float rotation, GameState gameState, GameEntity machine)
        {
            var sprite = machine.GetComponent<Sprite>();
            var body = machine.GetComponent<PhysicsBody>();
            sprite.AddShape(gameState, new QuadRenderShape(1)
            {
                ImagePath = TexturePath,
                Offset = cellPosition,
                Scale = Vector2F.One * MachineEditor.CellSize,
                Rotation = rotation
            });
            const float colliderOffsetY = 0.5f - WoodenPipeFactory.Width * 0.5f;
            const float colliderOffsetX = 0.5f - BombFactory.Radius * 0.5f;

            var sizeBottom = new Vector2F(1, WoodenPipeFactory.Width).Rotate(rotation) * MachineEditor.CellSize;
            var sizeTop = new Vector2F(0.5f - BombFactory.Radius, WoodenPipeFactory.Width)
                .Rotate(rotation) * MachineEditor.CellSize;
            var offsetY = colliderOffsetY * MachineEditor.CellSize * Vector2F.UnitY.Rotate(rotation);
            var offsetX = colliderOffsetX * MachineEditor.CellSize * Vector2F.UnitX.Rotate(rotation);
            
            body.AddCollider(new BoxCollider
            {
                Offset = cellPosition - offsetY - offsetX,
                Scaled = false,
                Size = sizeTop
            });
            body.AddCollider(new BoxCollider
            {
                Offset = cellPosition - offsetY + offsetX,
                Scaled = false,
                Size = sizeTop
            });
            body.AddCollider(new BoxCollider
            {
                Offset = cellPosition + offsetY,
                Scaled = false,
                Size = sizeBottom
            });
        }

        public void CleanUp()
        {
        }

        public GameEntity CreateDestroyedPart(Vector2F position, float rotation)
        {
            var entity = new GameEntity {Position = position, Rotation = rotation};
            entity.AddComponent(new Explosive
            {
                ForceExplodeOnStart = true,
                ExplosionFragCount = 5,
                ExplosionRadius = 1,
                ExplosionFragImpulse = 0.01f,
                MinLifetime = 2f,
                MaxLifetime = 5f
            });
            entity.AddComponent<PhysicsBody>();
            return entity;
        }
    }
}