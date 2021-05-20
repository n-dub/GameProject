using GameProject.CoreEngine;
using GameProject.Ecs;
using GameProject.Ecs.Graphics;
using GameProject.Ecs.Physics;
using GameProject.GameGraphics.RenderShapes;
using GameProject.GameMath;

namespace GameProject.GameLogic.Scripts.MachineParts
{
    internal class WoodenPipeFactory : IMachinePartFactory
    {
        public string TexturePath => "Resources/machine_parts/wooden_pipe.png";
        public bool HasBoxCollision => false;

        private const float Width = 0.1f;

        public void CreatePart(Vector2F cellPosition, GameState gameState, GameEntity machine)
        {
            var sprite = machine.GetComponent<Sprite>();
            var body = machine.GetComponent<PhysicsBody>();
            sprite.AddShape(gameState, new QuadRenderShape(1)
            {
                ImagePath = TexturePath,
                Offset = cellPosition,
                Scale = Vector2F.One * MachineEditor.CellSize
            });
            const float colliderOffset = 0.5f - Width + Width * 0.5f;
            
            body.AddCollider(new BoxCollider
            {
                Offset = cellPosition + colliderOffset * MachineEditor.CellSize * Vector2F.UnitY,
                Scaled = false,
                Size = new Vector2F(1, Width) * MachineEditor.CellSize
            });
            body.AddCollider(new BoxCollider
            {
                Offset = cellPosition - colliderOffset * MachineEditor.CellSize * Vector2F.UnitY,
                Scaled = false,
                Size = new Vector2F(1, Width) * MachineEditor.CellSize
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