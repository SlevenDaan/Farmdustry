namespace Farmdustry.World
{
    public struct WorldCell
    {
        public SoilType SoilType { get; private set; }
        public bool Tilled { get; set; }
        public Structure Structure { get; set; }
        public Crop Crop { get; set; }

        public WorldCell(SoilType soilType)
        {
            SoilType = soilType;
            Tilled = false;
            Structure = null;
            Crop = null;
        }
    }
}
