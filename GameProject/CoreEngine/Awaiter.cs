namespace GameProject.CoreEngine
{
    internal class Awaiter
    {
        public float Seconds { get; private set; }
        
        public int Frames { get; private set; }

        public bool Completed => Seconds <= 0 && Frames <= 0;
        
        public bool IsLast { get; set; }
        
        public Awaiter(int frames)
        {
            Frames = frames;
        }
        
        public Awaiter(float seconds)
        {
            Seconds = seconds;
        }

        public void Update(float deltaTime)
        {
            Seconds -= deltaTime;
            --Frames;
        }
    }
}