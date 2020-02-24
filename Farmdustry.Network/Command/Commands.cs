using Farmdustry.Inventory;
using Farmdustry.World;
using System;

namespace Farmdustry.Network.Command
{
    public static class Commands
    {
        /// <summary>
        /// Create a byte array.
        /// 0: size of data.
        /// 1: type of command.
        /// </summary>
        /// <param name="size">The size of the array.</param>
        /// <param name="commandType">The type of command.</param>
        /// <returns>A byte array with the size of the array at index 0 and the type of command at index 1.</returns>
        private static byte[] CreateData(byte size, CommandType commandType)
        {
            byte[] data = new byte[size];
            data[0] = size;
            data[1] = (byte)commandType;
            return data;
        }

        public static byte[] Tick(float deltaTime)
        {
            byte[] data = CreateData(6, CommandType.Tick);
            BitConverter.GetBytes(deltaTime).CopyTo(data, 2);
            return data;
        }

        //------------Player Actions------------

        public static byte[] PlantCrop(byte playerId, byte y, byte x, CropType type)
        {
            byte[] data = CreateData(6, CommandType.PlantCrop);
            data[2] = playerId;
            data[3] = y;
            data[4] = x;
            data[5] = (byte)type;
            return data;
        }

        public static byte[] HarvestCrop(byte playerId, byte y, byte x)
        {
            byte[] data = CreateData(5, CommandType.HarvestCrop);
            data[2] = playerId;
            data[3] = y;
            data[4] = x;
            return data;
        }

        public static byte[] PlaceStructure(byte playerId, byte y, byte x, StructureType type)
        {
            byte[] data = CreateData(6, CommandType.PlaceStructure);
            data[2] = playerId;
            data[3] = y;
            data[4] = x;
            data[5] = (byte)type;
            return data;
        }

        public static byte[] DestroyStructure(byte playerId, byte y, byte x)
        {
            byte[] data = CreateData(5, CommandType.DestroyStructure);
            data[2] = playerId;
            data[3] = y;
            data[4] = x;
            return data;
        }

        public static byte[] DropItem(byte playerId, float y, float x, ItemType itemType, int amount)
        {
            byte[] data = CreateData(16, CommandType.DropItem);
            data[2] = playerId;
            BitConverter.GetBytes(y).CopyTo(data, 3);
            BitConverter.GetBytes(x).CopyTo(data, 7);
            data[11] = (byte)itemType;
            BitConverter.GetBytes(amount).CopyTo(data, 12);
            return data;
        }

        public static byte[] PickupItem(byte playerId, float y, float x)
        {
            byte[] data = CreateData(11, CommandType.PickupItem);
            data[2] = playerId;
            BitConverter.GetBytes(y).CopyTo(data, 3);
            BitConverter.GetBytes(x).CopyTo(data, 7);
            return data;
        }

        public static byte[] UpdatePlayerLocation(byte playerId, float y, float x, float yVelocity, float xVelocity)
        {
            byte[] data = CreateData(19, CommandType.UpdatePlayerLocation);
            data[2] = playerId;
            BitConverter.GetBytes(y).CopyTo(data, 3);
            BitConverter.GetBytes(x).CopyTo(data, 7);
            BitConverter.GetBytes(yVelocity).CopyTo(data, 11);
            BitConverter.GetBytes(xVelocity).CopyTo(data, 15);
            return data;
        }

        //------------Game Commands------------

        public static byte[] AddCrop(byte y, byte x, CropType type)
        {
            byte[] data = CreateData(5, CommandType.AddCrop);
            data[2] = y;
            data[3] = x;
            data[4] = (byte)type;
            return data;
        }

        public static byte[] RemoveCrop(byte y, byte x)
        {
            byte[] data = CreateData(4, CommandType.RemoveCrop);
            data[2] = y;
            data[3] = x;
            return data;
        }

        public static byte[] AddStructure(byte y, byte x, StructureType type)
        {
            byte[] data = CreateData(5, CommandType.AddStructure);
            data[2] = y;
            data[3] = x;
            data[4] = (byte)type;
            return data;
        }

        public static byte[] RemoveStructure(byte y, byte x)
        {
            byte[] data = CreateData(4, CommandType.RemoveStructure);
            data[2] = y;
            data[3] = x;
            return data;
        }

        public static byte[] AddItemToInventory(byte playerId, ItemType itemType, int amount)
        {
            byte[] data = CreateData(8, CommandType.AddItemToInventory);
            data[2] = playerId;
            data[3] = (byte)itemType;
            BitConverter.GetBytes(amount).CopyTo(data, 4);
            return data;
        }

        public static byte[] RemoveItemFromInventory(byte playerId, ItemType itemType, int amount)
        {
            byte[] data = CreateData(8, CommandType.RemoveItemFromInventory);
            data[2] = playerId;
            data[3] = (byte)itemType;
            BitConverter.GetBytes(amount).CopyTo(data, 4);
            return data;
        }

        public static byte[] SpawnItemDrop(float y, float x, ItemType itemType, int amount)
        {
            byte[] data = CreateData(15, CommandType.SpawnItemDrop);
            BitConverter.GetBytes(y).CopyTo(data, 2);
            BitConverter.GetBytes(x).CopyTo(data, 6);
            data[10] = (byte)itemType;
            BitConverter.GetBytes(x).CopyTo(data, 11);
            return data;
        }

        public static byte[] RemoveItemDrop(int itemDropIndex)
        {
            byte[] data = CreateData(6, CommandType.RemoveItemDrop);
            BitConverter.GetBytes(itemDropIndex).CopyTo(data, 2);
            return data;
        }

        //------------Network Commands------------

        public static byte[] SetPlayerId(byte playerId)
        {
            byte[] data = CreateData(3, CommandType.SetPlayerId);
            data[2] = playerId;
            return data;
        }
    }
}
