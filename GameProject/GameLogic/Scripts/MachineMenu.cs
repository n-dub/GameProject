using GameProject.CoreEngine;
using GameProject.GameLogic.Scripts.MachineParts;

namespace GameProject.GameLogic.Scripts
{
    internal class MachineMenu : GameScript
    {
        private static IMachinePartFactory[] factories = {new WoodenBoxFactory()};
        
        private MachineMenuResponse Response { get; }
        
        public MachineMenu(MachineMenuResponse response)
        {
            Response = response;
        }
    }
}