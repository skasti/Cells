using System.Linq;
using Cells.GameObjects;
using Cells.Genetics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.IO;

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

        private Organism _fittest;

        public static Organism Debug;

        public Game1()
        {
            Random = new Random((int)DateTime.Now.Ticks);
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
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
            GeneInterpreter.CheckMakerIntegrity();

            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            Circle = Content.Load<Texture2D>("circle");
            Virus = Content.Load<Texture2D>("virus");
            Sprint = Content.Load<Texture2D>("sprint");

            for (int i = 0; i < 2; i++)
            {
                ObjectManager.Instance.Add(new Organism(new DNA("prey.txt")));
                ObjectManager.Instance.Add(new Organism(new DNA("predator.txt")));
                ObjectManager.Instance.Add(new Organism(new DNA("predator.txt")));
            }

            for (int i = 0; i < 2; i++)
            {
                ObjectManager.Instance.Add(new Organism(new DNA(20, 60)));
            }

            for (int i = 0; i < 50; i++)
            {
                ObjectManager.Instance.Add(new Food(new Vector2(Random.Next(Width), Random.Next(Height)), Random.Next(100, 500)));
            }

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            if (!Directory.Exists("Genomes"))
                Directory.CreateDirectory("Genomes");

            var genomes = ObjectManager.Instance.GetObjects<Organism>().OrderByDescending(o => (o.EnergyGiven + o.DistanceMoved)).Select(o => o.DNA).ToList();

            for (int i = 0; i < genomes.Count; i++)
            {
                genomes[i].Save("Genomes\\Genome_" + i + ".dna");
            }
        }

        private const float SpawnRate = 0.01f;
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

            MouseState mouse = Mouse.GetState();
            if (mouse.LeftButton == ButtonState.Pressed)
            {
                var position = mouse.Position.ToVector2();
                Debug = ObjectManager.Instance.GetObjects<Organism>(position).FirstOrDefault();
            }

            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            _spawnTime -= deltaTime;

            var organisms = ObjectManager.Instance.GetObjects<Organism>().OrderByDescending(o => (o.EnergyGiven + o.DistanceMoved)).ToList();

            if ((_fittest == null) && (organisms.Count > 1))
            {
                _fittest = organisms[0];
            }
            else if (organisms.Count > 1)
            {
                if (organisms[0].EnergyGiven + organisms[0].DistanceMoved > _fittest.EnergyGiven + _fittest.DistanceMoved)
                {
                    _fittest = organisms[0];
                    _fittest.DNA.Save("fittest.dna");
                }
            }

            if (_spawnTime < 0f)
            {
                if (ObjectManager.Instance.Count<Food>() < 150)
                    ObjectManager.Instance.Add(new Food(new Vector2(Random.Next(Width), Random.Next(Height)), Random.Next(100, 500)));

                if (ObjectManager.Instance.Count<Organism>() < 10)
                {
                    var dna = new DNA(30,100);
                    
                    if (_fittest != null)
                    {
                        var mate = organisms.FirstOrDefault(o => _fittest.DNA.RelatedPercent(o.DNA) > 0.5f);
                        dna = new DNA(_fittest.DNA, mate != null ? mate.DNA : dna);
                    }

                    ObjectManager.Instance.Add(new Organism(dna));
                }
                //Spawn
                _spawnTime = SpawnRate;
            }

            if (!Keyboard.GetState().IsKeyDown(Keys.Space))
                ObjectManager.Instance.Update(deltaTime);

            if (Debug != null)
                Window.Title = "Debugging: " + Debug.Position;
            else if (_fittest != null)
                Window.Title = "Fittest: " + (_fittest.EnergyGiven + _fittest.DistanceMoved);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            ObjectManager.Instance.Draw(_spriteBatch);

            base.Draw(gameTime);
        }
    }
}
