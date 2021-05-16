using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Runtime.CompilerServices;

namespace GameProject.GameInput
{
    /// <summary>
    ///     Represents current state of keyboard input
    /// </summary>
    public class Keyboard<TKey> where TKey : Enum
    {
        /// <summary>
        ///     Get state of a certain key
        /// </summary>
        /// <param name="key">Key to get state of</param>
        public KeyState this[TKey key]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => States.TryGetValue(key, out var state)
                ? state
                : KeyState.None;
        }

        private ConcurrentDictionary<TKey, KeyState> States { get; }

        /// <summary>
        ///     Create a new keyboard
        /// </summary>
        public Keyboard()
        {
            States = new ConcurrentDictionary<TKey, KeyState>(Environment.ProcessorCount, 256);
        }

        /// <summary>
        ///     Update state of key being pushed at this frame
        /// </summary>
        /// <param name="key">The key that have been pushed</param>
        public void PushKey(TKey key)
        {
            States[key] = this[key] == KeyState.Pushing
                ? KeyState.Pushing
                : KeyState.Down;
        }

        /// <summary>
        ///     Update state of key being released at this frame
        /// </summary>
        /// <param name="key">The key that have been released</param>
        public void ReleaseKey(TKey key)
        {
            States[key] = this[key] == KeyState.None
                ? KeyState.None
                : KeyState.Up;
        }

        /// <summary>
        ///     Update state of all keys, called every frame
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     Thrown if some keys have invalid values
        /// </exception>
        public virtual void UpdateKeyStates()
        {
            foreach (var (key, state) in States.Select(p => (p.Key, p.Value)))
                switch (state)
                {
                    case KeyState.Down:
                        States[key] = KeyState.Pushing;
                        break;
                    case KeyState.Up:
                        States[key] = KeyState.None;
                        break;
                    case KeyState.Pushing:
                    case KeyState.None:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
        }
    }
}