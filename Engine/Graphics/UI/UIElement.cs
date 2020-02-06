using Microsoft.Xna.Framework;

namespace Engine.Graphics.UI
{
    public struct UIElement<ElementType>
    {
        public bool Removed;
        public UIElementState State;
        public Point Position;
        public ElementType Element;

        public UIElement(int y, int x, ElementType element, UIElementState state)
        {
            Removed = false;
            State = state;
            Position = new Point(x, y);
            Element = element;
        }
    }
}
