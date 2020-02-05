namespace Farmdustry.Network.Command
{
    /*
    0: Not used
    1-199: Game Commands
    200-255: Network Commands
    */
    public enum CommandType
    {
        Tick = 1,

        AddCrop = 10,
        RemoveCrop = 11,

        AddStructure = 20,
        RemoveStructure = 21,

        UpdatePlayerLocation = 30,

        AddItemToInventory = 40,
        RemoveItemFromInventory = 41,

        SetPlayerId = 200
    }
}
