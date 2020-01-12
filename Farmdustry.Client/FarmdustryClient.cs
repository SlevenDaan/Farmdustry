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

        //Temporary player stuff
        private Player player = new Player();
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
                            if(this.playerId != playerId)
                            {
                                break;
                            }
                            player.Y = BitConverter.ToSingle(data.SubArray(startingIndex + 3, 4), 0);
                            player.X = BitConverter.ToSingle(data.SubArray(startingIndex + 7, 4), 0);
                            player.YVelocity = BitConverter.ToSingle(data.SubArray(startingIndex + 11, 4), 0);
                            player.XVelocity = BitConverter.ToSingle(data.SubArray(startingIndex + 15, 4), 0);
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

            player.YVelocity = keyboard.GetAxis(Keys.Z, Keys.S);
            player.XVelocity = keyboard.GetAxis(Keys.Q, Keys.D);
            player.X += player.XVelocity * deltaTime * 5;
            player.Y += player.YVelocity * deltaTime * 5;

            if (keyboard.IsPressed(Keys.Escape))
            {
                Exit();
            }

            if (keyboard.IsPressed(Keys.D1))
            {
                client.Send(Commands.AddCrop(playerId, (byte)player.Y, (byte)player.X, CropType.Carrot));
            }
            if (keyboard.IsPressed(Keys.D2))
            {
                client.Send(Commands.RemoveCrop(playerId, (byte)player.Y, (byte)player.X));
            }
            if (keyboard.IsPressed(Keys.D3))
            {
                client.Send(Commands.AddStructure(playerId, (byte)player.Y, (byte)player.X, StructureType.Container));
            }
            if (keyboard.IsPressed(Keys.D4))
            {
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
            spriteBatch.Draw(playerTexture, new Rectangle((int)(player.X * 32 - 16), (int)(player.Y * 32 - 16), 32, 32), Color.White);
            ui.Draw(spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
