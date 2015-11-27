using Cells.GameObjects;
using Cells.Genetics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cells
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public static int Width = 1280;
        public static int Height = 720;

        public static Texture2D circle, virus, sprint;

        public static Random r;

        public Game1()
        {
            r = new Random((int)DateTime.Now.Ticks);
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
            graphics.PreferredBackBufferWidth = Width;  // set this value to the desired width of your window
            graphics.PreferredBackBufferHeight = Height;   // set this value to the desired height of your window
            graphics.IsFullScreen = false;
            graphics.ApplyChanges();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            circle = Content.Load<Texture2D>("circle");
            virus = Content.Load<Texture2D>("virus");
            sprint = Content.Load<Texture2D>("sprint");

            for (int i = 0; i < 100; i++)
            {
                ObjectManager.Instance.Add(new Organism(new DNA(50, 200)));
            }

            for (int i = 0; i < 100; i++)
            {
                ObjectManager.Instance.Add(new Food(new Vector2(r.Next(Width), r.Next(Height)), r.Next(10, 100)));
            }

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

        float spawnRate = 5f;
        float spawnTime = 1f;

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            spawnTime -= deltaTime;

            if (spawnTime < 0f)
            {
                ObjectManager.Instance.Add(new Food(new Vector2(r.Next(Width), r.Next(Height)), r.Next(10, 100)));
                //Spawn
                spawnTime = spawnRate;
            }

            ObjectManager.Instance.Update(deltaTime);
            
            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            ObjectManager.Instance.Draw(spriteBatch);

            base.Draw(gameTime);
        }
    }
}
