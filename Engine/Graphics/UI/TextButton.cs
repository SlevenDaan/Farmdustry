using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Graphics.UI
{
    public class TextButton : Button
    {
        private Label textLabel;

        public string Text
        {
            get => textLabel.Text;
            set => textLabel.Text = value;
        }

        public SpriteFont Font
        {
            get => textLabel.Font;
            set => textLabel.Font = value;
        }
        public Color FontColor
        {
            get => textLabel.FontColor;
            set => textLabel.FontColor = value;
        }

        public TextButton(Texture2D texture, string text, SpriteFont font, int x, int y, int width, int height, Color hoverColor, Color fontColor) : base(y, x, texture, width, height, hoverColor)
        {
            textLabel = new Label(y, x, text, font, fontColor);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            textLabel.Draw(spriteBatch);
        }
    }
}
