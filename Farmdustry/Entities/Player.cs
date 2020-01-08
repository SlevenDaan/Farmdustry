using Farmdustry.Inventory;

namespace Farmdustry.Entities
{
    public class Player
    {
        public float Y { get; set; }
        public float X { get; set; }
        public float YVelocity { get; set; }
        public float XVelocity { get; set; }

        public Inventory.Inventory Inventory { get; private set; } = new Inventory.Inventory(20);
    }
}
