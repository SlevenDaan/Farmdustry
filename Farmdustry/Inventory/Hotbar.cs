namespace Farmdustry.Inventory
{
    public class Hotbar
    {
        public const int PLAYER_HOTBAR_SIZE = 9;

        private ItemType[] items = new ItemType[PLAYER_HOTBAR_SIZE];

        /// <summary>
        /// The index of the selected slot.
        /// </summary>
        public int SelectedSlot { get; private set; } = 0;

        /// <summary>
        /// The currently selected ItemType on the hotbar.
        /// </summary>
        public ItemType SelectedItem => items[SelectedSlot];

        /// <summary>
        /// The amount of slots on the hotbar.
        /// </summary>
        public int Size => PLAYER_HOTBAR_SIZE;

        /// <summary>
        /// Get the ItemType of a slot.
        /// </summary>
        /// <param name="index">The index of the slot.</param>
        /// <returns>The ItemType of the slot.</returns>
        public ItemType GetSlot(int index)
        {
            return items[index];
        }

        /// <summary>
        /// Set the currently selected slot.
        /// </summary>
        /// <param name="index">The index of the slot.</param>
        public void SelectSlot(int index)
        {
            if(index>=PLAYER_HOTBAR_SIZE || index < 0)
            {
                return;
            }

            SelectedSlot = index;
        }

        /// <summary>
        /// Assign an ItemType to a slot.
        /// </summary>
        /// <param name="index">The index of the slot.</param>
        /// <param name="itemType">The ItemType to assign.</param>
        public void AssignSlot(int index, ItemType itemType)
        {
            if (index >= PLAYER_HOTBAR_SIZE || index < 0)
            {
                return;
            }

            items[index] = itemType;
        }
    }
}
