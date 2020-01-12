using Farmdustry.World.Structures;

namespace Farmdustry.World
{
    public static class StructureFactory
    {
        public static Structure Create(byte y, byte x, StructureType type)
        {
            switch (type)
            {
                case StructureType.Container:
                    return new Container(y, x);

                default:
                    return null;
            }
        }
    }
}
