using Engine.Graphics;
using Farmdustry.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farmdustry.Client.Graphics
{
    public class WorldGridRenderer
    {
        private int CELL_SIZE = 32;//In Pixels

        private TextureAtlas soilTextureAtlas;
        private TextureAtlas cropTextureAtlas;

        public WorldGridRenderer(TextureAtlas soilTextureAtlas, TextureAtlas cropTextureAtlas)
        {
            this.soilTextureAtlas = soilTextureAtlas;
            this.cropTextureAtlas = cropTextureAtlas;
        }

        /// <summary>
        /// Renders all soil of the worldgrid.
        /// </summary>
        /// <param name="spriteBatch">The spritebatch to use for drawing.</param>
        /// <param name="worldGrid">The worldgrid to get all info from.</param>
        public void RenderSoil(SpriteBatch spriteBatch, WorldGrid worldGrid)
        {
            int MaxYSize = cropTextureAtlas.YSize;

            for (byte xIndex = 0; xIndex < worldGrid.Size; xIndex++)
            {
                for (byte yIndex = 0; yIndex < worldGrid.Size; yIndex++)
                {
                    int tilledIndex = Convert.ToInt32(worldGrid.GetTilled(yIndex, xIndex));
                    int soilTypeIndex = (int)worldGrid.GetSoil(yIndex, xIndex);

                    soilTextureAtlas.Draw(tilledIndex, soilTypeIndex, spriteBatch, new Rectangle(xIndex * CELL_SIZE, yIndex * CELL_SIZE, CELL_SIZE, CELL_SIZE), Color.White);
                }
            }
        }

        /// <summary>
        /// Renders all crops of the worldgrid.
        /// </summary>
        /// <param name="spriteBatch">The spritebatch to use for drawing.</param>
        /// <param name="worldGrid">The worldgrid to get all info from.</param>
        public void RenderCrops(SpriteBatch spriteBatch, WorldGrid worldGrid)
        {
            int MaxYSize = cropTextureAtlas.YSize;

            for (byte xIndex = 0; xIndex < worldGrid.Size; xIndex++)
            {
                for (byte yIndex = 0; yIndex < worldGrid.Size; yIndex++)
                {
                    if(!worldGrid.HasCrop(yIndex, xIndex))
                    {
                        continue;
                    }
                    Crop? currentCrop = worldGrid.GetCrop(yIndex, xIndex);

                    int growthIndex = (int)currentCrop.Value.Growth * MaxYSize;
                    int cropTypeIndex = ((int)currentCrop.Value.Type) - 1;

                    cropTextureAtlas.Draw(growthIndex, cropTypeIndex, spriteBatch, new Rectangle(xIndex * CELL_SIZE, yIndex * CELL_SIZE, CELL_SIZE, CELL_SIZE), Color.White);
                }
            }
        }
    }
}
