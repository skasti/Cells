using System;
using System.Dynamic;
using System.IO;
using System.Linq;
using Cells.GameObjects;
using Cells.Genetics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Rectangle = Cells.Geometry.Rectangle;

namespace Cells;

public class Game1 : Game
{
    readonly GraphicsDeviceManager _graphics;
    SpriteBatch _spriteBatch;

    public static int Width = 2500;
    public static int Height = 1200;
    public static Vector2 WindowSize = new Vector2(Width, Height);
    public static Vector2 WorldBounds = new Vector2(25000, 12000);

    private static float _ViewZoom = 1f;
    public static float ViewZoom
    {
        get { return _ViewZoom; }
        set
        {
            _ViewZoom = value;
            View = CalculateView();
        }
    }
    private static Vector2 _ViewPosition = WorldBounds * 0.5f;
    public static Vector2 ViewPosition
    {
        get { return _ViewPosition; }
        set
        {
            _ViewPosition = value;
            View = CalculateView();
        }
    }
    public static Rectangle View { get; private set; } = CalculateView();
    private static Rectangle CalculateView()
    {
        var viewSize = WindowSize / _ViewZoom;
        return new Rectangle(ViewPosition - (viewSize * 0.5f), viewSize);
    }

    public static Vector2 MaxBounds { get; private set; } = new Vector2(Width * 10, Height * 10);
    public static Vector2 RandomPosition()
    {
        return new Vector2(Random.NextSingle() * WorldBounds.X, Random.NextSingle() * WorldBounds.Y);
    }

    public static Texture2D Circle, Virus, Sprint;
    public static SpriteFont Arial;

    public static Random Random;

    private Organism _fittest;

    public static Organism Debug;
    public static Organism Observing { get; private set; }

    public Game1()
    {
        Random = new Random((int)DateTime.Now.Ticks);
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        base.IsFixedTimeStep = false;
        _graphics.SynchronizeWithVerticalRetrace = false;
        //_graphics.IsFullScreen = true;
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
        Arial = Content.Load<SpriteFont>("Arial");

        var numSpawns = 0;
        for (int i = 0; i < 10; i++)
        {
            if (File.Exists("fittest.dna"))
            {
                ObjectManager.Instance.Add(new Organism(new DNA("fittest.dna")));
                numSpawns++;
            }
            //ObjectManager.Instance.Add(new Organism(new DNA("prey.txt")));
            //ObjectManager.Instance.Add(new Organism(new DNA("predator.txt")));
            //ObjectManager.Instance.Add(new Organism(new DNA("predator.txt")));
        }

        if (Directory.Exists("Genomes"))
        {
            var files = Directory.GetFiles("Genomes", "*.dna");

            foreach (var file in files)
            {
                ObjectManager.Instance.Add(new Organism(new DNA(file)));
                numSpawns++;

                if (numSpawns > 75) {
                    break;
                }
            }
        }

        for (int i = numSpawns; i < 100; i++)
        {
            ObjectManager.Instance.Add(new Organism(new DNA(20, 100)));
        }

        for (int i = 0; i < 500; i++)
        {
            ObjectManager.Instance.Add(new Food(RandomPosition(), Random.Next(100, 5000)));
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

        var fittestOrganisms = ObjectManager.Instance.GetObjects<Organism>().OrderByDescending(o => o.Fitness).ToList();

        for (int i = 0; i < fittestOrganisms.Count; i++)
        {
            //File.WriteAllText($"Genomes\\Genome_{i}.txt", fittestOrganisms[i].Capabilities + "\n\n" + fittestOrganisms[i].UpdateCode);
            fittestOrganisms[i].DNA.Save("Genomes\\Genome_" + i + ".dna");
        }
    }

    private float SpawnRate = 0.1f;
    private const float MinSpawnRate = 0.1f;
    private const float MaxSpawnRate = 2f;
    private const float MaxSpawnRateChange = 0.2f;
    private const float SpawnRateChangeRate = 1f;
    float _spawnTime = 1f;
    float _spawnRateChangeTime = 5f;
    float _lastSpawnRateChange = 0f;

    public static float Friction = 0.2f;
    private float DisplayedTimewarp = 1f;
    private float _timeWarpDisplayUpdate = 0.5f;
    private TimeSpan _simulationTime = TimeSpan.FromSeconds(0);
    private int lastScrollPosition = Mouse.GetState().ScrollWheelValue;

    KeyboardState keyboardState = Keyboard.GetState();

    /// <summary>
    /// Allows the game to run logic such as updating the world,
    /// checking for collisions, gathering input, and playing audio.
    /// </summary>
    /// <param name="gameTime">Provides a snapshot of timing values.</param>
    protected override void Update(GameTime gameTime)
    {
        keyboardState = Keyboard.GetState();
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || keyboardState.IsKeyDown(Keys.Escape))
            Exit();

        MouseState mouse = Mouse.GetState();
        if (mouse.LeftButton == ButtonState.Pressed)
        {
            var position = mouse.Position.ToVector2();
            Debug = ObjectManager.Instance.GetObjects<Organism>(position).FirstOrDefault();
        }

        var realDeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        var deltaTime = (float)1f / 60; // (float)gameTime.ElapsedGameTime.TotalSeconds;
        //deltaTime = realDeltaTime;
        _simulationTime += TimeSpan.FromSeconds(deltaTime);
        var timeWarp = deltaTime / realDeltaTime;
        _timeWarpDisplayUpdate -= realDeltaTime;
        if (_timeWarpDisplayUpdate < 0f)
        {
            _timeWarpDisplayUpdate = 0.5f;
            DisplayedTimewarp = timeWarp;
        }

        _spawnTime -= deltaTime;
        _spawnRateChangeTime -= deltaTime;

        var organisms = ObjectManager.Instance.GetObjects<Organism>().OrderByDescending(o => o.Fitness).ToList();
        if (Observing == null || !Observing.Alive)
        {
            Observing = Debug ?? (_fittest?.Alive == true ? _fittest : organisms.FirstOrDefault(o => o.Alive && o.Energy > o.EnergyChangeRate * 10));
        }

        if ((_fittest == null) && (organisms.Count > 1))
        {
            _fittest = organisms[0];
        }
        else if (organisms.Count > 1)
        {
            if (organisms[0].Fitness > _fittest.Fitness)
            {
                _fittest = organisms[0];
                _fittest.DNA.Save("fittest.dna");
            }
        }

        if (_spawnTime < 0f)
        {
            if (ObjectManager.Instance.Count<Food>() < 500)
                ObjectManager.Instance.Add(new Food(RandomPosition(), Random.Next(100, 5000)));

            if (ObjectManager.Instance.Count<Organism>() < 50)
            {
                if (_fittest != null)
                {
                    var mate = organisms.FirstOrDefault(o => o != _fittest && _fittest.DNA.RelatedPercent(o.DNA) > 0.5f && _fittest.DNA.RelatedPercent(o.DNA) < 0.9f);
                    for (var i = 0; i < 10; i++) {
                        if (mate != null)
                            ObjectManager.Instance.Add(new Organism(new DNA(_fittest.DNA, mate.DNA)));

                        ObjectManager.Instance.Add(new Organism(new DNA(_fittest.DNA, new DNA(30, 100))));
                    }
                }
                else
                    ObjectManager.Instance.Add(new Organism(new DNA(30, 100)));
            }

            if (_spawnRateChangeTime <= 0f)
            {
                var change = (Random.NextSingle() * (MaxSpawnRateChange * 2)) - MaxSpawnRateChange;
                change += _lastSpawnRateChange * 0.75f;
                change = Math.Clamp(change, -MaxSpawnRateChange, MaxSpawnRateChange);
                SpawnRate += change;
                _lastSpawnRateChange = change;
                if (SpawnRate > MaxSpawnRate) SpawnRate = MaxSpawnRate;
                else if (SpawnRate < MinSpawnRate) SpawnRate = MinSpawnRate;

                _spawnRateChangeTime = SpawnRateChangeRate;
            }

            //Spawn
            _spawnTime = SpawnRate;
        }

        if (!keyboardState.IsKeyDown(Keys.Space))
            ObjectManager.Instance.Update(deltaTime);

        var scrollPosition = Mouse.GetState().ScrollWheelValue;

        if (scrollPosition > lastScrollPosition)
        {
            ViewZoom = Math.Min(ViewZoom + 0.1f, 1f);
        }
        else if (scrollPosition < lastScrollPosition)
        {
            ViewZoom = Math.Max(ViewZoom - 0.1f, 0.1f);
        }

        lastScrollPosition = scrollPosition;

        if (Observing != null)
        {
            ViewPosition = Observing.Position;
        } else {
            ViewPosition = WorldBounds * 0.5f;
        }

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
        _spriteBatch.Begin();
        _spriteBatch.DrawString(Arial, $"SimTime: T+{_simulationTime:g}", new Vector2(10, 10), Color.White);
        _spriteBatch.DrawString(Arial, $"Warp: {DisplayedTimewarp:0.00}x", new Vector2(10, 30), Color.White);
        _spriteBatch.DrawString(Arial, $"SpawnRate: {SpawnRate}", new Vector2(10, 50), Color.White);
        _spriteBatch.DrawString(Arial, $"Fittest: {_fittest.Fitness}", new Vector2(10, 70), Color.White);
        _spriteBatch.DrawString(Arial, $"Organisms: {ObjectManager.Instance.Count<Organism>()}", new Vector2(10, 90), Color.White);

        if (Observing != null && keyboardState.IsKeyDown(Keys.O))
        {
            _spriteBatch.DrawString(Arial, $"Energy: {Observing.Energy}", new Vector2(10, 120), Color.White);
            _spriteBatch.DrawString(Arial, $"Force: {Observing.Force.Length():0.00} {Observing.Force.ToShortString()}", new Vector2(10, 140), Color.White);
            _spriteBatch.DrawString(Arial, $"Age: {Observing.Age}", new Vector2(10, 160), Color.White);
            _spriteBatch.DrawString(Arial, $"Fitness: {Observing.Fitness}", new Vector2(10, 180), Color.White);

            _spriteBatch.DrawString(Arial, $"EnergyGiven: {Observing.EnergyGiven}", new Vector2(10, 210), Color.White);
            _spriteBatch.DrawString(Arial, $"EnergyConsumption: {Observing.EnergyConsumption}", new Vector2(10, 230), Color.White);
            _spriteBatch.DrawString(Arial, $"EnergyChangeRate: {Observing.EnergyChangeRate}", new Vector2(10, 250), Color.White);

            _spriteBatch.DrawString(Arial, $"Status: {Observing.Status}", new Vector2(10, 280), Color.White);
            _spriteBatch.DrawString(Arial, $"Update Log:\n{String.Join("\n",Observing.UpdateLog)}", new Vector2(10, 300), Color.White);
            _spriteBatch.DrawString(Arial, $"Collision Log:\n{String.Join("\n",Observing.CollisionLog)}", new Vector2(1000, 10), Color.White);
        }
        else if (keyboardState.IsKeyDown(Keys.D))
        {
            _spriteBatch.DrawString(Arial, $"Update Time: {ObjectManager.Instance.UpdateStopwatch.Elapsed}", new Vector2(10, 120), Color.White);
            _spriteBatch.DrawString(Arial, $"Collision Time: {ObjectManager.Instance.CollisionStopwatch.Elapsed}", new Vector2(10, 140), Color.White);
            _spriteBatch.DrawString(Arial, $"Draw Time: {ObjectManager.Instance.DrawStopwatch.Elapsed}", new Vector2(10, 160), Color.White);
        }

        _spriteBatch.End();
        base.Draw(gameTime);
    }
}
