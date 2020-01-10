using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Graphics.UI
{
    public struct Button
    {
        public Point Size;
        public string Text;
        public Color FontColor;
        public Texture2D Texture;
        public bool Hovered;
        public Color HoverColor;

        public Button(int height, int width, string text, Color fontColor, Texture2D texture, Color hoverColor)
        {
            Size = new Point(width, height);
            Text = text;
            FontColor = fontColor;
            Texture = texture;
            Hovered = false;
            HoverColor = hoverColor;
        }
    }
}
