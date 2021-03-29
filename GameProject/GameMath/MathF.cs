using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameProject.GameMath
{
    public static class MathF
    {
        public const float PI = (float)Math.PI;

        public static float Sqrt(float value) => (float)Math.Sqrt(value);

        public static float Sin(float value) => (float)Math.Sin(value);

        public static float Cos(float value) => (float)Math.Cos(value);

        public static float Abs(float value) => Math.Abs(value);
    }
}
