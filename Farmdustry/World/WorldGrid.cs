using System.Collections.Generic;

namespace Farmdustry.World
{
    public class WorldGrid
    {
        private const int WORLD_SIZE = 16;
        private readonly WorldCell[,] cells = new WorldCell[WORLD_SIZE, WORLD_SIZE];

        private byte structuresCount = 0;
        private readonly List<Structure> structures = new List<Structure>();
        private float structureUpdateTimer = 0;
        private const float structureUpdateTime = 1f;

        private byte cropsCount = 0;
        private readonly Crop[] crops = new Crop[WORLD_SIZE * WORLD_SIZE];
        private readonly Queue<int> removedCropsIndexes = new Queue<int>();

        public int Size => WORLD_SIZE;

        public WorldGrid()
        {
            for (int yIndex = 0; yIndex < WORLD_SIZE; yIndex++)
            {
                for (int xIndex = 0; xIndex < WORLD_SIZE; xIndex++)
                {
                    cells[yIndex, xIndex] = new WorldCell(SoilType.Stone);
                }
            }
        }

        /// <summary>
        /// Gives the type of soil at a given coördinate.
        /// </summary>
        /// <param name="y">The y coördinate of the crop.</param>
        /// <param name="x">The x coördinate of the crop.</param>
        /// <returns>The type of soil.</returns>
        public SoilType GetSoil(byte y, byte x)
        {
            return cells[y, x].SoilType;
        }

        /// <summary>
        /// Set if a cell is tilled or not.
        /// </summary>
        /// <param name="y">The y coördinate.</param>
        /// <param name="x">The x coördinate.</param>
        /// <param name="isTilled"></param>
        public void SetTilled(byte y,byte x, bool isTilled)
        {
            cells[y, x].Tilled = isTilled;
        }
        /// <summary>
        /// Get if a cell it tilled or not.
        /// </summary>
        /// <param name="y">The y coördinate.</param>
        /// <param name="x">The x coördinate.</param>
        /// <returns>If the soil at the given coördinate is tilled.</returns>
        public bool GetTilled(byte y,byte x)
        {
            return cells[y, x].Tilled;
        }

        /// <summary>
        /// Add a structure to the worldgrid.
        /// </summary>
        /// <param name="y">The y coördinate of the top left corner of the structure.</param>
        /// <param name="x">The x coördinate  of the top left corner of the structure.</param>
        /// <param name="structureType">The type of structure to add.</param>
        /// <returns>If the structure has been added.</returns>
        public bool AddStructure(byte y, byte x, StructureType structureType)
        {
            Structure structure = StructureFactory.Create(y, x, structureType);
            //Check if structureType has a valid creation
            if (structure == null)
            {
                return false;
            }

            structure.Y = y;
            structure.X = x;
            byte height = structure.Height;
            byte width = structure.Width;

            //Check if position is out of bounds
            if (IsOutOfBound(y, x) || IsOutOfBound(y + height - 1, x + width - 1))
            {
                return false;
            }

            //Checks if a structure is already present
            for (byte yIndex = 0; yIndex < height; yIndex++)
            {
                for (byte xIndex = 0; xIndex < width; xIndex++)
                {
                    if(cells[y + yIndex, x + xIndex].Structure != null)
                    {
                        return false;
                    }
                }
            }

            //Gives all structure pointers to the required cells
            for(byte yIndex = 0; yIndex < height; yIndex++)
            {
                for (byte xIndex = 0; xIndex < width; xIndex++)
                {
                    cells[y + yIndex, x + xIndex].Structure = structure;
                }
            }

            //Add structure to the structure list
            structures.Add(structure);
            structuresCount++;

            return true;
        }
        /// <summary>
        /// Get a structure from the worldgrid.
        /// </summary>
        /// <param name="y">A y coördinate containing the structure.</param>
        /// <param name="x">A x coördinate containing the structure.</param>
        /// <returns>The structure at the given coördinates.</returns>
        public Structure GetStructure(byte y,byte x)
        {
            return cells[y, x].Structure;
        }
        /// <summary>
        /// Get if a structure exists.
        /// </summary>
        /// <param name="y">The y coördinate of the structure.</param>
        /// <param name="x">The x coördinate of the structure.</param>
        /// <returns>If the structure exists.</returns>
        public bool HasStructure(byte y, byte x)
        {
            return cells[y, x].Structure != null;
        }
        /// <summary>
        /// Remove a structure from the worldgrid.
        /// </summary>
        /// <param name="y">A y coördinate containing the structure.</param>
        /// <param name="x">A x coördinate containing the structure.</param>
        /// <param name="removedStructure">Contains the removed structure.</param>
        /// <returns>If there was a structure present to remove.</returns>
        public bool RemoveStructure(byte y, byte x, out Structure removedStructure)
        {
            //Check if structureType has a valid creation
            if (IsOutOfBound(y, x))
            {
                removedStructure = null;
                return false;
            }

            removedStructure = cells[y, x].Structure;

            //Check if there is an existing structure
            if (removedStructure == null)
            {
                return false;
            }

            //Remove the structure
            y = removedStructure.Y;
            x = removedStructure.X;
            byte height = removedStructure.Height;
            byte width = removedStructure.Width;

            for (byte yIndex = 0; yIndex < height; yIndex++)
            {
                for (byte xIndex = 0; xIndex < width; xIndex++)
                {
                    cells[y + yIndex, x + xIndex].Structure = null;
                }
            }

            structures.Remove(removedStructure);
            structuresCount--;

            return true;
        }
        /// <summary>
        /// Updates all structures.
        /// </summary>
        /// <param name="deltaTime">The time in seconds between this update and the previous one.</param>
        public void UpdateStructures(float deltaTime)
        {
            structureUpdateTimer += deltaTime;

            //Check if an update needs to happen
            if (structureUpdateTimer < structureUpdateTime)
            {
                return;
            }

            //Update all structures
            while (structureUpdateTimer >= structureUpdateTime)
            {
                for (byte i = 0; i < structuresCount; i++)
                {
                    Structure currentstructure = structures[i];
                    currentstructure.Update(this);
                }
                structureUpdateTimer -= structureUpdateTimer;
            }
        }

        /// <summary>
        /// Add a crop to the worldgrid.
        /// </summary>
        /// <param name="y">The y coördinate containing the crop.</param>
        /// <param name="x">The x coördinate containing the crop.</param>
        /// <param name="cropType">The type of crop to add.</param>
        /// <returns>If the crop has been added.</returns>
        public bool AddCrop(byte y, byte x, CropType cropType)
        {
            //Check if position is out of bounds
            if (IsOutOfBound(y, x))
            {
                return false;
            }

            //Check if a crop is already present
            if (cells[y, x].CropIndex != -1)
            {
                return false;
            }

            //Add the crop
            int cropIndex = GetNextCropId();
            crops[cropIndex] = new Crop() { Type = cropType };
            cells[y, x].CropIndex = cropIndex;

            return true;
        }
        /// <summary>
        /// Get an open id for a crop.
        /// </summary>
        /// <returns>The id.</returns>
        private int GetNextCropId()
        {
            if (removedCropsIndexes.Count > 0)
            {
                return removedCropsIndexes.Dequeue();
            }

            return cropsCount++;
        }
        /// <summary>
        /// Get a crop from the worldgrid.
        /// </summary>
        /// <param name="y">The y coördinate of the crop.</param>
        /// <param name="x">The x coördinate of the crop.</param>
        /// <returns>A copy of the crop at the given coördinates.</returns>
        public Crop GetCrop(byte y, byte x)
        {
            return crops[cells[y, x].CropIndex];
        }
        /// <summary>
        /// Get if a crop exists.
        /// </summary>
        /// <param name="y">The y coördinate of the crop.</param>
        /// <param name="x">The x coördinate of the crop.</param>
        /// <returns>If the crop exists.</returns>
        public bool HasCrop(byte y,byte x)
        {
            return cells[y, x].CropIndex != -1;
        }
        /// <summary>
        /// Remove a crop from the worldgrid.
        /// </summary>
        /// <param name="y">The y coördinate containing the crop.</param>
        /// <param name="x">The x coördinate containing the crop.</param>
        /// <param name="removedCrop">Contains the removed crop.</param>
        /// <returns>If there was a crop present to remove.</returns>
        public bool RemoveCrop(byte y, byte x, out Crop removedCrop)
        {
            //Check if position is out of bounds
            if (IsOutOfBound(y, x))
            {
                removedCrop = new Crop();
                return false;
            }

            int removedCropIndex = cells[y, x].CropIndex;

            //Check if a crop is present
            if (removedCropIndex == -1)
            {
                removedCrop = new Crop();
                return false;
            }

            //Remove the crop
            removedCrop = crops[removedCropIndex];
            crops[removedCropIndex].Type = CropType.None;
            removedCropsIndexes.Enqueue(removedCropIndex);
            cells[y, x].CropIndex = -1;

            return true;
        }
        /// <summary>
        /// Updates all crops.
        /// </summary>
        /// <param name="deltaTime">The time in seconds between this update and the previous one.</param>
        public void UpdateCrops(float deltaTime)
        {
            for (byte i = 0; i < cropsCount; i++)
            {
                if (crops[i].Type!=CropType.None)
                {
                    crops[i].Growth += deltaTime * 1f/30f;
                    crops[i].Water -= deltaTime * 1f/30f;

                    //TODO
                }
            }
        }

        /// <summary>
        /// Check if a position is outside of the worldgrid area.
        /// </summary>
        /// <param name="y">The y coördinate.</param>
        /// <param name="x">The x coördinate.</param>
        /// <returns>If the given position is out of bounds.</returns>
        public bool IsOutOfBound(int y,int x)
        {
            return x < 0 || y < 0 || x >= WORLD_SIZE || y > WORLD_SIZE;
        }
    }
}
