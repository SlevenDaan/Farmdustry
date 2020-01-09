using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Graphics.UI
{
    public class Label : UIElement
    {
        public string Text { get; set; }

        public SpriteFont Font { get; set; }
        public Color FontColor { get; set; }

        public Label(int y, int x, string text, SpriteFont font, Color fontColor) : base(y, x)
        {
            Text = text;
            Font = font;
            FontColor = fontColor;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(Font, Text, new Vector2(X, Y), FontColor);
        }
    }
}
