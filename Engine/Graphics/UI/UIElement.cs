using Microsoft.Xna.Framework;

namespace Engine.Graphics.UI
{
    public struct UIElement<ElementType>
    {
        private static int nextId = 0;

        public readonly int Id;
        public UIElementState State;
        public Point Position;
        public ElementType Element;

        public UIElement(int y, int x, ElementType element, UIElementState state)
        {
            Id = nextId++;
            State = state;
            Position = new Point(x, y);
            Element = element;
        }
    }
}
