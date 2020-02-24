using Farmdustry.Inventory;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Farmdustry.Entities
{
    public class ItemDropList
    {
        private ItemDrop?[] itemDrops = new ItemDrop?[0];
        private readonly Queue<int> removedItemDropIndexes = new Queue<int>();

        /// <summary>
        /// Add an ItemDrop.
        /// </summary>
        /// <param name="y">The y coördinate of the ItemStack.</param>
        /// <param name="x">The x coördinate of the ItemStack.</param>
        /// <param name="itemType">the type of item to add.</param>
        /// <param name="amount">The amount of items to add.</param>
        /// <returns>The id of the added ItemDrop.</returns>
        public int Add(float y, float x, ItemType itemType, int amount)
        {
            int index;
            //Get a free index that is no longer in use
            if (removedItemDropIndexes.Count > 0)
            {
                index = removedItemDropIndexes.Dequeue();
            }
            //Make the array bigger to get a new index
            else
            {
                index = itemDrops.Length;
                Array.Resize(ref itemDrops, itemDrops.Length + 1);
            }

            //Add the new ItemDrop
            itemDrops[index] = new ItemDrop() { Y = y, X = x, Item = itemType, Amount = amount };

            return index;
        }

        /// <summary>
        /// Remove an ItemDrop.
        /// </summary>
        /// <param name="index">The id of the ItemDrop.</param>
        /// <returns>If the ItemDrop was removed.</returns>
        public bool Remove(int index)
        {
            if (index < itemDrops.Length)
            {
                return false;
            }

            itemDrops[index] = null;
            removedItemDropIndexes.Enqueue(index);
            return true;
        }

        /// <summary>
        /// Get all ItemDrops in a range given a center.
        /// </summary>
        /// <param name="centerY">The y coördinate of the center.</param>
        /// <param name="centerX">The x coördinate of the center.</param>
        /// <param name="range">The range to check for.</param>
        /// <returns>A list of all indexes of ItemDrops in range.</returns>
        public List<int> GetInRange(float centerY, float centerX, float range)
        {
            List<int> itemDropIndexesInRange = new List<int>();
            Vector2 center = new Vector2(centerX, centerY);

            for (int i = 0; i < itemDrops.Length; i++)
            {
                if (!itemDrops[i].HasValue)
                {
                    continue;
                }

                if(Vector2.Distance(center,new Vector2(itemDrops[i].Value.X, itemDrops[i].Value.Y)) <= range)
                {
                    itemDropIndexesInRange.Add(i);
                }
                    
            }

            return itemDropIndexesInRange;
        }

        /// <summary>
        /// Get a snapshot of a ItemDrop.
        /// </summary>
        /// <param name="id">The id of the ItemDrop.</param>
        /// <returns>The snapshot of the ItemDrop.</returns>
        public ItemDrop GetItemDropSnapshot(int id)
        {
            return itemDrops[id].Value;
        }
    }
}
