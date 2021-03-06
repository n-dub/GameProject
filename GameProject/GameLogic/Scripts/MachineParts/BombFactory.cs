using System.Collections.Generic;
using System.Linq;
using GameProject.CoreEngine;
using GameProject.Ecs;
using GameProject.Ecs.Physics;
using GameProject.GameMath;

namespace GameProject.GameLogic.Scripts.MachineParts
{
    internal class BombFactory : IMachinePartFactory
    {
        public const float Radius = 0.3f * 0.8f;
        public string TexturePath => "Resources/machine_parts/bomb_box.png";
        public bool HasBoxCollision => false;
        public bool Connectible => false;

        private readonly List<GameEntity> cleanupList = new List<GameEntity>();
        private readonly List<Vector2F> destroyedCells = new List<Vector2F>();

        public void CreatePart(Vector2F cellPosition, float rotation, GameState gameState, GameEntity machine)
        {
            if (destroyedCells.Any(c => c == cellPosition))
                return;
            var bomb = LevelUtility.CreateWheel(cellPosition + machine.Position,
                Radius / 0.8f * MachineEditor.CellSize, TexturePath, 0.7f);
            bomb.GetComponent<PhysicsBody>().Friction = 0;
            cleanupList.Add(bomb);
            gameState.AddEntity(bomb);
            var explosive = new Explosive {Strength = 0.7f};
            explosive.OnExplode += () => destroyedCells.Add(cellPosition);
            bomb.AddComponent(explosive);
        }

        public void CleanUp()
        {
            foreach (var entity in cleanupList)
                entity.Destroy();
            cleanupList.Clear();
        }

        public GameEntity CreateDestroyedPart(Vector2F position, float rotation)
        {
            return null;
        }
    }
}