using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Engine.Graphics.UI
{
    public class Button : InteractableUIElement
    {
        private Rectangle CollisionBox => new Rectangle(X, Y, Width, Height);

        /// <summary>
        /// The texture that is used for the button.
        /// </summary>
        public Texture2D Texture { get; set; }

        /// <summary>
        /// Width of the button.
        /// </summary>
        public int Width { get; set; }
        /// <summary>
        /// Height of the button.
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// True if the mouse is hovering over the button.
        /// </summary>
        public bool IsHovered { get; private set; }
        /// <summary>
        /// The color filter used when the mouse is hovering over the button.
        /// </summary>
        public Color HoverColor { get; set; } = Color.White;

        /// <summary>
        /// Called when the left mouse button is pressed on the button.
        /// </summary>
        public event Action Clicked;

        public Button(int y, int x, Texture2D texture, int width, int height, Color hoverColor) : base(y, x)
        {
            Texture = texture;
            Width = width;
            Height = height;
            HoverColor = hoverColor;
        }

        public override void Update()
        {
            CheckIfLeftMouseButtonClick();
        }

        private bool previousUpdateLeftClickPressed = false;
        /// <summary>
        /// Check if the button is pressed and triggers the clicked event.
        /// </summary>
        private void CheckIfLeftMouseButtonClick()
        {
            MouseState mouseState = Mouse.GetState();

            IsHovered = CollisionBox.Contains(mouseState.Position);

            bool currentUpdateLeftClickPressed = IsHovered && mouseState.LeftButton==ButtonState.Pressed;
            if (currentUpdateLeftClickPressed && !previousUpdateLeftClickPressed)
            {
                Clicked?.Invoke();
            }
            previousUpdateLeftClickPressed = currentUpdateLeftClickPressed;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (IsHovered)
            {
                spriteBatch.Draw(Texture, CollisionBox, HoverColor);
            }
            else
            {
                spriteBatch.Draw(Texture, CollisionBox, Color.White);
            }
        }
    }
}
