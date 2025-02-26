using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using Cells.GameObjects;
using Cells.Genetics;
using Cells.Genetics.Sequences;
using Cells2.Events;
using Cells2.Genetics.Sequences;
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
    public static Vector2 WorldSize = new Vector2(25000, 12000);
    public static Rectangle WorldBounds = new Rectangle(Vector2.Zero, WorldSize);

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
    private static Vector2 _ViewPosition = WorldSize * 0.5f;
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
        return RandomPosition(WorldBounds);
    }

    public static Vector2 RandomPosition(Rectangle bounds)
    {
        return bounds.Position + new Vector2(Random.NextSingle() * bounds.Width, Random.NextSingle() * bounds.Height);
    }

    public static Texture2D Circle, Virus, Sprint;
    public static SpriteFont Arial, ArialSmall;

    public static Random Random;

    private Organism _fittest;
    private Organism _fittestAlive;
    private List<Organism> _hallOfFame = new List<Organism>();
    public static Organism Observing { get; private set; }
    public static bool AutoObserve { get; private set; } = true;
    public static bool AutoArmor { get; private set; }
    public static GraphicsDevice PublicGraphicsDevice { get; private set; }

    public static DisplayMode CurrentDisplayMode { get; private set; } = DisplayMode.Full;
    public enum DisplayMode
    {
        Minimal,
        DebugInfo,
        Stats,
        Full
    }

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
        PublicGraphicsDevice = GraphicsDevice;
        Circle = Content.Load<Texture2D>("circle");
        Virus = Content.Load<Texture2D>("virus");
        Sprint = Content.Load<Texture2D>("sprint");
        Arial = Content.Load<SpriteFont>("Arial");
        ArialSmall = Content.Load<SpriteFont>("Arial Small");

        var numSpawns = 0;

        if (Directory.Exists("Genomes"))
        {
            var hofFiles = Directory.GetFiles("Genomes", "HOF*.dna");

            foreach (var file in hofFiles)
            {
                var dna = new DNA(file);
                for (int i = 0; i < 10; i++)
                {
                    ObjectManager.Instance.Add(new Organism(dna));
                    numSpawns++;
                }
            }

            var files = Directory.GetFiles("Genomes", "Genome_*.dna");

            foreach (var file in files)
            {
                var dna = new DNA(file);
                for (int i = 0; i < 10; i++)
                {
                    ObjectManager.Instance.Add(new Organism(dna));
                    numSpawns++;
                }

                if (numSpawns > 200)
                {
                    break;
                }
            }
        }

        for (int i = 0; i < 20; i++)
        {
            if (File.Exists("fittest.dna"))
            {
                _fittest = new Organism(new DNA("fittest.dna"));
                ObjectManager.Instance.Add(_fittest);
                numSpawns++;
            }
            ObjectManager.Instance.Add(new Organism(new DNA("prey.txt")));
            numSpawns++;
            ObjectManager.Instance.Add(new Organism(new DNA("predator.txt")));
            numSpawns++;
            ObjectManager.Instance.Add(new Organism(new DNA("predator.txt")));
            numSpawns++;
            ObjectManager.Instance.Add(new Organism(new DNA(
                new Breeding(),
                new Forage(),
                new Hunting()
            )));
        }

        for (int i = numSpawns; i < 50; i++)
        {
            ObjectManager.Instance.Add(new Organism(new DNA(20, 100)));
        }

        for (int i = 0; i < 150; i++)
        {
            ObjectManager.Instance.Add(new Food(RandomPosition(WorldBounds), Random.Next(100, 1000)));
            ObjectManager.Instance.Add(new Food(RandomPosition(WorldBounds.Shrunk(0.5f)), Random.Next(100, 1000)));
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

        for (int i = 0; i < _hallOfFame.Count; i++)
        {
            _hallOfFame[i].DNA.Save("Genomes\\HOF_" + i + ".dna");
        }
    }

    private float SpawnRate = 5f;
    private const float MinSpawnRate = 0.2f;
    private const float MaxSpawnRate = 10f;
    private const float MaxSpawnRateChange = 0.2f;
    private const float SpawnRateChangeRate = 1f;
    float _spawnTime = 1f;
    float _spawnRateChangeTime = 5f;
    float _lastSpawnRateChange = 0f;

    public static float Friction = 0.95f;
    public static float DisplayedTimewarp = 1f;
    private float _timeWarpDisplayUpdate = 0.5f;
    private float _updateTime = 1.0f;
    private TimeSpan _simulationTime = TimeSpan.FromSeconds(0);
    private int lastScrollPosition = Mouse.GetState().ScrollWheelValue;

    private bool spawnEnabled = true;

    KeyboardState keyboardState = Keyboard.GetState();
    KeyboardState previousKeyboardState = Keyboard.GetState();
    MouseState mouseState = Mouse.GetState();
    MouseState previousMouseState = Mouse.GetState();

    /// <summary>
    /// Allows the game to run logic such as updating the world,
    /// checking for collisions, gathering input, and playing audio.
    /// </summary>
    /// <param name="gameTime">Provides a snapshot of timing values.</param>
    protected override void Update(GameTime gameTime)
    {
        GeneExtensions.LogStopwatch.Reset();
        previousKeyboardState = keyboardState;
        keyboardState = Keyboard.GetState();
        previousMouseState = mouseState;
        mouseState = Mouse.GetState();

        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || keyboardState.IsKeyDown(Keys.Escape))
            Exit();

        if (mouseState.IsClicked(previousMouseState, MouseButton.Left))
        {
            var mousePosition = mouseState.Position.ToVector2();
            var worldPositon = View.Position + mousePosition / ViewZoom;
            Observing = ObjectManager.Instance.GetObjects<Organism>(worldPositon).FirstOrDefault();
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
        _fittestAlive = organisms.FirstOrDefault(o => o.Alive);

        //_hallOfFame = organisms.Take(20).ToList();

        if (keyboardState.IsKeyDown(Keys.LeftControl))
        {
            if (keyboardState.IsKeyPressed(previousKeyboardState, Keys.PageUp))
            {
                AutoObserve = !AutoObserve;
                if (AutoObserve)
                    AutoArmor = false;
            }
            else if (keyboardState.IsKeyPressed(previousKeyboardState, Keys.A))
            {
                AutoArmor = !AutoArmor;
                if (AutoArmor)
                    AutoObserve = false;
            }
            else if (keyboardState.IsKeyPressed(previousKeyboardState, Keys.M))
                CurrentDisplayMode = DisplayMode.Minimal;
            else if (keyboardState.IsKeyPressed(previousKeyboardState, Keys.D))
                CurrentDisplayMode = DisplayMode.DebugInfo;
            else if (keyboardState.IsKeyPressed(previousKeyboardState, Keys.S))
                CurrentDisplayMode = DisplayMode.Stats;
            else if (keyboardState.IsKeyPressed(previousKeyboardState, Keys.F))
                CurrentDisplayMode = DisplayMode.Full;
            else if (keyboardState.IsKeyPressed(previousKeyboardState, Keys.R))
                spawnEnabled = !spawnEnabled;
        }
        else if (keyboardState.IsKeyPressed(previousKeyboardState, Keys.PageUp))
            Observing = _fittest?.Alive == true ? _fittest : organisms.FirstOrDefault(o => o.Alive && o.Energy > o.EnergyChangeRate * 10);
        else if (keyboardState.IsKeyPressed(previousKeyboardState, Keys.Tab))
            Observing = organisms[Random.Next(organisms.Count-1)];
        else if (keyboardState.IsKeyPressed(previousKeyboardState, Keys.A))
            Observing = organisms.FirstOrDefault(o => o.Alive && o.Energy > o.EnergyChangeRate * 10 && o.Armor > 0);

        if ((_fittest == null) && (organisms.Count >= 1))
        {
            _fittest = organisms[0];
        }
        else if (organisms.Count >= 1)
        {
            if (_fittestAlive != _fittest && _fittestAlive.Fitness > _fittest.Fitness)
            {
                _fittest = _fittestAlive;
                _fittest.DNA.Save("fittest.dna");
                if (!_hallOfFame.Contains(_fittestAlive))
                {
                    _hallOfFame.Insert(0, _fittestAlive);
                    if (_hallOfFame.Count > 20)
                    {
                        var worst = _hallOfFame.OrderBy(o => o.Fitness).First();
                        _hallOfFame.Remove(worst);
                    }
                }
            }
        }

        if (_spawnTime < 0f)
        {
            if (ObjectManager.Instance.Count<Food>() < 500)
            {
                for (var i = 0; i < 10; i++)
                    ObjectManager.Instance.Add(new Food(RandomPosition(WorldBounds.Shrunk(0.5f)), Random.Next(100, 1000)));
            }

            if (spawnEnabled && organisms.Count == 0)
            {
                // if (_fittest != null)
                // {
                var spawned = 0;
                for (var i = 0; i < 10; i++)
                {
                    ObjectManager.Instance.Add(new Organism(_fittest.DNA));
                    spawned++;
                    _hallOfFame.ForEach(o => {
                        ObjectManager.Instance.Add(
                            new Organism(
                                new DNA(
                                    DNA.HighMutation,
                                    _fittest.DNA,
                                    o.DNA
                                )
                            ));
                        spawned++;
                    });
                }
                    // var mate = organisms.FirstOrDefault(o => o != _fittest && _fittest.DNA.RelatedPercent(o.DNA) > 0.5f && _fittest.DNA.RelatedPercent(o.DNA) < 0.95f);
                    // if (mate != null)
                    //         ObjectManager.Instance.Add(new Organism(new DNA(DNA.HighMutation, _fittest.DNA, mate.DNA)));
                // }
                // else
                // {
                    for (var i = spawned; i < 200; i++)
                        ObjectManager.Instance.Add(new Organism(new DNA(50, 150)));
                // }
            }

            if (_spawnRateChangeTime <= 0f)
            {
                var change = (Random.NextSingle() * (MaxSpawnRateChange * 2)) - MaxSpawnRateChange;
                if (timeWarp < 1) {
                    change = Math.Abs(change);
                }
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

        if (AutoObserve && (Observing == null || Observing.Dead))
            Observing = _fittest?.Alive == true ? _fittest : organisms.FirstOrDefault(o => o.Alive && o.Energy > o.EnergyChangeRate * 10);
        else if (AutoArmor && (Observing == null || Observing.Dead))
            Observing = organisms.FirstOrDefault(o => o.Alive && o.Energy > o.EnergyChangeRate * 10 && o.Armor > 0);

        if (Observing != null)
            ViewPosition = Observing.Position;
        else
        {
            var viewSpeed = 300f / ViewZoom;
            if (keyboardState.IsKeyDown(Keys.Left))
                ViewPosition += new Vector2(-viewSpeed * realDeltaTime, 0);
            else if (keyboardState.IsKeyDown(Keys.Right))
                ViewPosition += new Vector2(viewSpeed * realDeltaTime, 0);
            else if (keyboardState.IsKeyDown(Keys.Up))
                ViewPosition += new Vector2(0, -viewSpeed * realDeltaTime);
            else if (keyboardState.IsKeyDown(Keys.Down))
                ViewPosition += new Vector2(0, viewSpeed * realDeltaTime);
        }

        base.Update(gameTime);
    }

    /// <summary>
    /// This is called when the game should draw itself.
    /// </summary>
    /// <param name="gameTime">Provides a snapshot of timing values.</param>
    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.SetRenderTarget(null);
        GraphicsDevice.Clear(Color.CornflowerBlue);
        ObjectManager.Instance.Draw(_spriteBatch);
        _spriteBatch.Begin();
        _spriteBatch.LinePosition(10, 10);
        _spriteBatch.DrawLine(Arial, $"SimTime: T+{_simulationTime:g}", Color.White);
        _spriteBatch.DrawLine(Arial, $"Warp: {DisplayedTimewarp:0.00}x", Color.White);
        _spriteBatch.DrawLine(Arial, $"SpawnRate: {SpawnRate}", Color.White);
        _spriteBatch.DrawLine(Arial, $"Fittest: {_fittestAlive?.Fitness ?? 0.0f}", Color.White);
        _spriteBatch.DrawLine(Arial, $"All Time Fittest: {_fittest?.Fitness ?? 0.0f}", Color.White);
        _spriteBatch.DrawLine(Arial, $"Organisms: {ObjectManager.Instance.Count<Organism>()}", Color.White, 10);

        switch (CurrentDisplayMode)
        {
            case DisplayMode.Minimal:
                if (keyboardState.IsKeyDown(Keys.O))
                    DisplayObserving(true);
                else if (keyboardState.IsKeyDown(Keys.D))
                    DisplayDebugInfo();
                break;

            case DisplayMode.DebugInfo:
                DisplayDebugInfo();
                break;

            case DisplayMode.Stats:
                if (keyboardState.IsKeyDown(Keys.O))
                    DisplayObserving(true);
                else
                    DisplayObserving(false);
                break;

            case DisplayMode.Full:
                DisplayObserving(true);
                break;
        }

        DisplayHallOfFame();

        _spriteBatch.End();
        base.Draw(gameTime);
    }

    private void DisplayObserving(bool displayLogs)
    {
        if (Observing == null)
            return;

        if (displayLogs)
            _spriteBatch.DrawLine(ArialSmall, $"Update Log:\n{String.Join("\n", Observing.UpdateLog)}", Color.White);
        _spriteBatch.LinePosition(1000, 10);
        _spriteBatch.DrawLine(Arial, $"HP/Armor: {Observing.HP:0.0}/{Observing.Armor:0.0}", Color.White);
        _spriteBatch.DrawLine(Arial, $"Status: {Observing.Status}", Color.White);
        _spriteBatch.DrawLine(Arial, $"MetabolicRate: {Observing.BaseMetabolicRate}/{Observing.MovementMetabolicRate}", Color.White, 5);
        _spriteBatch.DrawLine(Arial, $"Energy: {Observing.Energy}", Color.White);
        _spriteBatch.DrawLine(Arial, $"Force: {Observing.Force.Length():0.00} {Observing.Force.ToShortString()}", Color.White);
        _spriteBatch.DrawLine(Arial, $"Age: {Observing.Age}", Color.White);
        _spriteBatch.DrawLine(Arial, $"Fitness: {Observing.Fitness}", Color.White);
        _spriteBatch.DrawLine(Arial, $"Children: {Observing.Children.Count}", Color.White, 10);

        _spriteBatch.DrawLine(Arial, $"EnergyGiven: {Observing.EnergyGiven}", Color.White);
        _spriteBatch.DrawLine(Arial, $"EnergyConsumption: {Observing.EnergyConsumption}", Color.White);
        _spriteBatch.DrawLine(Arial, $"EnergyChangeRate: {Observing.EnergyChangeRate}", Color.White, 10);

        if (displayLogs)
            _spriteBatch.DrawLine(ArialSmall, $"Collision Log:\n{String.Join("\n", Observing.CollisionLog)}", Color.White);
    }

    private void DisplayDebugInfo()
    {
        _spriteBatch.DrawLine(Arial, $"Update Time: {ObjectManager.Instance.UpdateStopwatch.Elapsed.TotalMilliseconds / DisplayedTimewarp}", Color.White);
        _spriteBatch.DrawLine(Arial, $"Collision Time: {ObjectManager.Instance.CollisionStopwatch.Elapsed.TotalMilliseconds / DisplayedTimewarp}", Color.White);
        _spriteBatch.DrawLine(Arial, $"Draw Time: {ObjectManager.Instance.DrawStopwatch.Elapsed.TotalMilliseconds / DisplayedTimewarp}", Color.White);
        _spriteBatch.DrawLine(Arial, $"Find Time: {ObjectManager.Instance.FindStopWatch.Elapsed.TotalMilliseconds / DisplayedTimewarp}", Color.White);
        _spriteBatch.DrawLine(Arial, $"Log Time: {GeneExtensions.LogStopwatch.Elapsed.TotalMilliseconds / DisplayedTimewarp}", Color.White, 10);
        _spriteBatch.DrawLine(Arial, $"Game Objects: {ObjectManager.Instance.Count()}", Color.White);
        _spriteBatch.DrawLine(Arial, $"SearchTree Objects: {ObjectManager.Instance.SearchTree.ObjectCount}", Color.White, 10);

        if (EventCollector.Events.Keys.Count > 0)
            _spriteBatch.DrawLine(Arial, $"Events:", Color.White);

        foreach (var type in EventCollector.Events.Keys)
            _spriteBatch.DrawLine(Arial, $"  {type.Name}: {EventCollector.Events[type].Count}", Color.White);
    }

    private void DisplayHallOfFame()
    {
        var size = new Vector2(100,100);
        var position = new Vector2(10, WindowSize.Y - size.Y - 10);
        foreach (var o in _hallOfFame.OrderByDescending(o => o.Fitness))
        {
            o.DrawAt(_spriteBatch, new Rectangle(position, size));
            _spriteBatch.LinePosition(position + new Vector2(size.X * 0.5f, 10));
            _spriteBatch.DrawCenteredLine(Arial, $"F: {o.Fitness:0}", Color.White);
            _spriteBatch.DrawCenteredLine(Arial, $"Cn: {o.MaxAliveChildren}", Color.White);
            _spriteBatch.DrawCenteredLine(Arial, $"Cf: {o.ChildrenFitness:0.0}", Color.White);
            position += new Vector2(size.X + 10, 0);
        }
    }
}
