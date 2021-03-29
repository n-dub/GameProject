using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameProject.GameInput
{
    public class Keyboard
    {
        public KeyState this[Keys key] =>
            states.TryGetValue(key, out var state)
            ? state
            : KeyState.None;

        private readonly ConcurrentDictionary<Keys, KeyState> states;

        public Keyboard()
        {
            states = new ConcurrentDictionary<Keys, KeyState>(Environment.ProcessorCount, 256);
        }

        public void PushKey(Keys key) => states[key] = KeyState.Down;

        public void ReleaseKey(Keys key) => states[key] = KeyState.Up;

        public void UpdateKeyStates()
        {
            foreach (var keyState in states)
            {
                var (key, state) = (keyState.Key, keyState.Value);

                switch (state)
                {
                    case KeyState.Down:
                        states[key] = KeyState.Pushing;
                        break;
                    case KeyState.Up:
                        states[key] = KeyState.None;
                        break;
                }
            }
        }
    }
}
