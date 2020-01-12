using Engine.Graphics;
using Farmdustry.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Farmdustry.Client.Graphics
{
    public class WorldGridRenderer
    {
        private const int DISPLAY_SCALE = 1;
        private const int CELL_SIZE = 32;//In Pixels

        private readonly TextureAtlas soilTextureAtlas;
        private readonly TextureAtlas cropTextureAtlas;
        private readonly TextureAtlas structureTextureAtlas;

        private const int STRUCTURE_HEIGHT = 6;
        private const int STRUCTURE_WIDTH = 4;

        public WorldGridRenderer(TextureAtlas soilTextureAtlas, TextureAtlas cropTextureAtlas, TextureAtlas structureTextureAtlas)
        {
            this.soilTextureAtlas = soilTextureAtlas;
            this.cropTextureAtlas = cropTextureAtlas;
            this.structureTextureAtlas = structureTextureAtlas;
        }

        /// <summary>
        /// Renders all soil of the worldgrid.
        /// </summary>
        /// <param name="spriteBatch">The spritebatch to use for drawing.</param>
        /// <param name="worldGrid">The worldgrid to get all info from.</param>
        public void RenderSoil(SpriteBatch spriteBatch, WorldGrid worldGrid)
        {
            for (byte xIndex = 0; xIndex < worldGrid.Size; xIndex++)
            {
                for (byte yIndex = 0; yIndex < worldGrid.Size; yIndex++)
                {
                    int tilledIndex = Convert.ToInt32(worldGrid.GetTilled(yIndex, xIndex)) * 3;
                    int soilTypeIndex = (int)worldGrid.GetSoil(yIndex, xIndex);
                    int variationIndex = (xIndex * yIndex) % 3;

                    soilTextureAtlas.Draw(
                        soilTypeIndex, tilledIndex + variationIndex, 1, 1,
                        spriteBatch,
                        new Rectangle(xIndex * CELL_SIZE * DISPLAY_SCALE, yIndex * CELL_SIZE * DISPLAY_SCALE, CELL_SIZE * DISPLAY_SCALE, CELL_SIZE * DISPLAY_SCALE),
                        Color.White
                    );
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
            float maxYTextureIndex = cropTextureAtlas.YSize - 1;

            for (byte xIndex = 0; xIndex < worldGrid.Size; xIndex++)
            {
                for (byte yIndex = 0; yIndex < worldGrid.Size; yIndex++)
                {
                    if(!worldGrid.HasCrop(yIndex, xIndex))
                    {
                        continue;
                    }

                    Crop? currentCrop = worldGrid.GetCrop(yIndex, xIndex);

                    int growthIndex = (int) (currentCrop.Value.Growth * maxYTextureIndex);
                    int cropTypeIndex = ((int)currentCrop.Value.Type) - 1;

                    cropTextureAtlas.Draw(
                        cropTypeIndex, growthIndex, 1, 1,
                        spriteBatch,
                        new Rectangle(xIndex * CELL_SIZE * DISPLAY_SCALE, yIndex * CELL_SIZE * DISPLAY_SCALE, CELL_SIZE * DISPLAY_SCALE, CELL_SIZE * DISPLAY_SCALE),
                        Color.White
                    );
                }
            }
        }

        /// <summary>
        /// Renders all structures of the worldgrid.
        /// </summary>
        /// <param name="spriteBatch">The spritebatch to use for drawing.</param>
        /// <param name="worldGrid">The worldgrid to get all info from.</param>
        public void RenderStructures(SpriteBatch spriteBatch, WorldGrid worldGrid)
        {
            for (byte xIndex = 0; xIndex < worldGrid.Size; xIndex++)
            {
                for (byte yIndex = 0; yIndex < worldGrid.Size; yIndex++)
                {
                    if (!worldGrid.HasStructure(yIndex, xIndex))
                    {
                        continue;
                    }

                    Structure currentStructure = worldGrid.GetStructure(yIndex, xIndex);

                    if(xIndex!=currentStructure.X || yIndex != currentStructure.Y)
                    {
                        continue;
                    }

                    int structureTypeIndex = (int)currentStructure.Type;
                    byte structureHeight = currentStructure.Height;

                    structureTextureAtlas.Draw(
                        structureTypeIndex * STRUCTURE_HEIGHT, 0, STRUCTURE_HEIGHT, STRUCTURE_WIDTH,
                        spriteBatch,
                        new Rectangle(xIndex * CELL_SIZE * DISPLAY_SCALE, (yIndex - STRUCTURE_HEIGHT + structureHeight) * CELL_SIZE * DISPLAY_SCALE, CELL_SIZE * STRUCTURE_WIDTH * DISPLAY_SCALE, CELL_SIZE * STRUCTURE_HEIGHT * DISPLAY_SCALE),
                        Color.White
                    );
                }
            }
        }
    }
}
