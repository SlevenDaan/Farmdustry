using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farmdustry.Inventory
{
    /// <summary>
    /// Item types correspond to their item id.
    /// 0: No Item
    /// 1-49: Seeds
    /// 50: Dead crop
    /// 51-99: Crops
    /// 100-149: Structures
    /// 150-255: ?
    /// </summary>
    public enum ItemType
    {
        None = 0,

        CarrotSeed = 1,

        Carrot = 51,
    }
}
