using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Graphics.UI
{
    public abstract class UIElement
    {
        /// <summary>
        /// The Y coördinate of the top left corner of the element.
        /// </summary>
        public int Y { get; set; }
        /// <summary>
        /// The X coördinate of the top left corner of the element.
        /// </summary>
        public int X { get; set; }

        public UIElement(int y, int x)
        {
            Y = y;
            X = x;
        }

        /// <summary>
        /// Draw the UiElement.
        /// </summary>
        /// <param name="spriteBatch">The sprite</param>
        public abstract void Draw(SpriteBatch spriteBatch);
    }
}
