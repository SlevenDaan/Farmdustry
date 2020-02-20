using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Graphics.UI
{
    public class Sprite : UIElement
    {
        public Point Size { get; set; }

        public Texture2D Image { get; set; }
        public Color ImageColor { get; set; }

        public Sprite(UIElementState state, Point position, Point size, Texture2D image, Color imageColor) : base(state, position)
        {
            Size = size;
            Image = image;
            ImageColor = imageColor;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Image, new Rectangle(Position, Size), ImageColor);
        }
    }
}
