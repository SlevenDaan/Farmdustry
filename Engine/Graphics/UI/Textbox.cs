using Engine.InputHandler;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Engine.Graphics.UI
{
    public class Textbox : InteractableUIElement
    {
        public Point Size { get; set; }

        public int MaxTextLength { get; set; }
        public string Text { get; private set; }
        public SpriteFont Font { get; set; }
        public Color FontColor { get; set; }

        public Texture2D Texture { get; set; }

        public bool Selected { get; private set; }
        public Color SelectedColor { get; set; }

        public Textbox(UIElementState state, Point position, Point size, int maxTextLength, string text, SpriteFont font, Color fontColor, Texture2D texture, Color selectedColor) : base(state, position)
        {
            Size = size;
            MaxTextLength = maxTextLength;
            Text = text;
            Font = font;
            FontColor = fontColor;
            Texture = texture;
            Selected = false;
            SelectedColor = selectedColor;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, new Rectangle(Position, Size), Selected ? SelectedColor : Color.White);
            spriteBatch.DrawString(Font, Text, Position.ToVector2(), FontColor);
        }

        public override void Update(MouseHandler mouse)
        {
            if (State != UIElementState.Active)
            {
                return;
            }

            if (mouse.IsPressed(MouseButtonType.Left))
            {
                Selected = new Rectangle(Position, Size).Contains(mouse.Position);
            }
        }

        public void InputText(List<char> typedCharacters)
        {
            if (!Selected || State != UIElementState.Active)
            {
                return;
            }

            foreach (char character in typedCharacters)
            {
                if (character == '\b')
                {
                    if (Text.Length > 0)
                    {
                        Text = Text.Substring(0, Text.Length - 1);
                    }
                }
                else if(Text.Length<MaxTextLength)
                {
                    Text += character;
                }
            }
        }
    }
}