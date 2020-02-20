using Engine.InputHandler;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Engine.Graphics.UI
{
    public class Button : InteractableUIElement
    {
        public Point Size { get; set; }

        public string Text { get; set; }
        public SpriteFont Font { get; set; }
        public Color FontColor { get; set; }

        public Texture2D Texture { get; set; }

        public bool Hovered { get; private set; }
        public Color HoverColor { get; set; }

        public event Action<Button> Clicked;

        public Button(UIElementState state, Point position, Point size, string text, SpriteFont font, Color fontColor, Texture2D texture, Color hoverColor) : base(state, position)
        {
            Size = size;
            Text = text;
            Font = font;
            FontColor = fontColor;
            Texture = texture;
            Hovered = false;
            HoverColor = hoverColor;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, new Rectangle(Position, Size), Hovered ? HoverColor : Color.White);
            spriteBatch.DrawString(Font, Text, Position.ToVector2(), FontColor);
        }

        public override void Update(MouseHandler mouse)
        {
            if (State != UIElementState.Active)
            {
                Hovered = false;
                return;
            }

            Hovered = new Rectangle(Position, Size).Contains(mouse.Position);

            if (Hovered && mouse.IsPressed(MouseButtonType.Left))
            {
                Clicked?.Invoke(this);
            }
        }
    }
}
