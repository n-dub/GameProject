using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Windows.Forms;

namespace GameProject.GameInput
{
    public class Keyboard
    {
        public KeyState this[Keys key] =>
            States.TryGetValue(key, out var state)
                ? state
                : KeyState.None;

        private ConcurrentDictionary<Keys, KeyState> States { get; }

        public Keyboard()
        {
            States = new ConcurrentDictionary<Keys, KeyState>(Environment.ProcessorCount, 256);
        }

        public void PushKey(Keys key) => States[key] = KeyState.Down;

        public void ReleaseKey(Keys key) => States[key] = KeyState.Up;

        public void UpdateKeyStates()
        {
            foreach (var (key, state) in States.Select(p => (p.Key, p.Value)))
            {
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
}