using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Graphics.UI
{
    public abstract class UIElement
    {
        public UIElementState State { get; set; }
        public Point Position { get; set; }

        public UIElement(UIElementState state, Point position)
        {
            State = state;
            Position = position;
        }

        /// <summary>
        /// Draw the UIElement.
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatch used to draw.</param>
        public abstract void Draw(SpriteBatch spriteBatch);
    }
}
