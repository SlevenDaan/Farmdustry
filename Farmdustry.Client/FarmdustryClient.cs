using Farmdustry.Network.Command;
using Farmdustry.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Net;
using Farmdustry.Helper;
using Engine.InputHandler;

namespace Farmdustry.Client
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class FarmdustryClient : Game
    {
        GraphicsDeviceManager Graphics;
        SpriteBatch SpriteBatch;

        private Network.Client client = new Network.Client(25566);
        private byte playerId;

        private static WorldGrid worldGrid = new WorldGrid();

        public FarmdustryClient()
        {
            Graphics = new GraphicsDeviceManager(this);
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

            client.DataReceived += DataReceived;

            client.Connect(IPAddress.Parse("169.254.43.84"));

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
            // Create a new SpriteBatch, which can be used to draw textures.
            SpriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        KeyboardHandler keyboard = new KeyboardHandler();
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            keyboard.Update();

            if (keyboard.IsPressed(Keys.Escape))
            {
                Exit();
            }

            if (keyboard.IsPressed(Keys.A))
            {
                client.Send(Commands.RemoveCrop(playerId, 0, 1));
            }
            if (keyboard.IsPressed(Keys.E))
            {
                client.Send(Commands.AddCrop(playerId, 0, 1, CropType.Carrot));
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

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
