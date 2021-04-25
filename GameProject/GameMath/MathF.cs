using System;
using System.Runtime.CompilerServices;

namespace GameProject.GameMath
{
    /// <summary>
    ///     An adapter for <see cref="Math" /> that uses <see cref="float" /> instead of <see cref="double" />
    /// </summary>
    public static class MathF
    {
        /// <summary>
        ///     Acceleration of gravity (in meters/second^2)
        /// </summary>
        public const float Gravity = 9.806f;

        /// <inheritdoc cref="Math.PI" />
        // ReSharper disable once InconsistentNaming
        public const float PI = (float) Math.PI;

        /// <inheritdoc cref="Math.Sqrt" />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Sqrt(float value)
        {
            return (float) Math.Sqrt(value);
        }

        /// <inheritdoc cref="Math.Sin" />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Sin(float value)
        {
            return (float) Math.Sin(value);
        }

        /// <inheritdoc cref="Math.Cos" />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Cos(float value)
        {
            return (float) Math.Cos(value);
        }

        /// <inheritdoc cref="Math.Abs(float)" />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Abs(float value)
        {
            return Math.Abs(value);
        }
    }
}