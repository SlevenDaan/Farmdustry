using Engine.InputHandler;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Engine.Graphics.UI
{
    public class UILayer
    {
        private readonly List<UIElement> uiElements = new List<UIElement>();
        private readonly List<char> typedCharacters = new List<char>();

        public UILayer(GameWindow gameWindow)
        {
            gameWindow.TextInput += TextInput;
        }

        private void TextInput(object sender, TextInputEventArgs e)
        {
            typedCharacters.Add(e.Character);
        }

        /// <summary>
        /// Add a UIElement.
        /// </summary>
        /// <param name="uiElement">The UIElement to add.</param>
        /// <returns>If the UIElement was added.</returns>
        public bool Add(UIElement uiElement)
        {
            if (uiElements.Contains(uiElement))
            {
                return false;
            }

            uiElements.Add(uiElement);
            return true;
        }

        /// <summary>
        /// Remove a UIElement.
        /// </summary>
        /// <param name="uiElement">The UIElement to remove.</param>
        /// <returns>If the UIElement was removed.</returns>
        public bool Remove(UIElement uiElement)
        {
            if (uiElements.Contains(uiElement))
            {
                return false;
            }

            uiElements.Remove(uiElement);
            return true;
        }

        /// <summary>
        /// Draw all visible UIElements.
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatch used to draw.</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (UIElement uiElement in uiElements)
            {
                if (uiElement.State == UIElementState.Hidden)
                {
                    continue;
                }

                uiElement.Draw(spriteBatch);
            }
        }

        /// <summary>
        /// Update all InteractableUIElements.
        /// </summary>
        /// <param name="mouse">The SpriteBatch used to draw.</param>
        public void Update(MouseHandler mouse)
        {
            foreach (UIElement uiElement in uiElements)
            {
                if (typeof(InteractableUIElement).IsAssignableFrom(uiElement.GetType()))
                {
                    ((InteractableUIElement)uiElement).Update(mouse);
                }

                if(uiElement is Textbox)
                {
                    ((Textbox)uiElement).InputText(typedCharacters);
                }
            }

            typedCharacters.Clear();
        }
    }
}
