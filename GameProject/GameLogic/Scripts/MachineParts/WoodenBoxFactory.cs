using GameProject.CoreEngine;
using GameProject.Ecs;
using GameProject.GameMath;

namespace GameProject.GameLogic.Scripts.MachineParts
{
    internal class WoodenBoxFactory : IMachinePartFactory
    {
        public string TexturePath { get; } = "Resources/machine_parts/wooden_box.png";
        
        public void CreatePart(Vector2F cellPosition, GameState gameState, GameEntity machine)
        {
            throw new System.NotImplementedException();
        }
    }
}