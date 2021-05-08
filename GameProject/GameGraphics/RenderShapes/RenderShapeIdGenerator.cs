using System.Threading;

namespace GameProject.GameGraphics.RenderShapes
{
    internal static class RenderShapeIdGenerator
    {
        private static int currentId;
        
        public static int GetId()
        {
            return Interlocked.Increment(ref currentId);
        }
    }
}