using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farmdustry.Server
{
    public class InventoryList
    {
        private Dictionary<int, Inventory.Inventory> inventories = new Dictionary<int, Inventory.Inventory>();
        
        public bool GetInventory(int playerId, out Inventory.Inventory inventory)
        {
            //Make a new inventory if the inventory was never used before
            if (!inventories.ContainsKey(playerId))
            {
                inventories.Add(playerId, new Inventory.Inventory(Inventory.Inventory.PLAYER_INVENTORY_VOLUME));
                //TODO Remove debug
                inventories[playerId].AddItem(Inventory.ItemType.CarrotSeed, 3);
            }

            inventory = inventories[playerId];
            return true;
        }
    }
}
