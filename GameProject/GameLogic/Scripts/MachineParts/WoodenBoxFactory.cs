using GameProject.CoreEngine;
using GameProject.Ecs;
using GameProject.Ecs.Graphics;
using GameProject.Ecs.Physics;
using GameProject.GameGraphics.RenderShapes;
using GameProject.GameMath;

namespace GameProject.GameLogic.Scripts.MachineParts
{
    internal class WoodenBoxFactory : IMachinePartFactory
    {
        public string TexturePath => "Resources/machine_parts/wooden_box.png";
        public bool HasBoxCollision => true;
        public bool Connectible => true;

        public void CreatePart(Vector2F cellPosition, float rotation, GameState gameState, GameEntity machine)
        {
            var sprite = machine.GetComponent<Sprite>();
            sprite.AddShape(gameState, new QuadRenderShape(1)
            {
                ImagePath = TexturePath,
                Offset = cellPosition,
                Scale = Vector2F.One * MachineEditor.CellSize,
                Rotation = rotation
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
                ExplosionFragCount = 20,
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