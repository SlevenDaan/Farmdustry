using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Engine.Graphics.UI
{
    public class UILayer
    {
        public List<UIElement> UIElements { get; private set; } = new List<UIElement>();

        public UILayer(params UIElement[] uiElements)
        {
            UIElements.AddRange(uiElements);
        }

        /// <summary>
        /// Updates the logic of all InteractableUIElements.
        /// </summary>
        public void Update()
        {
            foreach (UIElement uiElement in UIElements)
            {
                if (uiElement is InteractableUIElement)
                {
                    ((InteractableUIElement)uiElement).Update();
                }
            }
        }

        /// <summary>
        /// Draws all UIelements using the given spritebatch.
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (UIElement uiElement in UIElements)
            {
                uiElement.Draw(spriteBatch);
            }
        }
    }
}
