using System.Collections.Generic;
using GameProject.CoreEngine;
using GameProject.Ecs;
using GameProject.GameMath;

namespace GameProject.GameLogic.Scripts.MachineParts
{
    internal class SmallWheelFactory : IMachinePartFactory
    {
        public string TexturePath => "Resources/machine_parts/small_wheel.png";
        public bool HasBoxCollision => false;

        private readonly List<GameEntity> cleanupList = new List<GameEntity>();

        public void CreatePart(Vector2F cellPosition, GameState gameState, GameEntity machine)
        {
            var wheel = LevelUtility.CreateWheel(cellPosition.TransformBy(machine.GlobalTransform),
                0.4f * MachineEditor.CellSize, TexturePath);
            cleanupList.Add(wheel);
            wheel.AddComponent(new WheelControl {Machine = machine, Torque = 100f});
            gameState.AddEntity(wheel);
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