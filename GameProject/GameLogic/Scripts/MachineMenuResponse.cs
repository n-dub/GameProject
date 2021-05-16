namespace GameProject.GameLogic.Scripts
{
    internal class MachineMenuResponse
    {
        public IMachinePartFactory Factory { get; set; }

        public bool IsComplete { get; set; }

        public bool TryGetResult(out IMachinePartFactory factory)
        {
            factory = Factory;
            return IsComplete;
        }
    }
}