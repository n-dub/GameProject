using GameProject.CoreEngine;
using GameProject.Ecs;
using GameProject.GameMath;

namespace GameProject.GameLogic.Scripts.MachineParts
{
    internal class SmallWheelFactory : IMachinePartFactory
    {
        public string TexturePath => "Resources/machine_parts/small_wheel.png";
        public bool HasBoxCollision => false;

        public void CreatePart(Vector2F cellPosition, GameState gameState, GameEntity machine)
        {
            var wheel = LevelUtility.CreateWheel(cellPosition + machine.Position,
                0.4f * MachineEditor.CellSize, TexturePath);
            wheel.AddComponent(new WheelControl {Machine = machine, Torque = 0.5f});
            gameState.AddEntity(wheel);
        }
    }
}