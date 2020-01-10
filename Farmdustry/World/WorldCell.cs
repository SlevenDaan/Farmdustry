namespace Farmdustry.World
{
    public struct WorldCell
    {
        public readonly SoilType SoilType;
        public bool Tilled;
        public Structure Structure;
        public int CropIndex;

        public WorldCell(SoilType soilType)
        {
            SoilType = soilType;
            Tilled = false;
            Structure = null;
            CropIndex = -1;
        }
    }
}
