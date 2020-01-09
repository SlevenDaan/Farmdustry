using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Graphics
{
    public class TextureAtlas
    {
        private Texture2D texture;
        private int subTextureHeight;
        private int subTextureWidth;

        public int YSize { get; private set; }
        public int XSize { get; private set; }

        public TextureAtlas(Texture2D texture, int subTextureWidth, int subTextureHeight)
        {
            this.texture = texture;

            this.subTextureWidth = subTextureWidth;
            this.subTextureHeight = subTextureHeight;

            YSize = texture.Width / subTextureWidth;
            XSize = texture.Height / subTextureHeight;
        }

        /// <summary>
        /// Draw a texture.
        /// </summary>
        /// <param name="textureIndex">The index of the sub-texture.</param>
        /// <param name="spriteBatch">The spritebatch used to draw the texture.</param>
        /// <param name="destinationRectangle">The rectangle where the texture is to be drawn.</param>
        /// <param name="color">The color the texture is drawn in.</param>
        public void Draw(int textureYIndex, int textureXIndex, SpriteBatch spriteBatch, Rectangle destinationRectangle, Color color)
        {
            spriteBatch.Draw(texture, destinationRectangle, new Rectangle(textureYIndex * subTextureWidth, textureXIndex * subTextureHeight, subTextureWidth, subTextureHeight), color);
        }
    }
}
