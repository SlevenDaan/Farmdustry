using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Engine.InputHandler
{
    public class KeyboardHandler
    {
        private HashSet<Keys> previousPressedKeys = new HashSet<Keys>();
        private HashSet<Keys> currentPressedKeys = new HashSet<Keys>();

        public void Update()
        {
            previousPressedKeys = currentPressedKeys;
            currentPressedKeys = new HashSet<Keys>(Keyboard.GetState().GetPressedKeys());
        }

        /// <summary>
        /// Get if a key has been pressed down.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>If the key is pressed down.</returns>
        public bool IsPressed(Keys key)
        {
            return !previousPressedKeys.Contains(key) && currentPressedKeys.Contains(key);
        }
        /// <summary>
        /// Get if a key has been held down.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>If the key is held down.</returns>
        public bool IsHeld(Keys key)
        {
            return currentPressedKeys.Contains(key);
        }
        /// <summary>
        /// Get if a key has been released.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>If the key is released.</returns>
        public bool IsReleased(Keys key)
        {
            return previousPressedKeys.Contains(key) && !currentPressedKeys.Contains(key);
        }
    }
}
