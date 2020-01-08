using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farmdustry.Inventory
{
    public struct ItemStack
    {
        private byte amount;

        public byte Amount {
            get => amount;
            set
            {
                amount = value;
                if (amount < 0)
                {
                    amount = 0;
                    Type = ItemType.None;
                }
            }
        }
        public ItemType Type { get; private set; }

        public ItemStack(ItemType type, byte amount)
        {
            if (amount < 0)
            {
                amount = 0;
                type = ItemType.None;
            }
            this.amount = amount;
            Type = type;
        }
    }
}
