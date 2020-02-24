namespace Farmdustry.Network.Command
{
    /*
    0: Game Tick
    1-149: Player Actions
    150-199: Game Commands
    200-255: Network Commands
    */
    public enum CommandType
    {
        Tick = 1,

        //------------Player Actions------------

        PlantCrop = 10,
        HarvestCrop = 11,

        PlaceStructure = 20,
        DestroyStructure = 21,

        DropItem = 30,
        PickupItem = 31,

        UpdatePlayerLocation = 40,

        //------------Game Commands------------

        AddCrop = 150,
        RemoveCrop = 151,

        AddStructure = 160,
        RemoveStructure = 161,

        AddItemToInventory = 170,
        RemoveItemFromInventory = 171,

        SpawnItemDrop = 180,
        RemoveItemDrop = 181,

        //------------Network Commands------------

        SetPlayerId = 200
    }
}
