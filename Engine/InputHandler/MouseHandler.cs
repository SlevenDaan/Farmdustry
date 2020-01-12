using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.InputHandler
{
    public class MouseHandler
    {
        private bool[] previousPressedMouseButtons = new bool[5] { false, false, false, false, false };
        private bool[] currentPressedMouseButtons = new bool[5] { false, false, false, false, false };

        public Point Position { get; private set; }

        public void Update()
        {
            MouseState mouse = Mouse.GetState();

            Position = mouse.Position;

            previousPressedMouseButtons = currentPressedMouseButtons;
            currentPressedMouseButtons = new bool[5] {
                mouse.LeftButton == ButtonState.Pressed,
                mouse.RightButton == ButtonState.Pressed,
                mouse.MiddleButton == ButtonState.Pressed,
                mouse.XButton1 == ButtonState.Pressed,
                mouse.XButton2 == ButtonState.Pressed
            };
        }

        /// <summary>
        /// Get if a mouse button has been pressed down.
        /// </summary>
        /// <param name="mouseButton">The mouse button to check.</param>
        /// <returns>If the mouse button is pressed down.</returns>
        public bool IsPressed(MouseButtonType mouseButton)
        {
            return !previousPressedMouseButtons[(int)mouseButton] && currentPressedMouseButtons[(int)mouseButton];
        }
        /// <summary>
        /// Get if a mouse button has been held down.
        /// </summary>
        /// <param name="mouseButton">The mouse button to check.</param>
        /// <returns>If the mouse button is held down.</returns>
        public bool IsHeld(MouseButtonType mouseButton)
        {
            return currentPressedMouseButtons[(int)mouseButton];
        }
        /// <summary>
        /// Get if a mouse button has been released.
        /// </summary>
        /// <param name="mouseButton">The mouse button to check.</param>
        /// <returns>If the mouse button is released.</returns>
        public bool IsReleased(MouseButtonType mouseButton)
        {
            return previousPressedMouseButtons[(int)mouseButton] && !currentPressedMouseButtons[(int)mouseButton];
        }
    }
}
