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
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        private KeyboardHandler keyboard = new KeyboardHandler();
        private MouseHandler mouse = new MouseHandler();

        private Network.Client client = new Network.Client(25566);
        private byte playerId;

        private UILayer ui;

        private TextureAtlas soilTextureAtlas;
        private TextureAtlas cropTextureAtlas;
        private TextureAtlas structureTextureAtlas;
        private WorldGridRenderer worldGridRenderer;

        private WorldGrid worldGrid = new WorldGrid();

        private PlayerList players = new PlayerList();
        private Inventory.Inventory playerInventory = new Inventory.Inventory(Inventory.Inventory.PLAYER_INVENTORY_VOLUME);
        private Texture2D playerTexture;

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
                            byte y = data[startingIndex + 3];
                            byte x = data[startingIndex + 4];
                            CropType cropType = (CropType)data[startingIndex + 5];
                            worldGrid.AddCrop(y, x, cropType);
                            break;
                        }
                    case CommandType.RemoveCrop:
                        {
                            byte y = data[startingIndex + 3];
                            byte x = data[startingIndex + 4];
                            worldGrid.RemoveCrop(y, x, out _);
                            break;
                        }
                    case CommandType.AddStructure:
                        {
                            byte y = data[startingIndex + 3];
                            byte x = data[startingIndex + 4];
                            StructureType structureType = (StructureType)data[startingIndex + 5];
                            worldGrid.AddStructure(y, x, structureType);
                            break;
                        }
                    case CommandType.RemoveStructure:
                        {
                            byte y = data[startingIndex + 3];
                            byte x = data[startingIndex + 4];
                            worldGrid.RemoveStructure(y, x, out _);
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
                    case CommandType.AddItemToInventory:
                        {
                            byte playerId = data[startingIndex + 2];
                            if(this.playerId != playerId)
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

            ui = new UILayer(Content.Load<SpriteFont>("UI/Arial"), Window);

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

            ui.Update(mouse);

            {
                float yVelocity = keyboard.GetAxis(Keys.Z, Keys.S);
                float xVelocity = keyboard.GetAxis(Keys.Q, Keys.D);
                players.SetPlayerVelocity(playerId, yVelocity, xVelocity);
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
                client.Send(Commands.AddCrop(playerId, (byte)player.Y, (byte)player.X, CropType.Carrot));
            }
            if (keyboard.IsPressed(Keys.D2))
            {
                Player player = players.GetPlayerSnapshot(playerId);
                client.Send(Commands.RemoveCrop(playerId, (byte)player.Y, (byte)player.X));
            }
            if (keyboard.IsPressed(Keys.D3))
            {
                Player player = players.GetPlayerSnapshot(playerId);
                client.Send(Commands.AddStructure(playerId, (byte)player.Y, (byte)player.X, StructureType.Container));
            }
            if (keyboard.IsPressed(Keys.D4))
            {
                Player player = players.GetPlayerSnapshot(playerId);
                client.Send(Commands.RemoveStructure(playerId, (byte)player.Y, (byte)player.X));
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

            ui.Draw(spriteBatch);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
