namespace Farmdustry.World
{
    public abstract class Structure
    {
        public byte Y { get; set; }
        public byte X { get; set; }
        public byte Height { get; private set; }
        public byte Width { get; private set; }
        public StructureType Type { get; private set; }

        public Structure(byte y, byte x, byte height, byte width, StructureType type)
        {
            Y = y;
            X = x;
            Height = height;
            Width = width;
            Type = type;
        }

        public abstract void Update(WorldGrid parentWorld);
    }
}
