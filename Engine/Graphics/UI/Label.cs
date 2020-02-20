using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Graphics.UI
{
    public class Label : UIElement
    {
        public string Text { get; set; }
        public SpriteFont Font { get; set; }
        public Color FontColor { get; set; }

        public Label(UIElementState state, Point position, string text, SpriteFont font, Color fontColor) : base(state, position)
        {
            Text = text;
            Font = font;
            FontColor = fontColor;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(Font, Text, Position.ToVector2(), FontColor);
        }
    }
}
