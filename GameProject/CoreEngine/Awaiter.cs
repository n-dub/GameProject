namespace GameProject.CoreEngine
{
    internal sealed class Awaiter
    {
        public bool Completed => Seconds <= 0 && Frames <= 0;
        public float Seconds { get; private set; }

        public int Frames { get; private set; }

        public bool TimeScaled { get; }

        public bool IsLast { get; set; }

        private Awaiter(int frames)
        {
            Frames = frames;
        }

        private Awaiter(float seconds, bool timeScaled)
        {
            Seconds = seconds;
            TimeScaled = timeScaled;
        }

        public static Awaiter WaitForNextFrame()
        {
            return new Awaiter(1);
        }

        public static Awaiter WaitForFrames(int count)
        {
            return new Awaiter(count);
        }

        public static Awaiter WaitForSeconds(float seconds, bool timeScaled = true)
        {
            return new Awaiter(seconds, timeScaled);
        }

        public void Update(float deltaTime, float deltaTimeUnscaled)
        {
            Seconds -= TimeScaled ? deltaTime : deltaTimeUnscaled;
            --Frames;
        }
    }
}