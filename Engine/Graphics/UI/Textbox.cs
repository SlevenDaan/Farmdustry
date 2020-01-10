using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Graphics.UI
{
    public struct Textbox
    {
        public Point Size;
        public int MaxTextLength;
        public string Text;
        public Color FontColor;
        public Texture2D Texture;
        public bool Selected;
        public Color SelectedColor;

        public Textbox(int height, int width, int maxTextLength, string text, Color fontColor, Texture2D texture, Color selectedColor)
        {
            Size = new Point(width, height);
            MaxTextLength = maxTextLength;
            Text = text;
            FontColor = fontColor;
            Texture = texture;
            Selected = false;
            SelectedColor = selectedColor;
        }
    }
}
