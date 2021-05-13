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
        
        /// <inheritdoc cref="Math.Acos" />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Acos(float value)
        {
            return (float) Math.Acos(value);
        }

        /// <inheritdoc cref="Math.Abs(float)" />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Abs(float value)
        {
            return Math.Abs(value);
        }

        /// <inheritdoc cref="Math.Round(double)" />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Round(float value)
        {
            return (float) Math.Round(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Clamp(float value, float min, float max)
        {
            return Math.Max(min, Math.Min(max, value));
        }

        /// <inheritdoc cref="Math.Floor(double)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Floor(float value)
        {
            return (float) Math.Floor(value);
        }
        
        /// <inheritdoc cref="Math.Ceiling(double)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Ceiling(float value)
        {
            return (float) Math.Ceiling(value);
        }
    }
}