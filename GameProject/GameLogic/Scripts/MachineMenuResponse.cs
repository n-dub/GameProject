namespace GameProject.GameLogic.Scripts
{
    internal class MachineMenuResponse
    {
        public IMachinePartFactory Factory { get; set; }

        private bool IsComplete => Factory != null;
        
        public bool TryGetResult(out IMachinePartFactory factory)
        {
            factory = Factory;
            return IsComplete;
        }
    }
}