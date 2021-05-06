namespace GameProject.CoreEngine
{
    internal class Awaiter
    {
        public float Seconds { get; private set; }
        
        public int Frames { get; private set; }

        public bool Completed => Seconds <= 0 && Frames <= 0;
        
        public bool IsLast { get; set; }
        
        private Awaiter(int frames)
        {
            Frames = frames;
        }
        
        private Awaiter(float seconds)
        {
            Seconds = seconds;
        }

        public static Awaiter WaitForNextFrame()
        {
            return new Awaiter(1);
        }

        public static Awaiter WaitForFrames(int count)
        {
            return new Awaiter(count);
        }

        public static Awaiter WaitForSeconds(float seconds)
        {
            return new Awaiter(seconds);
        }

        public void Update(float deltaTime)
        {
            Seconds -= deltaTime;
            --Frames;
        }
    }
}