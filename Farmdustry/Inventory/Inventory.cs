using System.Collections.Generic;

namespace Farmdustry.Inventory
{
    public class Inventory
    {
        private Dictionary<ItemType, int> items = new Dictionary<ItemType, int>();

        /// <summary>
        /// The weigth of all the items in the inventory.
        /// </summary>
        public float Weight
        {
            get;
            private set;
        } = 0;
        /// <summary>
        /// The max amount of weight available in the inventory.
        /// </summary>
        public float MaxWeight { get; private set; }

        public Inventory(float maxWeight)
        {
            MaxWeight = maxWeight;
        }

        /// <summary>
        /// Add items to the inventory.
        /// </summary>
        /// <param name="itemType">The type of item to add.</param>
        /// <param name="amount">The amount of the type of item to add.</param>
        /// <returns>If the item has been added.</returns>
        public bool AddItem(ItemType itemType, int amount)
        {
            float addedWeight = ItemLibrary.GetWeight(itemType) * amount;

            //Check if there is enough space
            if (addedWeight + Weight > MaxWeight)
            {
                return false;
            }

            //Add items to the inventory
            if (!items.ContainsKey(itemType))
            {
                items.Add(itemType, 0);
            }
            items[itemType] = amount;

            //Add weight to the inventory
            Weight += addedWeight;

            return true;
        }
        /// <summary>
        /// Count the amount of items in the inventory.
        /// </summary>
        /// <param name="itemType">The type of item to count.</param>
        /// <returns>The count.</returns>
        public int CountItem(ItemType itemType)
        {
            if (!items.ContainsKey(itemType))
            {
                return 0;
            }

            return items[itemType];
        }
        /// <summary>
        /// Remove items from the inventory.
        /// </summary>
        /// <param name="itemType">The type of item to remove.</param>
        /// <param name="amount">The amount of the type of item to remove.</param>
        /// <returns>If the item has been removed.</returns>
        public bool RemoveItem(ItemType itemType, int amount)
        {
            //Check if inventory has the items
            if (CountItem(itemType) < amount)
            {
                return false;
            }

            //Remove items from the inventory
            items[itemType] -= amount;

            //Remove weight from the inventory
            Weight -= ItemLibrary.GetWeight(itemType) * amount;

            return true;
        }
    }
}
