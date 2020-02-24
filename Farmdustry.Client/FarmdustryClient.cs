using Farmdustry.Network.Command;
using Farmdustry.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Net;
using Farmdustry.Helper;
using Engine.InputHandler;
using Engine.Graphics.UI;
using Farmdustry.Client.Graphics;
using Engine.Graphics;
using Farmdustry.Entities;
using Farmdustry.Inventory;

namespace Farmdustry.Client
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class FarmdustryClient : Game
    {
        private readonly GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        private readonly KeyboardHandler keyboard = new KeyboardHandler();
        private readonly MouseHandler mouse = new MouseHandler();

        private readonly Network.Client client = new Network.Client(25566);
        private byte playerId;

        private TextureAtlas soilTextureAtlas;
        private TextureAtlas cropTextureAtlas;
        private TextureAtlas structureTextureAtlas;
        private Texture2D playerTexture;

        private SpriteFont font;

        private WorldGridRenderer worldGridRenderer;

        private readonly WorldGrid worldGrid = new WorldGrid();

        private readonly PlayerList players = new PlayerList();
        private readonly Inventory.Inventory playerInventory = new Inventory.Inventory(Inventory.Inventory.PLAYER_INVENTORY_VOLUME);
        private readonly Hotbar playerHotbar = new Hotbar();

        private readonly ItemDropList itemDrops = new ItemDropList();

        public FarmdustryClient()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            Window.Title = $"Farmdustry";
            Window.AllowUserResizing = true;
            IsMouseVisible = true;

            client.DataReceived += DataReceived;
            client.Connect(Network.Network.GetLocalIp());

            base.Initialize();
        }

        private void DataReceived(byte[] data)
        {
            int startingIndex = 0;
            while (startingIndex < data.Length)
            {
                switch ((CommandType)data[startingIndex + 1])
                {
                    case CommandType.Tick:
                        {
                            float deltaTime = BitConverter.ToSingle(data.SubArray(startingIndex + 2, 4), 0);
                            worldGrid.UpdateCrops(deltaTime);
                            worldGrid.UpdateStructures(deltaTime);
                            break;
                        }

                    case CommandType.AddCrop:
                        {
                            byte y = data[startingIndex + 2];
                            byte x = data[startingIndex + 3];
                            CropType cropType = (CropType)data[startingIndex + 4];
                            worldGrid.AddCrop(y, x, cropType);
                            break;
                        }
                    case CommandType.RemoveCrop:
                        {
                            byte y = data[startingIndex + 2];
                            byte x = data[startingIndex + 3];
                            worldGrid.RemoveCrop(y, x, out _);
                            break;
                        }
                    case CommandType.AddStructure:
                        {
                            byte y = data[startingIndex + 2];
                            byte x = data[startingIndex + 3];
                            StructureType structureType = (StructureType)data[startingIndex + 4];
                            worldGrid.AddStructure(y, x, structureType);
                            break;
                        }
                    case CommandType.RemoveStructure:
                        {
                            byte y = data[startingIndex + 2];
                            byte x = data[startingIndex + 3];
                            worldGrid.RemoveStructure(y, x, out _);
                            break;
                        }
                    case CommandType.AddItemToInventory:
                        {
                            byte playerId = data[startingIndex + 2];
                            if (this.playerId != playerId)
                            {
                                break;
                            }
                            ItemType itemType = (ItemType)data[startingIndex + 3];
                            int amount = BitConverter.ToInt32(data.SubArray(startingIndex + 4, 4), 0);
                            playerInventory.AddItem(itemType, amount);
                            break;
                        }
                    case CommandType.RemoveItemFromInventory:
                        {
                            byte playerId = data[startingIndex + 2];
                            if (this.playerId != playerId)
                            {
                                break;
                            }
                            ItemType itemType = (ItemType)data[startingIndex + 3];
                            int amount = BitConverter.ToInt32(data.SubArray(startingIndex + 4, 4), 0);
                            playerInventory.RemoveItem(itemType, amount);
                            break;
                        }
                    case CommandType.SpawnItemDrop:
                        {
                            float y = BitConverter.ToSingle(data.SubArray(startingIndex + 2, 4), 0);
                            float x = BitConverter.ToSingle(data.SubArray(startingIndex + 6, 4), 0);
                            ItemType itemType = (ItemType)data[startingIndex + 10];
                            byte amount = data[startingIndex + 11];
                            itemDrops.Add(y, x, itemType, amount);
                            break;
                        }
                    case CommandType.RemoveItemDrop:
                        {
                            int index = BitConverter.ToInt32(data.SubArray(startingIndex + 2, 4), 0);
                            itemDrops.Remove(index);
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
                            break;
                        }

                    case CommandType.SetPlayerId:
                        {
                            playerId = data[startingIndex + 2];

                            break;
                        }
                }

                startingIndex += data[startingIndex];
            }
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            font = Content.Load<SpriteFont>("UI/Arial");

            soilTextureAtlas = new TextureAtlas(Content.Load<Texture2D>("SoilAtlas"), 32, 32);
            cropTextureAtlas = new TextureAtlas(Content.Load<Texture2D>("CropAtlas"), 32, 32);
            structureTextureAtlas = new TextureAtlas(Content.Load<Texture2D>("StructureAtlas"), 32, 32);
            worldGridRenderer = new WorldGridRenderer( soilTextureAtlas, cropTextureAtlas, structureTextureAtlas);

            playerTexture = Content.Load<Texture2D>("Player");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {

        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            keyboard.Update();
            mouse.Update();

            { 
                //Update player position
                Vector2 velocity = new Vector2(keyboard.GetAxis(Keys.Q, Keys.D), keyboard.GetAxis(Keys.Z, Keys.S));
                if (velocity.X != 0 && velocity.Y != 0)
                {
                    velocity.Normalize();
                }
                players.SetPlayerVelocity(playerId, velocity.Y, velocity.X);
                players.UpdatePlayers(deltaTime);

                Player player = players.GetPlayerSnapshot(playerId);
                client.Send(Commands.UpdatePlayerLocation(playerId, player.Y, player.X, player.YVelocity, player.XVelocity));
            }

            if (keyboard.IsPressed(Keys.Escape))
            {
                Exit();
            }

            if (keyboard.IsPressed(Keys.D1))
            {
                Player player = players.GetPlayerSnapshot(playerId);
                client.Send(Commands.PlantCrop(playerId, (byte)player.Y, (byte)player.X, CropType.Carrot));
            }
            if (keyboard.IsPressed(Keys.D2))
            {
                Player player = players.GetPlayerSnapshot(playerId);
                client.Send(Commands.HarvestCrop(playerId, (byte)player.Y, (byte)player.X));
            }
            if (keyboard.IsPressed(Keys.D3))
            {
                Player player = players.GetPlayerSnapshot(playerId);
                client.Send(Commands.PlaceStructure(playerId, (byte)player.Y, (byte)player.X, StructureType.Container));
            }
            if (keyboard.IsPressed(Keys.D4))
            {
                Player player = players.GetPlayerSnapshot(playerId);
                client.Send(Commands.DestroyStructure(playerId, (byte)player.Y, (byte)player.X));
            }

            if (mouse.ScrollWheelState != ScrollWheelState.Idle)
            {
                playerHotbar.SelectSlot(playerHotbar.SelectedSlot + (int)mouse.ScrollWheelState);
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(samplerState : SamplerState.PointClamp);

            worldGridRenderer.RenderSoil(spriteBatch, worldGrid);
            worldGridRenderer.RenderCrops(spriteBatch, worldGrid);
            worldGridRenderer.RenderStructures(spriteBatch, worldGrid);

            for (byte i = 0; i < players.Count; i++)
            {
                Player player = players.GetPlayerSnapshot(i);
                spriteBatch.Draw(playerTexture, new Rectangle((int)(player.X * 32 - 16), (int)(player.Y * 32 - 16), 32, 32), Color.White);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
