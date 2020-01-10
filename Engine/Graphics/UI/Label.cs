using Microsoft.Xna.Framework;

namespace Engine.Graphics.UI
{
    public struct Label
    {
        public string Text;
        public Color FontColor;

        public Label(string text, Color fontColor)
        {
            Text = text;
            FontColor = fontColor;
        }
    }
}
