namespace Farmdustry.World
{
    public class Crop
    {
        private float growth;
        private float water;

        public CropType Type { get; set; }
        public float Growth
        {
            get => growth;
            set
            {
                growth = value;
                if (growth < 0)
                {
                    growth = 0;
                }
                else if (growth > 1)
                {
                    growth = 1;
                }
            }
        }
        public float Water
        {
            get => water;
            set
            {
                water = value;
                if (water < 0)
                {
                    water = 0;
                }
                else if (water > 1)
                {
                    water = 1;
                }
            }
        }
    }
}
