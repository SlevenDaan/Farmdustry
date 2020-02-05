using Farmdustry.Network.Command;
using Farmdustry.World;
using System;
using System.Diagnostics;
using System.Timers;
using Farmdustry.Helper;
using Farmdustry.Inventory;

namespace Farmdustry.Server
{
    class Server
    {
        private const int TICK_RATE = 10;
        private static Timer timer;
        private static Stopwatch deltaTimeStopwatch = new Stopwatch();

        private static Network.Server server = new Network.Server(25566);
        private static CommandList commandsToSendOnNextTick = new CommandList();

        private static WorldGrid worldGrid = new WorldGrid();

        private static InventoryList inventories = new InventoryList();

        static void Main(string[] arguments)
        {
            Console.Title = $"Farmdustry Server {Network.Network.GetLocalIp().ToString()}:25566";

            server.ConnectionEstablished += ConnectionEstablished;
            server.DataReceived += DataReceived;

            timer = new Timer(1f / TICK_RATE * 1000f);
            timer.Elapsed += Tick;
            deltaTimeStopwatch.Start();
            timer.Start();

            string input;
            while ((input = Console.ReadLine().ToLower()) != "stop")
            {
                //Some commands the server host can issue
            }
        }

        private static void DataReceived(byte[] data)
        {
            Console.WriteLine($"Data received Length: {data.Length}");

            int startingIndex = 0;
            while (startingIndex < data.Length)
            {
                bool valid = false;

                switch ((CommandType)data[startingIndex + 1])
                {
                    case CommandType.AddCrop:
                        {
                            byte playerId = data[startingIndex + 2];
                            byte y = data[startingIndex + 3];
                            byte x = data[startingIndex + 4];
                            CropType cropType = (CropType)data[startingIndex + 5];

                            inventories.GetInventory(playerId, out Inventory.Inventory inventory);
                            ItemType cropItemType = (ItemType)((int)cropType + 50);//Convert crop to seed item
                            //Check if player has enough seeds to plant a crop
                            if (inventory.RemoveItem(cropItemType, 1))
                            {
                                AddDataToCommandList(Commands.RemoveItemFromInventory(playerId, cropItemType, 1));
                                valid = worldGrid.AddCrop(y, x, cropType);
                            }
                            break;
                        }
                    case CommandType.RemoveCrop:
                        {
                            byte playerId = data[startingIndex + 2];
                            byte y = data[startingIndex + 3];
                            byte x = data[startingIndex + 4];

                            if(valid = worldGrid.RemoveCrop(y, x, out Crop crop))
                            {
                                inventories.GetInventory(playerId, out Inventory.Inventory inventory);
                                ItemType cropItemType = (ItemType)((int)crop.Type + (50 * Convert.ToInt32(crop.Growth != 1))); //Convert crop to crop seed or item

                                //Check if player has enough space in their inventory
                                if (inventory.AddItem(cropItemType, 1))
                                {
                                    AddDataToCommandList(Commands.AddItemToInventory(playerId, cropItemType, 1));
                                }
                                else
                                {
                                    //TODO Drop the item
                                }
                            }
                            break;
                        }
                    case CommandType.AddStructure:
                        {
                            byte playerId = data[startingIndex + 2];
                            byte y = data[startingIndex + 3];
                            byte x = data[startingIndex + 4];
                            StructureType structureType = (StructureType)data[startingIndex + 5];
                            valid = worldGrid.AddStructure(y, x, structureType);
                            break;
                        }
                    case CommandType.RemoveStructure:
                        {
                            byte playerId = data[startingIndex + 2];
                            byte y = data[startingIndex + 3];
                            byte x = data[startingIndex + 4];
                            valid = worldGrid.RemoveStructure(y, x, out _);
                            break;
                        }
                    case CommandType.UpdatePlayerLocation:
                        {
                            byte playerId = data[startingIndex + 2];
                            float y = BitConverter.ToSingle(data.SubArray(startingIndex + 3, 4), 0);
                            float x = BitConverter.ToSingle(data.SubArray(startingIndex + 7, 4), 0);
                            float yVelocity = BitConverter.ToSingle(data.SubArray(startingIndex + 11, 4), 0);
                            float xVelocity = BitConverter.ToSingle(data.SubArray(startingIndex + 15, 4), 0);
                            //TODO Change player position
                            //TODO Collision check
                            valid = true;
                            break;
                        }
                    case CommandType.AddItemToInventory:
                        {
                            byte playerId = data[startingIndex + 2];
                            ItemType itemType = (ItemType)data[startingIndex + 3];
                            int amount = BitConverter.ToInt32(data.SubArray(startingIndex + 4, 4), 0);

                            inventories.GetInventory(playerId, out Inventory.Inventory inventory);
                            valid = inventory.AddItem(itemType, amount);
                            break;
                        }
                    case CommandType.RemoveItemFromInventory:
                        {
                            byte playerId = data[startingIndex + 2];
                            ItemType itemType = (ItemType)data[startingIndex + 3];
                            int amount = BitConverter.ToInt32(data.SubArray(startingIndex + 4, 4), 0);

                            inventories.GetInventory(playerId, out Inventory.Inventory inventory);
                            valid = inventory.RemoveItem(itemType, amount);
                            break;
                        }
                }

                byte dataSize = data[startingIndex];

                if (valid)
                {
                    AddDataToCommandList(data.SubArray(startingIndex, dataSize));
                }

                startingIndex += dataSize;
            }
        }

        private static void AddDataToCommandList(byte[] data)
        {
            lock (commandsToSendOnNextTick)
            {
                commandsToSendOnNextTick.Add(data);
            }
        }

        private static void ConnectionEstablished()
        {
            Console.WriteLine("New connection established.");
        }

        private static void Tick(object sender, EventArgs e)
        {
            lock (commandsToSendOnNextTick)
            {
                float deltaTime = Convert.ToSingle(deltaTimeStopwatch.Elapsed.TotalSeconds);
                deltaTimeStopwatch.Restart();
                worldGrid.UpdateCrops(deltaTime);
                worldGrid.UpdateStructures(deltaTime);

                commandsToSendOnNextTick.Add(Commands.Tick(deltaTime));
                server.Send(commandsToSendOnNextTick.ToByteArray());
                commandsToSendOnNextTick.Clear();
            }
        }
    }
}
