using Farmdustry.Network.Command;
using Farmdustry.World;
using System;
using System.Diagnostics;
using System.Timers;
using Farmdustry.Helper;
using Farmdustry.Inventory;
using Farmdustry.Entities;
using System.Collections;

namespace Farmdustry.Server
{
    class Server
    {
        private const int TICK_RATE = 10;
        private static Timer timer;
        private static readonly Stopwatch deltaTimeStopwatch = new Stopwatch();

        private static readonly Network.Server server = new Network.Server(25566);
        private static readonly CommandList commandsToSendOnNextTick = new CommandList();

        private static readonly WorldGrid worldGrid = new WorldGrid();

        private static readonly PlayerList players = new PlayerList();
        private static readonly InventoryList inventories = new InventoryList();

        private static readonly ItemDropList itemDrops = new ItemDropList();

        static void Main(string[] arguments)
        {
            int port = Convert.ToInt32(arguments[0]);

            Console.Title = $"Farmdustry Server {Network.Network.GetLocalIp().ToString()}:{port}";

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
            //Console.WriteLine($"Data received Length: {data.Length}");

            int startingIndex = 0;
            while (startingIndex < data.Length)
            {
                switch ((CommandType)data[startingIndex + 1])
                {
                    case CommandType.PlantCrop:
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
                                worldGrid.AddCrop(y, x, cropType);
                                AddDataToCommandList(Commands.RemoveItemFromInventory(playerId, cropItemType, 1));
                                AddDataToCommandList(Commands.AddCrop(y,x,cropType));
                            }
                            break;
                        }
                    case CommandType.HarvestCrop:
                        {
                            byte playerId = data[startingIndex + 2];
                            byte y = data[startingIndex + 3];
                            byte x = data[startingIndex + 4];

                            if(worldGrid.RemoveCrop(y, x, out Crop crop))
                            {
                                AddDataToCommandList(Commands.RemoveCrop(y, x));

                                inventories.GetInventory(playerId, out Inventory.Inventory inventory);
                                ItemType cropItemType = (ItemType)((int)crop.Type + (50 * Convert.ToInt32(crop.Growth != 1))); //Convert crop to crop seed or item

                                //Check if player has enough space in their inventory
                                if (inventory.AddItem(cropItemType, 1))
                                {
                                    AddDataToCommandList(Commands.AddItemToInventory(playerId, cropItemType, 1));
                                }
                                else
                                {
                                    AddDataToCommandList(Commands.SpawnItemDrop(y, x, cropItemType, 1));
                                }
                            }
                            break;
                        }
                    case CommandType.PlaceStructure:
                        {
                            byte playerId = data[startingIndex + 2];
                            byte y = data[startingIndex + 3];
                            byte x = data[startingIndex + 4];
                            StructureType structureType = (StructureType)data[startingIndex + 5];

                            //TODO: Check if player has enough items

                            worldGrid.AddStructure(y, x, structureType);
                            AddDataToCommandList(Commands.AddStructure(y, x, structureType));
                            break;
                        }
                    case CommandType.DestroyStructure:
                        {
                            byte playerId = data[startingIndex + 2];
                            byte y = data[startingIndex + 3];
                            byte x = data[startingIndex + 4];

                            worldGrid.RemoveStructure(y, x, out Structure structure);
                            AddDataToCommandList(Commands.RemoveStructure(y, x));

                            //TODO: Give back item to player
                            break;
                        }
                    case CommandType.DropItem:
                        {
                            byte playerId = data[startingIndex + 2];
                            float y = BitConverter.ToSingle(data.SubArray(startingIndex + 3, 4), 0);
                            float x = BitConverter.ToSingle(data.SubArray(startingIndex + 7, 4), 0);
                            ItemType itemType = (ItemType)data[startingIndex + 11];
                            int amount = BitConverter.ToInt32(data.SubArray(startingIndex + 12, 4), 0);

                            inventories.GetInventory(playerId, out Inventory.Inventory inventory);

                            if (inventory.RemoveItem(itemType, amount))
                            {
                                itemDrops.Add(y, x, itemType, amount);
                                AddDataToCommandList(Commands.RemoveItemFromInventory(playerId, itemType, amount));
                                AddDataToCommandList(Commands.SpawnItemDrop(y, x, itemType, amount));
                            }
                            break;
                        }
                    case CommandType.PickupItem:
                        {
                            byte playerId = data[startingIndex + 2];
                            float y = BitConverter.ToSingle(data.SubArray(startingIndex + 3, 4), 0);
                            float x = BitConverter.ToSingle(data.SubArray(startingIndex + 7, 4), 0);

                            inventories.GetInventory(playerId, out Inventory.Inventory inventory);
                            IList itemDropsInRange = itemDrops.GetInRange(y, x, 1);

                            //Add as many items in range to the player inventory
                            foreach (int itemDropId in itemDropsInRange)
                            {
                                ItemDrop itemDrop = itemDrops.GetItemDropSnapshot(itemDropId);
                                if (inventory.AddItem(itemDrop.Item, itemDrop.Amount))
                                {
                                    AddDataToCommandList(Commands.AddItemToInventory(playerId, itemDrop.Item, itemDrop.Amount));
                                }
                            }

                            break;
                        }
                    case CommandType.UpdatePlayerLocation:
                        {
                            byte playerId = data[startingIndex + 2];
                            float y = BitConverter.ToSingle(data.SubArray(startingIndex + 3, 4), 0);
                            float x = BitConverter.ToSingle(data.SubArray(startingIndex + 7, 4), 0);
                            float yVelocity = BitConverter.ToSingle(data.SubArray(startingIndex + 11, 4), 0);
                            float xVelocity = BitConverter.ToSingle(data.SubArray(startingIndex + 15, 4), 0);

                            players.SetPlayerPositionAndVelocity(playerId, y, x, yVelocity, xVelocity);
                            AddDataToCommandList(Commands.UpdatePlayerLocation(playerId, y, x, yVelocity, xVelocity));
                            //TODO Collision check
                            break;
                        }
                }

                byte dataSize = data[startingIndex];
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
