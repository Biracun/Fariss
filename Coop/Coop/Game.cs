using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Storage;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Factories;
using FarseerPhysics.Controllers;
using System.Xml;

#if (DEBUG)
using FarseerPhysics.DebugViews;
#endif

// Card tower
// User controllable ferris wheel
// Both players hold back to exit

namespace Platformer
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class PlatformGame : Microsoft.Xna.Framework.Game
    {
        // 360 Stuff
#if (XBOX360)
        StorageDevice _storageDevice;
        StorageContainer _storageContainer;
        IAsyncResult _storageResult;
        bool _storageDeviceWaiting;
        bool _storageWaiting;
#endif
        bool _noSaves = false;

        // XNA stuff
        GraphicsDeviceManager _graphics;
        SpriteBatch _spriteBatch;

        GamePadState _gamePadStatePrevious;
        GamePadState _gamePadStatePreviousP2;
        MouseState _mouseStatePrevious;

        Viewport _singleViewPort;
        Viewport _leftViewPort;
        Viewport _rightViewPort;

        // Physics instances
        private World _world;
        private List<Fixture> _platforms;

        protected Health _health;
        protected Color _gameOverColour;
        protected double _gameInitialOverTimer;
        protected double _gameOverTimer;
        protected Texture2D _gameOverScreen;

        protected double _backCounter;

        protected bool _trySave;

#if (DEBUG)
        private DebugViewXNA _debugView;
        bool _debugViewVisible = true;
#endif

        // Game instances
        private PlayerOne _playerOne;
        private PlayerTwo _playerTwo;
        private Level _playerOneLevel;
        private Level _playerTwoLevel;

        // Resources
        List<Texture2D> _textures;
        List<Texture2D> _backgrounds;
        List<Texture2D> _menustuff;
        private SpriteFont _defaultFont;
        private Texture2D _4px;
        private Texture2D _borderVert;
        private Texture2D _borderHorz;
        private Texture2D _redHealthbar;
        private Texture2D _greenHealthbar;
        private Texture2D _arrow;

        // State
        protected GameState GameState = GameState.MAINMENU;
        protected MenuState MenuState = MenuState.MAIN;
        protected int _menuOption;
        public GameSwitches GameSwitches;
        Matrix proj;
        Vector2 _currentScreen;
        Vector2 _nextScreen;
        Vector2 _blockScreenSize;
        SplitScreenMode _splitScreenMode = SplitScreenMode.NONE;

        // Data
        List<string> _saveFiles;
        public SaveGame CurrentSave { get; set; }
        protected const int MAX_LEVEL_X = 8;
        protected const int MAX_LEVEL_Y = 8;
        public Level[,] _levels;

        // Developer
        public Vector2 _firstClick;
        public Vector2 _release;

        public PlatformGame()
        {
#if (DEBUG)
#endif
            GameState = GameState.MAINMENU;
            MenuState = MenuState.MAIN;
            /*
             * Blank level generator
             */
            //for (int y = 0; y < 8; y++)
            //{
            //    for (int x = 0; x < 8; x++)
            //    {
            //        string test = "<?xml version=\"1.0\" encoding=\"utf-8\" ?>\n<XnaContent>\n\t\t<Asset Type=\"Platformer.Level\">\n\t\t<Backgrounds>\n\t\t\t<Item Type=\"Platformer.Background\">\n\t\t\t\t<ID>0</ID>\n\t\t\t\t<Velocity>0 0</Velocity>\n\t\t\t\t<Position>0 0</Position>\n\t\t\t</Item>\n\t\t</Backgrounds>\n\t\t<StaticObjects>\n\t\t</StaticObjects>\n\t\t<DynamicObjects>\n\t\t</DynamicObjects>\n\t</Asset>\n</XnaContent>";
            //        FileStream fs = new FileStream("..\\..\\..\\..\\CoopContent\\Level\\" + x.ToString() + "_" + y.ToString() + ".xml", FileMode.Create);
            //        fs.Write(System.Text.ASCIIEncoding.ASCII.GetBytes(test), 0, System.Text.ASCIIEncoding.ASCII.GetBytes(test).Length);
            //        fs.Close();
            //    }
            //}
            //this.Exit();
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 720;
            IsMouseVisible = true;
            Content.RootDirectory = "Content";
            _world = new World(new Vector2(0, -10));
            TargetElapsedTime = new TimeSpan(0, 0, 0, 0, 10);
            _blockScreenSize = new Vector2(_graphics.PreferredBackBufferWidth / 32.0f, _graphics.PreferredBackBufferHeight / 32.0f);
            _currentScreen = new Vector2(-1, -1);
            _nextScreen = new Vector2(0, 0);

#if (DEBUG)
            _debugView = new DebugViewXNA(_world);
#endif
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
#if (XBOX360)
            this.Components.Add(new GamerServicesComponent(this));
#endif

            _gameOverColour = Color.White;
            _gameOverTimer = 0;

            _saveFiles = new List<string>();
            //// Load SaveGame
            //CurrentSave = new SaveGame();
            //CurrentSave.FileName = "..\\SaveGames\\1.xml";

            //FileStream fileStream = new FileStream(CurrentSave.FileName, FileMode.OpenOrCreate);
            //XmlSerializer xs = new XmlSerializer(CurrentSave.GetType());
            //CurrentSave = (SaveGame)xs.Deserialize(fileStream);
            //fileStream.Close();
            //CurrentSave.FileName = "..\\SaveGames\\1.xml";
            //CurrentSave.GameSwitches = GameSwitches.BOSS_1_DEFEATED | GameSwitches.INITIAL_CUTSCENE_COMPLETE;
            //CurrentSave.Health = 100;
            //fileStream = new FileStream(CurrentSave.FileName, FileMode.Create);
            //xs.Serialize(fileStream, CurrentSave);
            //fileStream.Close();

            _platforms = new List<Fixture>();
            _menustuff = new List<Texture2D>();
            _textures = new List<Texture2D>();
            _backgrounds = new List<Texture2D>();

            _mouseStatePrevious = Mouse.GetState();

            _singleViewPort = GraphicsDevice.Viewport;
            _leftViewPort = GraphicsDevice.Viewport;
            _rightViewPort = GraphicsDevice.Viewport;
            _leftViewPort.Width /= 2;
            _rightViewPort.Width /= 2;
            _rightViewPort.X += _graphics.PreferredBackBufferWidth / 2;

            //// Temp savedata stuff

            //SaveGame saveGame = new SaveGame();
            //saveGame.PlayerOne = new PlayerLoad();
            //saveGame.PlayerOne.Position = new Vector2(0, 1);
            //saveGame.PlayerOne.NextScreen = new Vector2(6, 3);
            //saveGame.PlayerTwo = new PlayerLoad();
            //saveGame.PlayerTwo.Position = new Vector2(0, 1);
            //saveGame.PlayerTwo.NextScreen = new Vector2(6, 3);
            //saveGame.Health = 100;
            //saveGame.PlayerOne.Collectables = 0;
            //saveGame.PlayerTwo.Collectables = Collectable.GRAB;
            //CurrentSave = saveGame;
            //CurrentSave.CurrentGameSwitches = 0;

            //_health = new Health();
            //_health.MaxHealth = CurrentSave.Health;
            //_health.CurrentHealth = CurrentSave.Health;

            //_playerOne = new PlayerOne() { SaveGame = CurrentSave };
            //_playerOne.Position = CurrentSave.PlayerOne.Position;
            //_playerOne.NextScreen = CurrentSave.PlayerOne.NextScreen;
            //_playerOne.Collectables = CurrentSave.PlayerOne.Collectables;

            //_playerTwo = new PlayerTwo() { SaveGame = CurrentSave };
            //_playerTwo.Position = CurrentSave.PlayerTwo.Position;
            //_playerTwo.NextScreen = CurrentSave.PlayerTwo.NextScreen;
            //_playerTwo.Collectables = CurrentSave.PlayerTwo.Collectables;

            //GameSwitches = CurrentSave.GameSwitches;

            //_playerOne.Create(_world);
            //_playerTwo.Create(_world);

            //_playerOne.SharedHealth = _health;
            //_playerTwo.SharedHealth = _health;

            //_playerOne._textures = _textures;
            //_playerTwo._textures = _textures;

            //GameState = GameState.PLAY;

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

            _defaultFont = Content.Load<SpriteFont>("DefaultFont");
            _4px = Content.Load<Texture2D>("_4px");
            _borderVert = Content.Load<Texture2D>("borderVert");
            _borderHorz = Content.Load<Texture2D>("borderHorz");
            _redHealthbar = Content.Load<Texture2D>("red");
            _greenHealthbar = Content.Load<Texture2D>("green");
            _arrow = Content.Load<Texture2D>("arrow");
            _gameOverScreen = Content.Load<Texture2D>("gameOver1");

            // Load textures
            // Menu stuff
            _menustuff.Add(Content.Load<Texture2D>("titleback"));               // 0
            _menustuff.Add(Content.Load<Texture2D>("pressstart"));              // 1
            _menustuff.Add(Content.Load<Texture2D>("new"));                     // 2
            _menustuff.Add(Content.Load<Texture2D>("newselected"));             // 3
            _menustuff.Add(Content.Load<Texture2D>("load"));                    // 4
            _menustuff.Add(Content.Load<Texture2D>("loadselected"));            // 5
            _menustuff.Add(Content.Load<Texture2D>("loadscreen"));              // 6
            _menustuff.Add(Content.Load<Texture2D>("instructions"));            // 7
            _menustuff.Add(Content.Load<Texture2D>("instructionsselected"));    // 8
            _menustuff.Add(Content.Load<Texture2D>("instructionsscreen"));      // 9

            // Game stuff
            _textures.Add(_4px);                                        // 0
            _textures.Add(Content.Load<Texture2D>("_32x32"));           // 1
            _textures.Add(Content.Load<Texture2D>("player"));           // 2
            _textures.Add(Content.Load<Texture2D>("playerWheel"));      // 3
            _textures.Add(Content.Load<Texture2D>("grass"));            // 4
            _textures.Add(Content.Load<Texture2D>("dirt"));             // 5
            _textures.Add(Content.Load<Texture2D>("darkDirt"));         // 6
            _textures.Add(Content.Load<Texture2D>("bar"));              // 7
            _textures.Add(Content.Load<Texture2D>("savepoint"));        // 8
            _textures.Add(Content.Load<Texture2D>("savepointglow"));    // 9
            _textures.Add(Content.Load<Texture2D>("justgrass"));        // 10
            _textures.Add(Content.Load<Texture2D>("gravel"));           // 11
            _textures.Add(Content.Load<Texture2D>("1bar"));             // 12
            _textures.Add(Content.Load<Texture2D>("brick"));            // 13
            _textures.Add(Content.Load<Texture2D>("coat"));             // 14
            _textures.Add(Content.Load<Texture2D>("computer"));         // 15
            _textures.Add(Content.Load<Texture2D>("caveground"));       // 16
            _textures.Add(Content.Load<Texture2D>("cavetile"));         // 17
            _textures.Add(Content.Load<Texture2D>("cavevertblend"));    // 18
            _textures.Add(Content.Load<Texture2D>("cavevertascent"));   // 19
            _textures.Add(Content.Load<Texture2D>("caveverttop"));      // 20
            _textures.Add(Content.Load<Texture2D>("pad"));              // 21
            _textures.Add(Content.Load<Texture2D>("downarrow"));        // 22
            _textures.Add(Content.Load<Texture2D>("uparrow"));          // 23
            _textures.Add(Content.Load<Texture2D>("padglow"));          // 24
            _textures.Add(Content.Load<Texture2D>("shield"));           // 25
            _textures.Add(Content.Load<Texture2D>("cavevertblendr"));   // 26
            _textures.Add(Content.Load<Texture2D>("cavevertascentr"));  // 27
            _textures.Add(Content.Load<Texture2D>("caveverttopr"));     // 28
            _textures.Add(Content.Load<Texture2D>("spike"));            // 29
            _textures.Add(Content.Load<Texture2D>("wheel"));            // 30
            _textures.Add(Content.Load<Texture2D>("tree"));             // 31
            _textures.Add(Content.Load<Texture2D>("spike2"));           // 32
            _textures.Add(Content.Load<Texture2D>("companion"));        // 33
            _textures.Add(Content.Load<Texture2D>("roof"));             // 34
            _textures.Add(Content.Load<Texture2D>("shoecube"));         // 35
            _textures.Add(Content.Load<Texture2D>("cog"));              // 36
            _textures.Add(Content.Load<Texture2D>("blob"));             // 37
            _textures.Add(Content.Load<Texture2D>("powercell"));        // 38
            _textures.Add(Content.Load<Texture2D>("roughbar"));         // 39
            _textures.Add(Content.Load<Texture2D>("spikeball"));        // 40
            _textures.Add(Content.Load<Texture2D>("caveceiling"));      // 41
            _textures.Add(Content.Load<Texture2D>("spike"));            // 42
            _textures.Add(Content.Load<Texture2D>("spaceship"));        // 43
            _textures.Add(Content.Load<Texture2D>("hammer"));           // 44
            _textures.Add(Content.Load<Texture2D>("brick2"));           // 45

            // Background stuff
            _backgrounds.Add(Content.Load<Texture2D>("sky"));           // 0
            _backgrounds.Add(Content.Load<Texture2D>("ship"));          // 1
            _backgrounds.Add(Content.Load<Texture2D>("cave"));          // 2
            _backgrounds.Add(Content.Load<Texture2D>("face"));          // 3

#if (DEBUG)
            _debugView.LoadContent(_graphics.GraphicsDevice, Content);
#endif

            // Load levels from XML data

            _levels = new Level[MAX_LEVEL_X, MAX_LEVEL_Y];
            for (int x = 0; x < _levels.GetLength(0); x++)
            {
                for (int y = 0; y < _levels.GetLength(1); y++)
                {
                    _levels[x, y] = Content.Load<Level>("Level\\" + x.ToString() + "_" + y.ToString());
                }
            }

            //if (_playerOne.NextScreen != _playerTwo.NextScreen)
            //    LoadLevel((int)_playerTwo.NextScreen.X, (int)_playerTwo.NextScreen.Y, _blockScreenSize.X);

            // Create barrier between levels
            FixtureFactory.CreateRectangle(_world, 2, 100, 1, new Vector2(_blockScreenSize.X - 1, 0));

            //test = Content.Load<Level>("level");

            //// Make sure to initialise all objects
            //foreach (GameObject loadedObject in test.StaticObjects)
            //{
            //    loadedObject.Create(_world);
            //}

            List<DynamicObject> children = new List<DynamicObject>();
            foreach (Level level in _levels)
            {
                level.Background = _backgrounds;
                // Load textures (this is inefficient, TODO: update it to use a BST or something)
                foreach (GameObject gameObject in level.StaticObjects)
                {
                    gameObject._textures = _textures;
                    gameObject.GameSave = CurrentSave;
                    gameObject._defaultFont = _defaultFont;
                    gameObject._content = Content;
                }
                foreach (DynamicObject gameObject in level.DynamicObjects)
                {
                    gameObject._textures = _textures;
                    gameObject.GameSave = CurrentSave;
                    gameObject._defaultFont = _defaultFont;
                    gameObject._content = Content;
                    children.Add(gameObject);
                }
            }
            foreach (Level level in _levels)
            {
                foreach (DynamicObject dynamicObject in level.DynamicObjects)
                {
                    if (dynamicObject is Parent)
                    {
                        foreach (DynamicObject child in children)
                        {
                            foreach (int ID in ((Parent)dynamicObject).ChildIDs)
                            {
                                if (child.ID == ID)
                                {
                                    ((Parent)dynamicObject).Children.Add(child);
                                }
                            }
                        }
                    }
                }
            }

            _world.ContactManager.BeginContact += BeginContact;
            _world.ContactManager.EndContact += EndContact;

            //_staticObjects.Add(new Blade(_world, new Vector2(6, 0.5f), new Vector2(-3, -3)));
            //_staticObjects.Add(new Blade(_world, new Vector2(6, 0.5f), new Vector2(3, 0)));
            //_staticObjects.Add(new Blade(_world, new Vector2(6, 0.5f), new Vector2(-3, 3)));

            // TODO: use this.Content to load your game content here
        }

        private bool BeginContact(Contact contact)
        {
            Fixture fixtureA = contact.FixtureA;
            Fixture fixtureB = contact.FixtureB;

            if (fixtureA.UserData is GameObject && fixtureB.UserData is GameObject)
            {
                ((GameObject)fixtureA.UserData)._contactFixtures.Add(fixtureB);
                ((GameObject)fixtureB.UserData)._contactFixtures.Add(fixtureA);
            }

            if (fixtureB.UserData is IClibmableThing && fixtureA.UserData is Player)
                ((Player)fixtureA.UserData).ClimbableThings.Add(fixtureB);

            if (fixtureB.UserData is SavePoint)
            {
                Player player;
                SavePoint savepoint = (SavePoint)fixtureB.UserData;
                if (fixtureA.UserData is Player)
                {
                    player = (Player)fixtureA.UserData;
                    player.SavePointContact = true;
                    savepoint.Activated = true;
                }
            }

            return true;
        }

        private void EndContact(Contact contact)
        {
            Fixture fixtureA = contact.FixtureA;
            Fixture fixtureB = contact.FixtureB;

            if (fixtureA.UserData is GameObject && fixtureB.UserData is GameObject)
            {
                ((GameObject)fixtureA.UserData)._contactFixtures.Remove(fixtureB);
                ((GameObject)fixtureB.UserData)._contactFixtures.Remove(fixtureA);
            }

            if (fixtureB.UserData is IClibmableThing && fixtureA.UserData is Player)
                ((Player)fixtureA.UserData).ClimbableThings.Remove(fixtureB);

            if (fixtureB.UserData is SavePoint)
            {
                Player player;
                SavePoint savepoint = (SavePoint)fixtureB.UserData;
                if (fixtureA.UserData is Player)
                {
                    player = (Player)fixtureA.UserData;
                    player.SavePointContact = false;
                    savepoint.Activated = false;
                }
            }

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit

#if (false)
            if (Mouse.GetState().LeftButton == ButtonState.Pressed && _mouseStatePrevious.LeftButton == ButtonState.Released)
            {
                _firstClick = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
            }
            else if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                _release = new Vector2(Mouse.GetState().X, Mouse.GetState().Y) - _firstClick;
            }
            else
            {
                _firstClick = Vector2.Zero;
                _release = Vector2.Zero;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.E))
            {
                _firstClick.X += 0.1f;
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                _firstClick.X -= 0.1f;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.OemComma))
            {
                _firstClick.Y -= 0.1f;
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.O))
            {
                _firstClick.Y += 0.1f;
            }
#endif

            switch (GameState)
            {
                case GameState.MAINMENU:
                    switch (MenuState)
                    {
                        case MenuState.MAIN:
                            if (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.Start)
                                || GamePad.GetState(PlayerIndex.Two).IsButtonDown(Buttons.Start))
                                MenuState = MenuState.NEWLOAD;
                            if ((!GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.Back)
                                    && _gamePadStatePrevious.IsButtonDown(Buttons.Back))
                                || (!GamePad.GetState(PlayerIndex.Two).IsButtonDown(Buttons.Back)
                                    && _gamePadStatePreviousP2.IsButtonDown(Buttons.Back)))
                            {
                                //this.Exit();
                            }
                            break;
                        case MenuState.NEWLOAD:
                            if ((!GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.Back)
                                    && _gamePadStatePrevious.IsButtonDown(Buttons.Back))
                                || (!GamePad.GetState(PlayerIndex.Two).IsButtonDown(Buttons.Back)
                                    && _gamePadStatePreviousP2.IsButtonDown(Buttons.Back)))
                            {
                                MenuState = MenuState.MAIN;
                            }
#if (XBOX360)
                            if (!Guide.IsVisible && !_storageDeviceWaiting && _storageDevice == null)
                            {
                                _storageResult = StorageDevice.BeginShowSelector(PlayerIndex.One, null, null);
                                _storageDeviceWaiting = true;
                            }
#endif
                            if ((GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.LeftThumbstickDown) &&
                                    !_gamePadStatePrevious.IsButtonDown(Buttons.LeftThumbstickDown))
                                || (GamePad.GetState(PlayerIndex.Two).IsButtonDown(Buttons.LeftThumbstickDown) &&
                                    !_gamePadStatePreviousP2.IsButtonDown(Buttons.LeftThumbstickDown)))
                            {
                                _menuOption++;
#if (!XBOX360)
                                if (_menuOption == 1)
                                    _menuOption = 2;
#endif
                                if (_menuOption > 2)
                                    _menuOption = 0;
                            }
                            else if ((GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.LeftThumbstickUp) &&
                                      !_gamePadStatePrevious.IsButtonDown(Buttons.LeftThumbstickUp))
                                  || (GamePad.GetState(PlayerIndex.Two).IsButtonDown(Buttons.LeftThumbstickUp) &&
                                      !_gamePadStatePreviousP2.IsButtonDown(Buttons.LeftThumbstickUp)))
                            {
                                _menuOption--;
#if (!XBOX360)
                                if (_menuOption == 1)
                                    _menuOption = 0;
#endif
                                if (_menuOption < 0)
                                    _menuOption = 2;
                            }

                            if ((GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.A)
                                    && !_gamePadStatePrevious.IsButtonDown(Buttons.A))
                                || (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.B)
                                    && !_gamePadStatePrevious.IsButtonDown(Buttons.B))
                                || (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.X)
                                    && !_gamePadStatePrevious.IsButtonDown(Buttons.X))
                                || (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.Y)
                                    && !_gamePadStatePrevious.IsButtonDown(Buttons.Y))
                                || (GamePad.GetState(PlayerIndex.Two).IsButtonDown(Buttons.A)
                                    && !_gamePadStatePreviousP2.IsButtonDown(Buttons.A))
                                || (GamePad.GetState(PlayerIndex.Two).IsButtonDown(Buttons.B)
                                    && !_gamePadStatePreviousP2.IsButtonDown(Buttons.B))
                                || (GamePad.GetState(PlayerIndex.Two).IsButtonDown(Buttons.X)
                                    && !_gamePadStatePreviousP2.IsButtonDown(Buttons.X))
                                || (GamePad.GetState(PlayerIndex.Two).IsButtonDown(Buttons.Y)
                                    && !_gamePadStatePreviousP2.IsButtonDown(Buttons.Y)))
                            {
                                if (_menuOption == 0)
                                {
#if (XBOX360)
                                    if (_storageDevice != null)
                                    {
                                        MenuState = MenuState.NEW;
                                    }
#else
                                    SaveGame saveGame = new SaveGame();
                                    saveGame.PlayerOne = new PlayerLoad();
                                    saveGame.PlayerOne.Position = new Vector2(-2, -6);
                                    saveGame.PlayerOne.NextScreen = new Vector2(6, 3);
                                    saveGame.PlayerTwo = new PlayerLoad();
                                    saveGame.PlayerTwo.Position = new Vector2(2, -6);
                                    saveGame.PlayerTwo.NextScreen = new Vector2(6, 3);
                                    saveGame.Health = 100;
                                    saveGame.PlayerOne.Collectables = Collectable.JUMP;
                                    saveGame.PlayerTwo.Collectables = Collectable.GRAB;
                                    CurrentSave = saveGame;

                                    _health = new Health();
                                    _health.MaxHealth = CurrentSave.Health;
                                    _health.CurrentHealth = CurrentSave.Health;

                                    _playerOne = new PlayerOne() { SaveGame = CurrentSave };
                                    _playerOne.Position = CurrentSave.PlayerOne.Position;
                                    _playerOne.NextScreen = CurrentSave.PlayerOne.NextScreen;
                                    _playerOne.Collectables = CurrentSave.PlayerOne.Collectables;
                                    _playerOne._content = Content;

                                    _playerTwo = new PlayerTwo() { SaveGame = CurrentSave };
                                    _playerTwo.Position = CurrentSave.PlayerTwo.Position;
                                    _playerTwo.NextScreen = CurrentSave.PlayerTwo.NextScreen;
                                    _playerTwo.Collectables = CurrentSave.PlayerTwo.Collectables;
                                    _playerTwo._content = Content;

                                    GameSwitches = CurrentSave.GameSwitches;

                                    UpdateGameSave();

                                    _playerOne.Create(_world);
                                    _playerTwo.Create(_world);

                                    _playerOne.SharedHealth = _health;
                                    _playerTwo.SharedHealth = _health;

                                    _playerOne._textures = _textures;
                                    _playerTwo._textures = _textures;
                                    GameState = GameState.PLAY;
#endif

#if (XBOX360)
#endif

                                }
                                else if (_menuOption == 1)
                                {
#if (XBOX360)
                                    if (_storageDevice != null)
                                    {
                                        MenuState = MenuState.LOAD;
                                    }
#else
                                    MenuState = MenuState.LOAD;
#endif
                                }
                                else
                                {
                                    // Instructions
                                    MenuState = MenuState.INSTRUCTIONS;
                                }
                            }
#if (XBOX360)


                            //if (_storageWaiting && _storageResult.IsCompleted)
                            //{
                            //    _storageContainer = _storageDevice.EndOpenContainer(_storageResult);

                            //    Stream saveFile = _storageContainer.OpenFile(((int)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds).ToString() + ".xml", FileMode.Create);
                            //    SaveGame saveGame = new SaveGame();
                            //    saveGame.Health = 100;
                            //    saveGame.PlayerOne = new PlayerLoad();
                            //    saveGame.PlayerOne.NextScreen = new Vector2(6, 3);
                            //    saveGame.PlayerOne.Position = new Vector2(0, 0);
                            //    saveGame.PlayerTwo = new PlayerLoad();
                            //    saveGame.PlayerTwo.NextScreen = new Vector2(6, 3);
                            //    saveGame.PlayerTwo.Position = new Vector2(0, 0);
                            //    XmlSerializer xmlSerializer = new XmlSerializer(saveGame.GetType());
                            //    xmlSerializer.Serialize(saveFile, saveGame);
                            //    saveFile.Close();

                            //    _storageWaiting = false;
                            //    _storageContainer.Dispose();
                            //}
                            if (_storageDeviceWaiting && _storageResult.IsCompleted)
                            {
                                _storageDevice = StorageDevice.EndShowSelector(_storageResult);
                                if (_storageDevice != null && _storageDevice.IsConnected)
                                {
                                    _storageDeviceWaiting = false;
                                }
                            }

#endif
                            break;
                        case MenuState.INSTRUCTIONS:
                            if ((!GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.Back)
                                    && _gamePadStatePrevious.IsButtonDown(Buttons.Back))
                                || (!GamePad.GetState(PlayerIndex.Two).IsButtonDown(Buttons.Back)
                                    && _gamePadStatePreviousP2.IsButtonDown(Buttons.Back)))
                            {
                                MenuState = MenuState.NEWLOAD;
                            }
                            break;
                        case MenuState.NEW:
#if (XBOX360)
                            if (!_storageWaiting && (_storageContainer == null || _storageContainer.IsDisposed))
                            {
                                _storageResult = _storageDevice.BeginOpenContainer("Fariss_SaveData", null, null);
                                _storageWaiting = true;
                            }

                            if (_storageWaiting && _storageResult.IsCompleted)
                            {
                                _storageContainer = _storageDevice.EndOpenContainer(_storageResult);

                                Stream fileStream = _storageContainer.CreateFile(((int)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds).ToString() + ".xml");
                                SaveGame saveGame = new SaveGame();
                                saveGame.Health = 100;
                                saveGame.PlayerOne = new PlayerLoad();
                                saveGame.PlayerOne.NextScreen = new Vector2(6, 3);
                                saveGame.PlayerOne.Position = new Vector2(3, -6);
                                saveGame.PlayerOne.Collectables = Collectable.JUMP;
                                saveGame.PlayerTwo = new PlayerLoad();
                                saveGame.PlayerTwo.NextScreen = new Vector2(6, 3);
                                saveGame.PlayerTwo.Position = new Vector2(8, -6);
                                saveGame.PlayerTwo.Collectables = Collectable.GRAB;
                                XmlSerializer xmlSerializer = new XmlSerializer(saveGame.GetType());
                                xmlSerializer.Serialize(fileStream, saveGame);
                                fileStream.Close();

                                CurrentSave = saveGame;

                                _health = new Health();
                                _health.MaxHealth = CurrentSave.Health;
                                _health.CurrentHealth = CurrentSave.Health;

                                _playerOne = new PlayerOne() { SaveGame = CurrentSave };
                                _playerOne.Position = CurrentSave.PlayerOne.Position;
                                _playerOne.NextScreen = CurrentSave.PlayerOne.NextScreen;
                                _playerOne.Collectables = CurrentSave.PlayerOne.Collectables;
                                _playerOne._content = Content;

                                _playerTwo = new PlayerTwo() { SaveGame = CurrentSave };
                                _playerTwo.Position = CurrentSave.PlayerTwo.Position;
                                _playerTwo.NextScreen = CurrentSave.PlayerTwo.NextScreen;
                                _playerTwo.Collectables = CurrentSave.PlayerTwo.Collectables;
                                _playerTwo._content = Content;

                                GameSwitches = CurrentSave.GameSwitches;

                                UpdateGameSave();

                                _playerOne.Create(_world);
                                _playerTwo.Create(_world);

                                _playerOne.SharedHealth = _health;
                                _playerTwo.SharedHealth = _health;

                                _playerOne._textures = _textures;
                                _playerTwo._textures = _textures;

                                GameState = GameState.PLAY;

                                _storageContainer.Dispose();
                                _storageWaiting = false;
                            }
#endif
                            break;
                        case MenuState.LOAD:

                            if (_saveFiles.Count <= 0 && !_noSaves)
                            {
                                _menuOption = 0;
#if (XBOX360)
                                if (!_storageWaiting && (_storageContainer == null || _storageContainer.IsDisposed))
                                {
                                    _storageResult = _storageDevice.BeginOpenContainer("Fariss_SaveData", null, null);
                                    _storageWaiting = true;
                                }

                                if (_storageWaiting && _storageResult.IsCompleted)
                                {
                                    _storageContainer = _storageDevice.EndOpenContainer(_storageResult);

                                    string[] fileList = _storageContainer.GetFileNames("*.xml");
                                    if (fileList.Length <= 0)
                                    {
                                        _noSaves = true;
                                        break;
                                    }
                                    foreach (string fileName in fileList)
                                    {
                                        _saveFiles.Add(fileName);
                                    }

                                    _saveFiles.Sort();
                                    _saveFiles.Reverse();

                                    _storageWaiting = false;
                                }
#else
                                //DirectoryInfo directoryInfo = new DirectoryInfo("SaveGames");
                                //FileInfo[] directoryFiles = directoryInfo.GetFiles("*.xml");
                                //foreach (FileInfo file in directoryFiles)
                                //{
                                //    saveFiles.Add(file);
                                //    Console.WriteLine(file.Name);
                                //}
#endif
                            }
                            if ((GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.LeftThumbstickDown) &&
                                    !_gamePadStatePrevious.IsButtonDown(Buttons.LeftThumbstickDown))
                                || (GamePad.GetState(PlayerIndex.Two).IsButtonDown(Buttons.LeftThumbstickDown) &&
                                    !_gamePadStatePreviousP2.IsButtonDown(Buttons.LeftThumbstickDown)))
                            {
                                if (_menuOption + 1 < _saveFiles.Count)
                                    _menuOption++;
                            }
                            else if ((GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.LeftThumbstickUp) &&
                                      !_gamePadStatePrevious.IsButtonDown(Buttons.LeftThumbstickUp))
                                  || (GamePad.GetState(PlayerIndex.Two).IsButtonDown(Buttons.LeftThumbstickUp) &&
                                      !_gamePadStatePreviousP2.IsButtonDown(Buttons.LeftThumbstickUp)))
                            {
                                if (_menuOption > 0)
                                    _menuOption--;
                            }

                            if ((GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.A)
                                    && !_gamePadStatePrevious.IsButtonDown(Buttons.A))
                                || (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.B)
                                    && !_gamePadStatePrevious.IsButtonDown(Buttons.B))
                                || (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.X)
                                    && !_gamePadStatePrevious.IsButtonDown(Buttons.X))
                                || (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.Y)
                                    && !_gamePadStatePrevious.IsButtonDown(Buttons.Y))
                                || (GamePad.GetState(PlayerIndex.Two).IsButtonDown(Buttons.A)
                                    && !_gamePadStatePreviousP2.IsButtonDown(Buttons.A))
                                || (GamePad.GetState(PlayerIndex.Two).IsButtonDown(Buttons.B)
                                    && !_gamePadStatePreviousP2.IsButtonDown(Buttons.B))
                                || (GamePad.GetState(PlayerIndex.Two).IsButtonDown(Buttons.X)
                                    && !_gamePadStatePreviousP2.IsButtonDown(Buttons.X))
                                || (GamePad.GetState(PlayerIndex.Two).IsButtonDown(Buttons.Y)
                                    && !_gamePadStatePreviousP2.IsButtonDown(Buttons.Y)))
                            {
                                // Load selected save file
#if (XBOX360)
                                if (_saveFiles.Count > 0)
                                {
                                    if (_storageContainer != null)
                                    {
                                        Stream fileStream = _storageContainer.OpenFile(_saveFiles[_menuOption], FileMode.OpenOrCreate);
                                        SaveGame saveGame = new SaveGame();
                                        XmlSerializer xmlSerializer = new XmlSerializer(saveGame.GetType());
                                        saveGame = (SaveGame)xmlSerializer.Deserialize(fileStream);
                                        CurrentSave = saveGame;

                                        _health = new Health();
                                        _health.MaxHealth = CurrentSave.Health;
                                        _health.CurrentHealth = CurrentSave.Health;

                                        _playerOne = new PlayerOne() { SaveGame = CurrentSave };
                                        _playerOne.Position = CurrentSave.PlayerOne.Position;
                                        _playerOne.NextScreen = CurrentSave.PlayerOne.NextScreen;
                                        _playerOne.Collectables = CurrentSave.PlayerOne.Collectables;
                                        _playerOne._content = Content;

                                        _playerTwo = new PlayerTwo() { SaveGame = CurrentSave };
                                        _playerTwo.Position = CurrentSave.PlayerTwo.Position;
                                        _playerTwo.NextScreen = CurrentSave.PlayerTwo.NextScreen;
                                        _playerTwo.Collectables = CurrentSave.PlayerTwo.Collectables;
                                        _playerTwo._content = Content;

                                        GameSwitches = CurrentSave.GameSwitches;
                                        CurrentSave.CurrentGameSwitches = CurrentSave.GameSwitches;

                                        _playerOne.Create(_world);
                                        _playerTwo.Create(_world);

                                        _playerOne.SharedHealth = _health;
                                        _playerTwo.SharedHealth = _health;

                                        _playerOne._textures = _textures;
                                        _playerTwo._textures = _textures;

                                        UpdateGameSave();

                                        _storageContainer.Dispose();
                                        GameState = GameState.PLAY;
                                        _saveFiles.Clear();
                                    }
                                }
#endif

                            }
                            // Exit
                            if ((!GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.Back)
                                    && _gamePadStatePrevious.IsButtonDown(Buttons.Back))
                                || (!GamePad.GetState(PlayerIndex.Two).IsButtonDown(Buttons.Back)
                                    && _gamePadStatePreviousP2.IsButtonDown(Buttons.Back)))
                            {
#if (XBOX360)
                                _storageDeviceWaiting = false;
                                _storageWaiting = false;
                                _noSaves = false;
                                _saveFiles.Clear();
                                if (_storageContainer != null && !_storageContainer.IsDisposed)
                                    _storageContainer.Dispose();
#endif
                                MenuState = MenuState.NEWLOAD;
                            }
                            break;
                        case MenuState.GAMEOVER:
                            UnloadLevel((int)_playerOne.NextScreen.X, (int)_playerOne.NextScreen.Y);
                            UnloadLevel((int)_playerTwo.NextScreen.X, (int)_playerTwo.NextScreen.Y);
                            _playerOne.Dispose();
                            _playerTwo.Dispose();
                            if (_gameInitialOverTimer <= 2000)
                            {
                                _gameInitialOverTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
                            }
                            if (_gameInitialOverTimer > 2000)
                            {
                                _gameOverTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
                                if (_gameOverTimer > 100)
                                {
                                    _gameOverTimer = 0;
                                    _gameOverColour = new Color(255, 255, 255, _gameOverColour.A - 25);
                                }
                                if (_gameOverColour.A == 0)
                                {
                                    _gameOverColour = Color.White;
                                    _gameOverTimer = 0;
                                    MenuState = MenuState.MAIN;
                                }
                            }
                            break;
                        default:
                            break;
                    }
                    break;
                case GameState.PLAY:
                    if ((CurrentSave.CurrentGameSwitches & GameSwitches.GOT_COG) == GameSwitches.GOT_COG
                        && (CurrentSave.CurrentGameSwitches & GameSwitches.GOT_BLOB) == GameSwitches.GOT_BLOB
                        && (CurrentSave.CurrentGameSwitches & GameSwitches.GOT_POWER_CELL) == GameSwitches.GOT_POWER_CELL)
                    {
                        UnloadLevel((int)_playerOne.NextScreen.X, (int)_playerOne.NextScreen.Y);
                        UnloadLevel((int)_playerTwo.NextScreen.X, (int)_playerTwo.NextScreen.Y);
                        GameState = GameState.MAINMENU;
                        MenuState = MenuState.GAMEOVER;
                    }
#if (XBOX360)
                    if (_trySave && _storageResult != null && _storageResult.IsCompleted)
                    {
                        _trySave = false;

                        _storageContainer = _storageDevice.EndOpenContainer(_storageResult);
                        CurrentSave.GameSwitches = CurrentSave.CurrentGameSwitches;
                        CurrentSave.PlayerOne.Position = _playerOne.Position;
                        CurrentSave.PlayerOne.NextScreen = _playerOne.NextScreen;
                        CurrentSave.PlayerOne.Collectables = _playerOne.Collectables;
                        CurrentSave.PlayerTwo.Position = _playerTwo.Position;
                        CurrentSave.PlayerTwo.NextScreen = _playerTwo.NextScreen;
                        CurrentSave.PlayerTwo.Collectables = _playerTwo.Collectables;
                        CurrentSave.Health = _playerOne.SharedHealth.MaxHealth;

                        Stream stream = _storageContainer.CreateFile(((int)(DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds).ToString() + ".xml");
                        XmlSerializer xmlSerializer = new XmlSerializer(CurrentSave.GetType());
                        xmlSerializer.Serialize(stream, CurrentSave);
                        _storageContainer.Dispose();
                    }
                    if (!_trySave
                        && GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.Y) && _playerOne.SavePointContact
                        && GamePad.GetState(PlayerIndex.Two).IsButtonDown(Buttons.Y) && _playerTwo.SavePointContact)
                    {
                        _trySave = true;
                        _storageResult = _storageDevice.BeginOpenContainer("Fariss_SaveData", null, null);
                    }
#endif

                    // If both players are holding back, increment counter
                    if (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.Back) && GamePad.GetState(PlayerIndex.Two).IsButtonDown(Buttons.Back))
                    {
                        _backCounter += gameTime.ElapsedGameTime.TotalMilliseconds;
                    }
                    else
                    {
                        _backCounter = 0;
                    }

                    if (_backCounter > 500)
                    {
                        _playerOne.Dispose();
                        _playerTwo.Dispose();
                        UnloadLevel((int)_playerOne.NextScreen.X, (int)_playerOne.NextScreen.Y);
                        UnloadLevel((int)_playerTwo.NextScreen.X, (int)_playerTwo.NextScreen.Y);
                        GameState = GameState.MAINMENU;
                        MenuState = MenuState.MAIN;
                    }

                    // Check players haven't gameovered
                    if (_health.CurrentHealth <= 0)
                    {
                        UnloadLevel((int)_playerOne.NextScreen.X, (int)_playerOne.NextScreen.Y);
                        UnloadLevel((int)_playerTwo.NextScreen.X, (int)_playerTwo.NextScreen.Y);
                        GameState = GameState.MAINMENU;
                        MenuState = MenuState.GAMEOVER;
                    }
                    // Check player hasn't gone off screen and stuff
                    UpdatePlayerScreen(_playerOne, _playerTwo);
                    UpdatePlayerScreen(_playerTwo, _playerOne);

                    // If both players are transitioning to new screens
                    if (_playerOne.NextScreen != _playerOne.CurrentScreen && _playerTwo.NextScreen != _playerTwo.CurrentScreen)
                    {
                        _playerOne.HeldObject = null;
                        _playerTwo.HeldObject = null;
                        // If both are transitioning to the same screen
                        if (_playerOne.NextScreen == _playerTwo.NextScreen)
                        {
                            // Then use virtual screen one
                            if (_playerOne.VirtualScreen == VirtualScreen.TWO)
                                _playerOne.X -= _blockScreenSize.X * 2;
                            if (_playerTwo.VirtualScreen == VirtualScreen.TWO)
                                _playerTwo.X -= _blockScreenSize.X * 2;
                            _playerOne.VirtualScreen = VirtualScreen.ONE;
                            _playerTwo.VirtualScreen = VirtualScreen.ONE;
                            UnloadLevel((int)_playerOne.CurrentScreen.X, (int)_playerOne.CurrentScreen.Y);
                            UnloadLevel((int)_playerTwo.CurrentScreen.X, (int)_playerTwo.CurrentScreen.Y);
                            LoadLevel((int)_playerOne.NextScreen.X, (int)_playerOne.NextScreen.Y, 0);
                        }
                        // If they're transitioning to different screens
                        else
                        {
                            if (_playerOne.VirtualScreen == VirtualScreen.TWO)
                                _playerOne.X -= _blockScreenSize.X * 2;
                            if (_playerTwo.VirtualScreen == VirtualScreen.ONE)
                                _playerTwo.X += _blockScreenSize.X * 2;
                            _playerOne.VirtualScreen = VirtualScreen.ONE;
                            _playerTwo.VirtualScreen = VirtualScreen.TWO;
                            UnloadLevel((int)_playerOne.CurrentScreen.X, (int)_playerOne.CurrentScreen.Y);
                            UnloadLevel((int)_playerTwo.CurrentScreen.X, (int)_playerTwo.CurrentScreen.Y);
                            LoadLevel((int)_playerOne.NextScreen.X, (int)_playerOne.NextScreen.Y, 0);
                            LoadLevel((int)_playerTwo.NextScreen.X, (int)_playerTwo.NextScreen.Y, _blockScreenSize.X * 2.0f);
                        }
                    }
                    // If just one of them is
                    else if (_playerOne.NextScreen != _playerOne.CurrentScreen || _playerTwo.NextScreen != _playerTwo.CurrentScreen)
                    {
                        // If it's player one
                        if (_playerOne.NextScreen != _playerOne.CurrentScreen)
                        {
                            _playerOne.HeldObject = null;
                            // If they're moving to the same screen as player two
                            if (_playerOne.NextScreen == _playerTwo.CurrentScreen)
                            {
                                if (_playerOne.VirtualScreen == VirtualScreen.TWO && _playerTwo.VirtualScreen == VirtualScreen.ONE)
                                    _playerOne.X -= _blockScreenSize.X * 2;
                                else if (_playerOne.VirtualScreen == VirtualScreen.ONE && _playerTwo.VirtualScreen == VirtualScreen.TWO)
                                    _playerOne.X += _blockScreenSize.X * 2;
                                _playerOne.VirtualScreen = _playerTwo.VirtualScreen;
                                UnloadLevel((int)_playerOne.CurrentScreen.X, (int)_playerOne.CurrentScreen.Y);
                            }
                            // Otherwise if player two is on virtual screen one
                            else if (_playerTwo.VirtualScreen == VirtualScreen.ONE)
                            {
                                // Load level on virtual screen 2
                                if (_playerOne.VirtualScreen == VirtualScreen.ONE)
                                    _playerOne.X += _blockScreenSize.X * 2;
                                _playerOne.VirtualScreen = VirtualScreen.TWO;
                                LoadLevel((int)_playerOne.NextScreen.X, (int)_playerOne.NextScreen.Y, _blockScreenSize.X * 2);
                                if (_playerOne.CurrentScreen != _playerTwo.CurrentScreen)
                                    UnloadLevel((int)_playerOne.CurrentScreen.X, (int)_playerOne.CurrentScreen.Y);
                            }
                            else
                            {
                                // Load level on virtual screen 1
                                if (_playerOne.VirtualScreen == VirtualScreen.TWO)
                                    _playerOne.X -= _blockScreenSize.X * 2;
                                _playerOne.VirtualScreen = VirtualScreen.ONE;
                                LoadLevel((int)_playerOne.NextScreen.X, (int)_playerOne.NextScreen.Y, 0);
                                if (_playerOne.CurrentScreen != _playerTwo.CurrentScreen)
                                    UnloadLevel((int)_playerOne.CurrentScreen.X, (int)_playerOne.CurrentScreen.Y);
                            }
                        }
                        // Otherwise if it's player 2
                        else
                        {
                            _playerTwo.HeldObject = null;
                            // If they're moving to the same screen as player one
                            if (_playerTwo.NextScreen == _playerOne.CurrentScreen)
                            {
                                if (_playerTwo.VirtualScreen == VirtualScreen.TWO && _playerOne.VirtualScreen == VirtualScreen.ONE)
                                    _playerTwo.X -= _blockScreenSize.X * 2;
                                else if (_playerTwo.VirtualScreen == VirtualScreen.ONE && _playerOne.VirtualScreen == VirtualScreen.TWO)
                                    _playerTwo.X += _blockScreenSize.X * 2;
                                _playerTwo.VirtualScreen = _playerOne.VirtualScreen;
                                UnloadLevel((int)_playerTwo.CurrentScreen.X, (int)_playerTwo.CurrentScreen.Y);
                            }
                            // Otherwise if player one is on virtual screen one
                            else if (_playerOne.VirtualScreen == VirtualScreen.ONE)
                            {
                                // Load level on virtual screen 2
                                if (_playerTwo.VirtualScreen == VirtualScreen.ONE)
                                    _playerTwo.X += _blockScreenSize.X * 2;
                                _playerTwo.VirtualScreen = VirtualScreen.TWO;
                                LoadLevel((int)_playerTwo.NextScreen.X, (int)_playerTwo.NextScreen.Y, _blockScreenSize.X * 2);
                                if (_playerTwo.CurrentScreen != _playerOne.CurrentScreen)
                                    UnloadLevel((int)_playerTwo.CurrentScreen.X, (int)_playerTwo.CurrentScreen.Y);
                            }
                            else
                            {
                                // Load level on virtual screen 1
                                if (_playerTwo.VirtualScreen == VirtualScreen.TWO)
                                    _playerTwo.X -= _blockScreenSize.X * 2;
                                _playerTwo.VirtualScreen = VirtualScreen.ONE;
                                LoadLevel((int)_playerTwo.NextScreen.X, (int)_playerTwo.NextScreen.Y, 0);
                                if (_playerTwo.CurrentScreen != _playerOne.CurrentScreen)
                                    UnloadLevel((int)_playerTwo.CurrentScreen.X, (int)_playerTwo.CurrentScreen.Y);
                            }
                        }
                    }
                    _playerOne.CurrentScreen = _playerOne.NextScreen;
                    _playerTwo.CurrentScreen = _playerTwo.NextScreen;

                    _playerOneLevel = _levels[(int)_playerOne.CurrentScreen.X, (int)_playerOne.CurrentScreen.Y];
                    _playerTwoLevel = _levels[(int)_playerTwo.CurrentScreen.X, (int)_playerTwo.CurrentScreen.Y];

#if (DEBUG)
                    if (GamePad.GetState(PlayerIndex.One).Buttons.B == ButtonState.Released && _gamePadStatePrevious.Buttons.B == ButtonState.Pressed)
                        _debugViewVisible = !_debugViewVisible;
#endif

                    _playerOne.Update(gameTime);
                    _playerTwo.Update(gameTime);
                    UpdateView(_playerOne, _playerTwo);

                    _playerOneLevel.Update(gameTime);
                    if (_playerOne.CurrentScreen != _playerTwo.CurrentScreen)
                        _playerTwoLevel.Update(gameTime);

                    _world.Step((float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.001f);

                    break;
                default:
                    this.Exit();
                    break;
            }

            _mouseStatePrevious = Mouse.GetState();
            _gamePadStatePrevious = GamePad.GetState(PlayerIndex.One);
            _gamePadStatePreviousP2 = GamePad.GetState(PlayerIndex.Two);

            base.Update(gameTime);
        }

        public void UpdateGameSave()
        {
            _playerOne.GameSave = CurrentSave;
            _playerOne.SaveGame = CurrentSave;
            _playerTwo.GameSave = CurrentSave;
            _playerTwo.SaveGame = CurrentSave;
            foreach (Level level in _levels)
            {
                foreach (GameObject gameObject in level.StaticObjects)
                {
                    gameObject.GameSave = CurrentSave;
                }
                foreach (GameObject gameObject in level.DynamicObjects)
                {
                    if (gameObject is PickupSpecial)
                    {

                    }
                    gameObject.GameSave = CurrentSave;
                }
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            switch (GameState)
            {
                case GameState.MAINMENU:
                    _spriteBatch.Begin();
                    _spriteBatch.Draw(_menustuff[0], Vector2.Zero, Color.White);

                    switch (MenuState)
                    {
                        case MenuState.MAIN:
                            if (gameTime.TotalGameTime.TotalMilliseconds % 1400 < 1100)
                                _spriteBatch.Draw(_menustuff[1], Vector2.Zero, Color.White);
                            break;
                        case MenuState.NEWLOAD:
                            _spriteBatch.Draw((_menuOption == 0 ? _menustuff[3] : _menustuff[2]), Vector2.Zero, Color.White);
                            _spriteBatch.Draw((_menuOption == 1 ? _menustuff[5] : _menustuff[4]), Vector2.Zero, Color.White);
                            _spriteBatch.Draw((_menuOption == 2 ? _menustuff[8] : _menustuff[7]), Vector2.Zero, Color.White);
                            break;
                        case MenuState.LOAD:
                            //for (int i = _menuOption - 2; i < saveFiles.Count && i < _menuOption + 3; i++)
                            if (_noSaves)
                            {
                                _spriteBatch.DrawString(_defaultFont, "No save data available", new Vector2(150, 450), Color.White);
                            }
                            else
                            {
                                for (int i = 0; i < 5; i++)
                                {
                                    int currentSave = i + _menuOption - 2;
                                    if (currentSave >= 0 && currentSave < _saveFiles.Count)
                                    {
                                        int unixTime;
                                        if (!int.TryParse(_saveFiles[currentSave].Substring(0, _saveFiles[currentSave].Length - 4), out unixTime))
                                            unixTime = 0;
                                        _spriteBatch.DrawString(_defaultFont, (new DateTime(1970, 1, 1, 0, 0, 0)).AddSeconds(unixTime).ToString(), new Vector2(150, 450 + i * 25), (i == 2 ? Color.Yellow : Color.White));
                                    }
                                }
                            }
                            break;
                        case MenuState.INSTRUCTIONS:
                            _spriteBatch.Draw(_menustuff[9], Vector2.Zero, _gameOverColour);
                            break;
                        case MenuState.GAMEOVER:
                            _spriteBatch.Draw(_gameOverScreen, Vector2.Zero, _gameOverColour);
                            break;
                        default:
                            break;
                    }
                    _spriteBatch.End();
                    break;
                case GameState.PLAY:
                    if (_playerOne.CurrentScreen == _playerTwo.CurrentScreen)
                    {
                        // If players are on the same screen, render normally
                        _spriteBatch.Begin(SpriteSortMode.Texture, null, null, null, null, null);
                        _spriteBatch.End();

                        // Render farseer debug overlay
                        Matrix view = Matrix.Identity;
                        Matrix realView = Matrix.Identity;
                        if (_playerOne.VirtualScreen == VirtualScreen.TWO)
                        {
                            view *= Matrix.CreateTranslation(-_blockScreenSize.X * 2, 0, 0);
                            realView = Matrix.CreateTranslation(-_blockScreenSize.X * 64, 0, 0);
                        }

                        _spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, realView);
                        _spriteBatch.Draw(_4px, new Rectangle((int)_firstClick.X, (int)_firstClick.Y, (int)_release.X, (int)_release.Y), Color.Black);
                        if (_playerOneLevel != null)
                            _playerOneLevel.Draw(gameTime, _spriteBatch);
                        _playerOne.Draw(gameTime, _spriteBatch);
                        _playerTwo.Draw(gameTime, _spriteBatch);
                        _spriteBatch.End();
#if (DEBUG)

                        if (_debugViewVisible)
                            _debugView.RenderDebugData(ref proj, ref view);
#endif
                    }
                    else
                    {
                        // Else, render view ports separetly
                        // Render left view port
                        {
                            GraphicsDevice.Viewport = _leftViewPort;

                            Matrix view = Matrix.Identity;
                            Matrix realView = Matrix.Identity;
                            if (_splitScreenMode == SplitScreenMode.HORIZONTAL)
                            {
                                // Render farseer debug overlay
                                float viewX = _playerOne.X;
                                if (viewX < -_blockScreenSize.X / 4.0f)
                                    viewX = -_blockScreenSize.X / 4.0f;
                                else if (viewX > _blockScreenSize.X / 4.0f && viewX < _blockScreenSize.X / 2.0f)
                                    viewX = _blockScreenSize.X / 4.0f;
                                else if (viewX >= (3 * _blockScreenSize.X) / 2.0 && viewX < (7 * _blockScreenSize.X) / 4.0f)
                                    viewX = (7 * _blockScreenSize.X) / 4.0f;
                                else if (viewX > (9 * _blockScreenSize.X) / 4.0f)
                                    viewX = (9 * _blockScreenSize.X) / 4.0f;
                                view = Matrix.CreateTranslation(-viewX, 0, 0);
                                realView = Matrix.CreateTranslation((-viewX * 32) - 320, 0, 0);
                            }
                            else if (_splitScreenMode == SplitScreenMode.VERTICAL)
                            {
                                float viewY = _playerOne.Y;
                                if (viewY > _blockScreenSize.Y / 4.0f)
                                    viewY = _blockScreenSize.Y / 4.0f;
                                else if (viewY < -(_blockScreenSize.Y / 4.0f))
                                    viewY = -(_blockScreenSize.Y / 4.0f);

                                if (_playerOne.VirtualScreen == VirtualScreen.ONE)
                                {
                                    view = Matrix.CreateTranslation(0, -viewY, 0);
                                    realView = Matrix.CreateTranslation(0, (viewY * 32) - 178, 0);
                                }
                                else
                                {
                                    view = Matrix.CreateTranslation(-_blockScreenSize.X * 2, -viewY, 0);
                                    realView = Matrix.CreateTranslation(-_blockScreenSize.X * 64, (viewY * 32) - 178, 0);
                                }
                            }

                            _spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, realView);
                            _spriteBatch.Draw(_4px, new Rectangle((int)_firstClick.X, (int)_firstClick.Y, (int)_release.X, (int)_release.Y), Color.Black);
                            _playerOneLevel.Draw(gameTime, _spriteBatch);
                            _playerOne.Draw(gameTime, _spriteBatch);
                            _spriteBatch.Draw(_arrow, new Vector2(_playerOne.X * 32 + 640, _playerOne.Y * -32 + 360), null, Color.White, -(float)Math.Atan2(_playerTwo.NextScreen.X - _playerOne.NextScreen.X, _playerTwo.NextScreen.Y - _playerOne.NextScreen.Y) - MathHelper.Pi, new Vector2(125, 125), 1, SpriteEffects.None, 0);
                            _spriteBatch.End();

#if (DEBUG)
                            if (_debugViewVisible)
                                _debugView.RenderDebugData(ref proj, ref view);
#endif
                        }
                        // Render right view port
                        {
                            GraphicsDevice.Viewport = _rightViewPort;
                            _spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null);
                            _spriteBatch.End();

                            Matrix view = Matrix.Identity;
                            Matrix realView = Matrix.Identity;
                            if (_splitScreenMode == SplitScreenMode.HORIZONTAL)
                            {
                                // Render farseer debug overlay
                                float viewX = _playerTwo.X;
                                if (viewX < -_blockScreenSize.X / 4.0f)
                                    viewX = -_blockScreenSize.X / 4.0f;
                                else if (viewX > _blockScreenSize.X / 4.0f && viewX < _blockScreenSize.X / 2.0f)
                                    viewX = _blockScreenSize.X / 4.0f;
                                else if (viewX >= (3 * _blockScreenSize.X) / 2.0 && viewX < (7 * _blockScreenSize.X) / 4.0f)
                                    viewX = (7 * _blockScreenSize.X) / 4.0f;
                                else if (viewX > (9 * _blockScreenSize.X) / 4.0f)
                                    viewX = (9 * _blockScreenSize.X) / 4.0f;
                                view = Matrix.CreateTranslation(-viewX, 0, 0);
                                realView = Matrix.CreateTranslation((-viewX * 32) - 320, 0, 0);
                            }
                            else if (_splitScreenMode == SplitScreenMode.VERTICAL)
                            {
                                float viewY = _playerTwo.Y;
                                if (viewY > _blockScreenSize.Y / 4.0f)
                                    viewY = _blockScreenSize.Y / 4.0f;
                                else if (viewY < -(_blockScreenSize.Y / 4.0f))
                                    viewY = -(_blockScreenSize.Y / 4.0f);

                                if (_playerTwo.VirtualScreen == VirtualScreen.ONE)
                                {
                                    view = Matrix.CreateTranslation(0, -viewY, 0);
                                    realView = Matrix.CreateTranslation(0, (viewY * 32) - 178, 0);
                                }
                                else
                                {
                                    view = Matrix.CreateTranslation(-_blockScreenSize.X * 2, -viewY, 0);
                                    realView = Matrix.CreateTranslation(-_blockScreenSize.X * 64, (viewY * 32) - 178, 0);
                                }
                            }

                            _spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, realView);
                            _spriteBatch.Draw(_4px, new Rectangle((int)_firstClick.X, (int)_firstClick.Y, (int)_release.X, (int)_release.Y), Color.Black);
                            _playerTwoLevel.Draw(gameTime, _spriteBatch);
                            _playerTwo.Draw(gameTime, _spriteBatch);
                            _spriteBatch.Draw(_arrow, new Vector2(_playerTwo.X * 32 + 640, _playerTwo.Y * -32 + 360), null, Color.White, -(float)Math.Atan2(_playerOne.NextScreen.X - _playerTwo.NextScreen.X, _playerOne.NextScreen.Y - _playerTwo.NextScreen.Y) - MathHelper.Pi, new Vector2(125, 125), 1, SpriteEffects.None, 0);
                            _spriteBatch.End();

#if (DEBUG)
                            if (_debugViewVisible)
                                _debugView.RenderDebugData(ref proj, ref view);
#endif
                        }
                        GraphicsDevice.Viewport = _singleViewPort;
                        // Draw dividing line
                        switch (_splitScreenMode)
                        {
                            case SplitScreenMode.HORIZONTAL:
                                _spriteBatch.Begin();
                                _spriteBatch.Draw(_borderVert, Vector2.Zero, Color.White);
                                _spriteBatch.End();
                                break;
                            case SplitScreenMode.VERTICAL:
                                _spriteBatch.Begin();
                                _spriteBatch.Draw(_borderHorz, Vector2.Zero, Color.White);
                                _spriteBatch.End();
                                break;
                            default:
                                break;
                        }
                    }

#if (DEBUG)
                    _spriteBatch.Begin();
                    _spriteBatch.DrawString(_defaultFont, "P1 Current screen: [" + _playerOne.CurrentScreen.X + ", " + _playerOne.CurrentScreen.Y + "] [" + _playerOne.NextScreen.X + ", " + _playerOne.NextScreen.Y + "] " + _playerOne.X + ", " + _playerOne.Y + " (" + (_playerOne.VirtualScreen == VirtualScreen.ONE ? "one" : "two") + ")", Vector2.Zero, Color.White);
                    _spriteBatch.DrawString(_defaultFont, "P2 Current screen: [" + _playerTwo.CurrentScreen.X + ", " + _playerTwo.CurrentScreen.Y + "] [" + _playerOne.NextScreen.X + ", " + _playerOne.NextScreen.Y + "] " + _playerTwo.X + ", " + _playerTwo.Y, new Vector2(0, 20), Color.White);
                    _spriteBatch.DrawString(_defaultFont, "Split screen mode: " + (_splitScreenMode == SplitScreenMode.NONE ? "none" : (_splitScreenMode == SplitScreenMode.HORIZONTAL ? "horizontal" : "vertical")), new Vector2(0, 40), Color.White);
                    _spriteBatch.DrawString(_defaultFont, (Mouse.GetState().X / 32.0f - _blockScreenSize.X / 2.0f).ToString() + ", " + (_blockScreenSize.Y / 2.0f - Mouse.GetState().Y / 32.0f).ToString(), new Vector2(Mouse.GetState().X, Mouse.GetState().Y), Color.White);
                    _spriteBatch.DrawString(_defaultFont, "Player 1 Virtual Screen: " + _playerOne.VirtualScreen, new Vector2(50, 500), Color.White);
                    _spriteBatch.End();
#endif

                    // Draw HUD
                    GraphicsDevice.Viewport = _singleViewPort;
                    _spriteBatch.Begin();

                    // Check if back pressed
                    if (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.Back)
                        || GamePad.GetState(PlayerIndex.Two).IsButtonDown(Buttons.Back)
                        || (CurrentSave.CurrentGameSwitches & GameSwitches.GOT_THING) == GameSwitches.GOT_THING)
                    {
                        string drawString = "Both players must hold BACK to exit the game";
                        _spriteBatch.DrawString(_defaultFont, drawString, new Vector2(GraphicsDevice.Viewport.Width / 2, 150) - (_defaultFont.MeasureString(drawString) / 2), Color.Navy);
                        drawString = "WARNING: All unsaved progress will be lost";
                        _spriteBatch.DrawString(_defaultFont, drawString, new Vector2(GraphicsDevice.Viewport.Width / 2, 170) - (_defaultFont.MeasureString(drawString) / 2), Color.Navy);
                    }
                    else if (_playerOne.SavePointContact || _playerTwo.SavePointContact)
                    {
                        string drawString = "Stand on the SAVE POINT together and hold Y to save the game.";
                        _spriteBatch.DrawString(_defaultFont, drawString, new Vector2(GraphicsDevice.Viewport.Width / 2, 150) - (_defaultFont.MeasureString(drawString) / 2), Color.Navy);
                    }

                    double X = 640 - (_health.MaxHealth / 2.0);
                    double Y = 75;
                    if (_splitScreenMode == SplitScreenMode.VERTICAL)
                    {
                        Y = 350;
                    }
                    _spriteBatch.Draw(_redHealthbar, new Rectangle((int)X, (int)Y, (int)_health.MaxHealth, 20), Color.White);
                    _spriteBatch.Draw(_greenHealthbar, new Rectangle((int)X, (int)Y, (int)_health.CurrentHealth, 20), Color.White);
                    _spriteBatch.DrawString(_defaultFont, "Energy", new Vector2((float)X + (float)(_health.MaxHealth / 2.0) - 31, (float)Y - 5), Color.White);
                    //{
                    //    if (_splitScreenMode == SplitScreenMode.NONE)
                    //    {
                    //        _spriteBatch.Draw(_redHealthbar, new Rectangle(25, 100, (int)_playerOne.MaxHealth, 20), Color.White);
                    //        _spriteBatch.Draw(_greenHealthbar, new Rectangle(25, 100, (int)_playerOne.Health, 20), Color.White);
                    //        _spriteBatch.DrawString(_defaultFont, "Player 1", new Vector2(30, 96), Color.White);
                    //        _spriteBatch.Draw(_redHealthbar, new Rectangle(25, 130, (int)_playerTwo.MaxHealth, 20), Color.White);
                    //        _spriteBatch.Draw(_greenHealthbar, new Rectangle(25, 130, (int)_playerTwo.Health, 20), Color.White);
                    //        _spriteBatch.DrawString(_defaultFont, "Player 2", new Vector2(30, 126), Color.White);
                    //    }
                    //    else if (_splitScreenMode == SplitScreenMode.HORIZONTAL)
                    //    {
                    //        int p1healthbarX;
                    //        int p2healthbarX;

                    //        if (_leftViewPort.X > 0)
                    //        {
                    //            p1healthbarX = 665;
                    //            p2healthbarX = 25;
                    //        }
                    //        else
                    //        {
                    //            p1healthbarX = 25;
                    //            p2healthbarX = 665;
                    //        }

                    //        _spriteBatch.Draw(_redHealthbar, new Rectangle(p1healthbarX, 100, (int)_playerOne.MaxHealth, 20), Color.White);
                    //        _spriteBatch.Draw(_greenHealthbar, new Rectangle(p1healthbarX, 100, (int)_playerOne.Health, 20), Color.White);
                    //        _spriteBatch.DrawString(_defaultFont, "Player 1", new Vector2(p1healthbarX + 5, 96), Color.White);
                    //        _spriteBatch.Draw(_redHealthbar, new Rectangle(p2healthbarX, 100, (int)_playerTwo.MaxHealth, 20), Color.White);
                    //        _spriteBatch.Draw(_greenHealthbar, new Rectangle(p2healthbarX, 100, (int)_playerTwo.Health, 20), Color.White);
                    //        _spriteBatch.DrawString(_defaultFont, "Player 2", new Vector2(p2healthbarX + 5, 96), Color.White);
                    //    }
                    //    else if (_splitScreenMode == SplitScreenMode.VERTICAL)
                    //    {
                    //        int p1healthbarY;
                    //        int p2healthbarY;

                    //        if (_leftViewPort.Y > 0)
                    //        {
                    //            p1healthbarY = 460;
                    //            p2healthbarY = 100;
                    //        }
                    //        else
                    //        {
                    //            p1healthbarY = 100;
                    //            p2healthbarY = 460;
                    //        }

                    //        _spriteBatch.Draw(_redHealthbar, new Rectangle(25, p1healthbarY, (int)_playerOne.MaxHealth, 20), Color.White);
                    //        _spriteBatch.Draw(_greenHealthbar, new Rectangle(25, p1healthbarY, (int)_playerOne.Health / 2 , 20), Color.White);
                    //        _spriteBatch.DrawString(_defaultFont, "Player 1", new Vector2(30, p1healthbarY - 4), Color.White);
                    //        _spriteBatch.Draw(_redHealthbar, new Rectangle(25, p2healthbarY, (int)_playerTwo.MaxHealth, 20), Color.White);
                    //        _spriteBatch.Draw(_greenHealthbar, new Rectangle(25, p2healthbarY, (int)_playerTwo.Health, 20), Color.White);
                    //        _spriteBatch.DrawString(_defaultFont, "Player 2", new Vector2(30, p2healthbarY - 4), Color.White);
                    //    }
                    //}
                    _spriteBatch.End();
                    break;
                default:
                    break;
            }

            base.Draw(gameTime);
        }

        public void DrawLine(Vector2 p1, Vector2 p2, Color colour)
        {
            _spriteBatch.Draw(_4px, new Rectangle((int)p1.X, (int)p1.Y, (int)(p2 - p1).Length(), 1), Color.Black);
        }

        public void UnloadLevel(int X, int Y)
        {
            if (X >= 0 && Y >= 0 && X < _levels.GetLength(0) && Y < _levels.GetLength(1))
            {
                // Dispose old objects
                foreach (GameObject currentObject in _levels[X, Y].StaticObjects)
                {
                    currentObject.Dispose();
                }
                foreach (GameObject currentObject in _levels[X, Y].DynamicObjects)
                {
                    currentObject.Dispose();
                }
            }
        }

        public void LoadLevel(int X, int Y, float Xoffset)
        {
            Level currentLevel = _levels[X, Y];
            currentLevel.Offset = Xoffset;
            if (X >= 0 && Y >= 0 && X < _levels.GetLength(0) && Y < _levels.GetLength(1))
            {
                // Load new objects
                foreach (GameObject currentObject in currentLevel.StaticObjects)
                {
                    currentObject.Create(_world, Xoffset);
                    if (!currentObject.CollideWithPlayer)
                    {
                        foreach (Fixture fixture in currentObject.Fixtures)
                        {
                            foreach (Fixture playerFixture in _playerOne.Fixtures)
                            {
                                playerFixture.CollisionFilter.IgnoreCollisionWith(fixture);
                            }
                            foreach (Fixture playerFixture in _playerTwo.Fixtures)
                            {
                                playerFixture.CollisionFilter.IgnoreCollisionWith(fixture);
                            }
                        }
                    }
                    else
                    {
                        foreach (Fixture fixture in currentObject.IgnoreFixtures)
                        {
                            foreach (Fixture playerFixture in _playerOne.Fixtures)
                            {
                                playerFixture.CollisionFilter.IgnoreCollisionWith(fixture);
                            }
                            foreach (Fixture playerFixture in _playerTwo.Fixtures)
                            {
                                playerFixture.CollisionFilter.IgnoreCollisionWith(fixture);
                            }
                        }
                    }
                }
                foreach (DynamicObject currentObject in currentLevel.DynamicObjects)
                {
                    currentObject.Create(_world, Xoffset);
                    if (!currentObject.CollideWithPlayer)
                    {
                        foreach (Fixture fixture in currentObject.Fixtures)
                        {
                            foreach (Fixture playerFixture in _playerOne.Fixtures)
                            {
                                playerFixture.CollisionFilter.IgnoreCollisionWith(fixture);
                            }
                            foreach (Fixture playerFixture in _playerTwo.Fixtures)
                            {
                                playerFixture.CollisionFilter.IgnoreCollisionWith(fixture);
                            }
                        }
                    }
                    else
                    {
                        foreach (Fixture fixture in currentObject.IgnoreFixtures)
                        {
                            foreach (Fixture playerFixture in _playerOne.Fixtures)
                            {
                                playerFixture.CollisionFilter.IgnoreCollisionWith(fixture);
                            }
                            foreach (Fixture playerFixture in _playerTwo.Fixtures)
                            {
                                playerFixture.CollisionFilter.IgnoreCollisionWith(fixture);
                            }
                        }
                    }
                }
                foreach (DynamicObject dynamicObject in currentLevel.DynamicObjects)
                {
                    foreach (int ignoreID in dynamicObject.IgnoreIDs)
                    {
                        foreach (DynamicObject otherDynamicObject in currentLevel.DynamicObjects)
                        {
                            foreach (Fixture fixture in dynamicObject.Fixtures)
                            {
                                foreach (Fixture otherFixture in otherDynamicObject.Fixtures)
                                {
                                    fixture.CollisionFilter.IgnoreCollisionWith(otherFixture);
                                }
                            }
                        }
                    }
                }
            }
        }

#if (false)
        public void ReloadLevel(int X, int Y)
        {
            string time = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds.ToString();
            string test = _contentBuilder.OutputDirectory;
            _contentBuilder.Clear();
            _contentBuilder.Add("Level\\" + X.ToString() + "_" + Y.ToString() + ".xml", time, null, null);
            string error = _contentBuilder.Build();
            File.Copy("C:\\Temp\\CoopContent\\bin\\content\\" + time + ".xnb", "C:\\Temp\\Games Design\\Coop\\Coop\\Coop\\bin\\x86\\Debug\\Content\\Temp\\" + time + ".xnb");
            foreach (GameObject gObject in _levels[X, Y].DynamicObjects)
                gObject.Dispose();
            foreach (GameObject gObject in _levels[X, Y].StaticObjects)
                gObject.Dispose();
            _levels[X, Y] = Content.Load<Level>("Temp\\" + time); //"Level\\" + X.ToString() + "_" + Y.ToString()
            Level level = _levels[X, Y];
            level.Background = _backgrounds;
            foreach (GameObject gameObject in level.StaticObjects)
            {
                gameObject._textures = _textures;
            }
            foreach (GameObject gameObject in level.DynamicObjects)
            {
                gameObject._textures = _textures;
            }
            List<DynamicObject> children = new List<DynamicObject>();
            // Pass child references to parents
            foreach (DynamicObject dynamicObject in level.DynamicObjects)
            {
                children.Add(dynamicObject);
            }
            foreach (DynamicObject dynamicObject in level.DynamicObjects)
            {
                if (dynamicObject is Parent)
                {
                    foreach (DynamicObject child in children)
                    {
                        foreach (int ID in ((Parent)dynamicObject).ChildIDs)
                        {
                            if (child.ID == ID)
                            {
                                ((Parent)dynamicObject).Children.Add(child);
                            }
                        }
                    }
                }
            }
        }
#endif

        public void UpdatePlayerScreen(Player player, Player player2)
        {
            float minX;
            float maxX;

            if (player.VirtualScreen == VirtualScreen.ONE)
            {
                minX = -_blockScreenSize.X / 2.0f;
                maxX = _blockScreenSize.X / 2.0f;
            }
            else
            {
                minX = (3 * _blockScreenSize.X) / 2.0f;
                maxX = (5 * _blockScreenSize.X) / 2.0f;
            }

            if (player.Position.X < minX)
            {
                if (player.CurrentScreen.X > 0)
                {
                    player.NextScreen -= new Vector2(1, 0);
                    player.X = maxX;
                }
                else
                {
                    player.X = minX;
                    player.MainFixture.Body.LinearVelocity *= new Vector2(0, 1);
                }

                if (player is PlayerOne)
                {
                    foreach (DynamicObject dObject in player._dynamicObjects)
                    {
                        if (dObject is Bullet)
                            dObject.Dispose();
                    }
                }
            }
            else if (player.Position.X > maxX)
            {
                if (player.CurrentScreen.X < MAX_LEVEL_X - 1)
                {
                    player.NextScreen += new Vector2(1, 0);
                    player.X = minX;
                }
                else
                {
                    player.X = maxX;
                    player.MainFixture.Body.LinearVelocity *= new Vector2(0, 1);
                }

                if (player is PlayerOne)
                {
                    foreach (DynamicObject dObject in player._dynamicObjects)
                    {
                        if (dObject is Bullet)
                            dObject.Dispose();
                    }
                }
            }

            if (player.Position.Y < -_blockScreenSize.Y / 2)
            {
                if (player.CurrentScreen.Y < MAX_LEVEL_Y - 1)
                {
                    player.NextScreen += new Vector2(0, 1);
                    player.Y = _blockScreenSize.Y / 2;
                }
                else
                {
                    player.Y = -_blockScreenSize.Y / 2;
                    player.MainFixture.Body.LinearVelocity *= new Vector2(1, 0);
                }
            }
            else if (player.Position.Y > _blockScreenSize.Y / 2)
            {
                if (player.CurrentScreen.Y > 0)
                {
                    player.NextScreen -= new Vector2(0, 1);
                    player.Y = -_blockScreenSize.Y / 2;
                }
                else
                {
                    player.Y = _blockScreenSize.Y / 2;
                    player.MainFixture.Body.LinearVelocity *= new Vector2(1, 0);
                }
            }
        }

        public void UpdateView(Player player, Player player2)
        {
            if (player.NextScreen.X < player2.NextScreen.X)
            {
                _leftViewPort.Height = _graphics.PreferredBackBufferHeight;
                _rightViewPort.Height = _graphics.PreferredBackBufferHeight;
                _leftViewPort.Width = _graphics.PreferredBackBufferWidth / 2;
                _rightViewPort.Width = _graphics.PreferredBackBufferWidth / 2;
                _leftViewPort.X = 0;
                _rightViewPort.X = _graphics.PreferredBackBufferWidth / 2;
                _leftViewPort.Y = 0;
                _rightViewPort.Y = 0;
                _splitScreenMode = SplitScreenMode.HORIZONTAL;
                proj = Matrix.CreateOrthographic(_blockScreenSize.X / 2, _blockScreenSize.Y, 0, 1);
            }
            else if (player.NextScreen.X > player2.NextScreen.X)
            {
                _leftViewPort.Height = _graphics.PreferredBackBufferHeight;
                _rightViewPort.Height = _graphics.PreferredBackBufferHeight;
                _leftViewPort.Width = _graphics.PreferredBackBufferWidth / 2;
                _rightViewPort.Width = _graphics.PreferredBackBufferWidth / 2;
                _leftViewPort.X = _graphics.PreferredBackBufferWidth / 2;
                _rightViewPort.X = 0;
                _leftViewPort.Y = 0;
                _rightViewPort.Y = 0;
                _splitScreenMode = SplitScreenMode.HORIZONTAL;
                proj = Matrix.CreateOrthographic(_blockScreenSize.X / 2, _blockScreenSize.Y, 0, 1);
            }
            else if (player.NextScreen.Y < player2.NextScreen.Y)
            {
                _leftViewPort.Height = _graphics.PreferredBackBufferHeight / 2;
                _rightViewPort.Height = _graphics.PreferredBackBufferHeight / 2;
                _leftViewPort.Width = _graphics.PreferredBackBufferWidth;
                _rightViewPort.Width = _graphics.PreferredBackBufferWidth;
                _leftViewPort.X = 0;
                _rightViewPort.X = 0;
                _leftViewPort.Y = 0;
                _rightViewPort.Y = _graphics.PreferredBackBufferHeight / 2;
                _splitScreenMode = SplitScreenMode.VERTICAL;
                proj = Matrix.CreateOrthographic(_blockScreenSize.X, _blockScreenSize.Y / 2, 0, 1);
            }
            else if (player.NextScreen.Y > player2.NextScreen.Y)
            {
                _leftViewPort.Height = _graphics.PreferredBackBufferHeight / 2;
                _rightViewPort.Height = _graphics.PreferredBackBufferHeight / 2;
                _leftViewPort.Width = _graphics.PreferredBackBufferWidth;
                _rightViewPort.Width = _graphics.PreferredBackBufferWidth;
                _leftViewPort.X = 0;
                _rightViewPort.X = 0;
                _leftViewPort.X = 0;
                _rightViewPort.X = 0;
                _leftViewPort.Y = _graphics.PreferredBackBufferHeight / 2;
                _rightViewPort.Y = 0;
                _splitScreenMode = SplitScreenMode.VERTICAL;
                proj = Matrix.CreateOrthographic(_blockScreenSize.X, _blockScreenSize.Y / 2, 0, 1);
            }
            else
            {
                _splitScreenMode = SplitScreenMode.NONE;
                proj = Matrix.CreateOrthographic(_blockScreenSize.X, _blockScreenSize.Y, 0, 1);
            }
        }
    }

    public enum SplitScreenMode
    {
        NONE,
        HORIZONTAL,
        VERTICAL
    }

    public enum GameState
    {
        MAINMENU,
        LOAD,
        PLAY
    }

    public enum MenuState
    {
        MAIN,
        NEWLOAD,
        NEW,
        LOAD,
        INSTRUCTIONS,
        GAMEOVER
    }
}
