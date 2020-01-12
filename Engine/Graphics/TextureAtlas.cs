using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Graphics
{
    public class TextureAtlas
    {
        private Texture2D textureGrid;
        private int textureGridCellHeight;
        private int textureGridCellWidth;

        public int YSize { get; private set; }
        public int XSize { get; private set; }

        public TextureAtlas(Texture2D textureGrid, int textureGridCellHeight, int textureGridCellWidth)
        {
            this.textureGrid = textureGrid;

            this.textureGridCellWidth = textureGridCellHeight;
            this.textureGridCellHeight = textureGridCellWidth;

            YSize = textureGrid.Width / textureGridCellHeight;
            XSize = textureGrid.Height / textureGridCellWidth;
        }

        /// <summary>
        /// Draw a texture.
        /// </summary>
        /// <param name="textureY">The y coördinate on the texture grid.</param>
        /// <param name="textureX">The x coördinate on the texture grid.</param>
        /// <param name="textureHeight">The height on the texture grid.</param>
        /// <param name="textureWidth">The height on the texture grid.</param>
        /// <param name="spriteBatch">The spritebatch used for drawing.</param>
        /// <param name="destinationRectangle">The destination of the texture on the screen.</param>
        /// <param name="color">The color of the texture.</param>
        public void Draw(int textureY, int textureX, int textureHeight, int textureWidth, SpriteBatch spriteBatch, Rectangle destinationRectangle, Color color)
        {
            spriteBatch.Draw(textureGrid, destinationRectangle, new Rectangle(textureX * textureGridCellWidth, textureY * textureGridCellHeight, textureWidth * textureGridCellWidth, textureHeight * textureGridCellHeight), color);
        }
    }
}
