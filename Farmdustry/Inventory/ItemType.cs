namespace Farmdustry.Inventory
{
    /// <summary>
    /// Item types correspond to their item id.
    /// 0: No Item
    /// 1-49: Seeds
    /// 50: Dead crop
    /// 51-99: Crops
    /// 100: ?
    /// 101-149: Structures
    /// 150-255: ?
    /// </summary>
    public enum ItemType
    {
        None = 0,

        Carrot = 1,
        Wheat = 2,

        CarrotSeed = 51,
        WheatSeed = 52,

        Container = 101
    }
}
