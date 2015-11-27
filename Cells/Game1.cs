using Cells.GameObjects;
using Cells.Genetics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Cells
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        readonly GraphicsDeviceManager _graphics;
        SpriteBatch _spriteBatch;

        public static int Width = 1280;
        public static int Height = 720;

        public static Texture2D Circle, Virus, Sprint;

        public static Random Random;

        public Game1()
        {
            Random = new Random((int)DateTime.Now.Ticks);
            _graphics = new GraphicsDeviceManager(this);
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
            _graphics.PreferredBackBufferWidth = Width;  // set this value to the desired width of your window
            _graphics.PreferredBackBufferHeight = Height;   // set this value to the desired height of your window
            _graphics.IsFullScreen = false;
            _graphics.ApplyChanges();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            Circle = Content.Load<Texture2D>("circle");
            Virus = Content.Load<Texture2D>("virus");
            Sprint = Content.Load<Texture2D>("sprint");

            for (int i = 0; i < 50; i++)
            {
                ObjectManager.Instance.Add(new Organism(new DNA(50, 200)));
            }

            for (int i = 0; i < 100; i++)
            {
                ObjectManager.Instance.Add(new Food(new Vector2(Random.Next(Width), Random.Next(Height)), Random.Next(10, 100)));
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

        private const float SpawnRate = 10f;
        float _spawnTime = 1f;

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            _spawnTime -= deltaTime;

            if (_spawnTime < 0f)
            {
                if (ObjectManager.Instance.Count<Food>() < 100)
                    ObjectManager.Instance.Add(new Food(new Vector2(Random.Next(Width), Random.Next(Height)), Random.Next(10, 100)));
                
                if (ObjectManager.Instance.Count<Organism>() < 50)
                    ObjectManager.Instance.Add(new Organism(new DNA(50, 200)));
                //Spawn
                _spawnTime = SpawnRate;
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

            ObjectManager.Instance.Draw(_spriteBatch);

            base.Draw(gameTime);
        }
    }
}
