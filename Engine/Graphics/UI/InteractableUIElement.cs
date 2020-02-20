using Engine.InputHandler;
using Microsoft.Xna.Framework;

namespace Engine.Graphics.UI
{
    public abstract class InteractableUIElement : UIElement
    {
        public InteractableUIElement(UIElementState state, Point position) : base(state, position)
        {

        }

        /// <summary>
        /// Update the InteractableUIElement.
        /// </summary>
        /// <param name="mouse">The MouseHandler to use for mouse inputs.</param>
        public abstract void Update(MouseHandler mouse);
    }
}
