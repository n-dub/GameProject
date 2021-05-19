using GameProject.CoreEngine;
using GameProject.Ecs;
using GameProject.GameMath;

namespace GameProject.GameLogic.Scripts.MachineParts
{
    internal class BombBoxFactory : IMachinePartFactory
    {
        public string TexturePath => "Resources/machine_parts/bomb_box.png";
        public bool HasBoxCollision => false;

        public void CreatePart(Vector2F cellPosition, GameState gameState, GameEntity machine)
        {
            var bomb = LevelUtility.CreateWheel(cellPosition + machine.Position,
                0.4f * MachineEditor.CellSize, TexturePath);
            gameState.AddEntity(bomb);
            bomb.AddComponent(new WheelControl {Torque = 0, Machine = machine});
            bomb.AddComponent<Explosive>();
        }
    }
}