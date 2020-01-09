using Engine.InputHandler;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Graphics.UI
{
    public class Textbox : InteractableUIElement
    {
        private Label textLabel;

        public bool Enabled { get; set; }

        public Textbox(int y, int x, SpriteFont font, Color fontColor) : base(y, x)
        {
            textLabel = new Label(y, x, "", font, fontColor);
        }

        public override void Update()
        {
            if (!Enabled)
            {
                return;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            textLabel.Draw(spriteBatch);
        }
    }
}
