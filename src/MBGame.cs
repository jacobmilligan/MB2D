using Microsoft.Xna.Framework;
using MonoGame.Extended;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MB2D.Scenes;
using MB2D.EntityComponent;
using MB2D.IO;
using MB2D.Testing;
using MonoGame.Extended.Shapes;
using System;

namespace MB2D
{
	/// <summary>
	/// This is the main type for your game.
	/// </summary>
	public class MBGame : Game
	{
		private TextInputHandler _textHandler;
		private string _lastInput;

		/// <summary>
		/// The main graphics adapter used in the game.
		/// </summary>
		private static GraphicsDeviceManager _graphics;

		/// <summary>
		/// Holds the current frames per second of the game.
		/// </summary>
		private static FramesPerSecondCounter _fps;

		/// <summary>
		/// Delta Time. Holds the time it took to complete the last frame.
		/// </summary>
		private static float _dt;

		/// <summary>
		/// The main 2D camera used in the game. Other cameras can be used
		/// but this should always be active.
		/// </summary>
		private static Camera2D _camera;

		/// <summary>
		/// The SpriteBatch for drawing all world-based textures
		/// </summary>
		private SpriteBatch _spriteBatch,
	/// <summary>
	/// SpriteBatch for drawing all screen-based textures
	/// </summary>
	_uiSpriteBatch;

		/// <summary>
		/// Color used when refreshing the window
		/// </summary>
		private Color _bgColor;

		/// <summary>
		/// The games debug console. Static so as to be accessed globally for adding functions/variables
		/// to be tracked and altered via the console
		/// </summary>
		private static MBConsole _debugConsole;

		/// <summary>
		/// The scene stack in the current game. The scene at the top
		/// is always drawn and updated from the main loop
		/// </summary>
		private SceneStack _scenes;

		/// <summary>
		/// All entities known by the game, passed around as a reference to new scenes.
		/// </summary>
		private EntityMap _gameObjects;

		private Type _initScene;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:MB2D.MBGame"/> class and
		/// defines essential graphics settings.
		/// </summary>
		public MBGame(Type initSceneType)
		{
			_graphics = new GraphicsDeviceManager(this);
			_scenes = new SceneStack(); // main scene stack
			_gameObjects = new EntityMap();
			_fps = new FramesPerSecondCounter();
			_textHandler = new TextInputHandler();
			_lastInput = string.Empty;
			_initScene = initSceneType;

			ForceQuit = false;
			Content.RootDirectory = "Content";

			// 720p
			_graphics.PreferredBackBufferWidth = 1280;
			_graphics.PreferredBackBufferHeight = 720;
			_graphics.ApplyChanges();
		}

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize()
		{

			this.IsMouseVisible = true; // show mouse

			// Setup main viewport for camera use
			_graphics.GraphicsDevice.Viewport = new Viewport
			{
				X = 0,
				Y = 0,
				Width = _graphics.PreferredBackBufferWidth,
				Height = _graphics.PreferredBackBufferHeight,
			};

			// Initialize camera at viewport location
			_camera = new Camera2D(Graphics);
			_camera.LookAt(new Vector2(0, 0));

			base.Initialize(); // MonoGame setup

			_bgColor = Color.Black;
			Window.Title = "MB2D Engine";

			SetUpDebugVals();
			RegisterSystems();

			_scenes.ResetTo((Scene)Activator.CreateInstance(_initScene, _gameObjects, Content));
		}

		/// <summary>
		/// Loads content at the beginning of the game
		/// </summary>
		protected override void LoadContent()
		{
			_spriteBatch = new SpriteBatch(GraphicsDevice);
			_uiSpriteBatch = new SpriteBatch(GraphicsDevice);

			_debugConsole = new MBConsole(Color.Black, Color.Yellow, Content.Load<SpriteFont>("Fonts/SourceCode"));
			_debugConsole.InitWindow(Graphics);
		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update(GameTime gameTime)
		{
			// Update frame time
			_dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
			_fps.Update(gameTime);

			// Update the current scene
			if ( _scenes.Top != null )
			{
				_bgColor = _scenes.Top.WindowBackgroundColor;
				_scenes.Top.DeltaTime = _dt;
			}

			// Check for quit request
			if ( ForceQuit )
			{
				Exit();
			}

			_scenes.Update();

			_debugConsole.Update();

			base.Update(gameTime); // Monogame update
		}

		/// <summary>
		/// Draws the current scene to the window
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			_graphics.GraphicsDevice.Clear(_bgColor);

			_uiSpriteBatch.Begin();

			// Setup to draw relative to the cameras view matrix
			// and clamp coords to int points to avoid weird floating
			// point stutters
			_spriteBatch.Begin(
			  transformMatrix: _camera.GetViewMatrix(),
			  samplerState: SamplerState.PointClamp
			);

			// Draw the current scene
			if ( _scenes.Top != null )
			{
				_scenes.Draw(_spriteBatch, _uiSpriteBatch);
			}

			CheckDebugVars();

			if ( !ForceQuit )
				_debugConsole.Draw(_uiSpriteBatch);

			_spriteBatch.End();
			_uiSpriteBatch.End();

			// Update IO states
			IOUtil.UpdateKeyState();
			IOUtil.UpdateMouseState();

			base.Draw(gameTime);
		}

		/// <summary>
		/// Checks if the debug variables are set and then executes appropriate render
		/// logic 
		/// </summary>
		private void CheckDebugVars()
		{
			if ( (bool)_debugConsole.Vars["showKeys"] )
			{
				var lastChar = _textHandler.LastChar;

				if ( lastChar != '\0' )
				{
					_lastInput = lastChar.ToString();

					if ( _textHandler.IsSpecialChar(lastChar) )
					{
						_lastInput = _textHandler.EscapeString(lastChar);
					}
				}

				_uiSpriteBatch.DrawString(
				  Content.Load<SpriteFont>("Fonts/SourceCode"),
				  "Last char: " + _lastInput,
				  new Vector2(450, 0),
				  Color.White
				);
			}

			// Draw framerate
			if ( (bool)_debugConsole.Vars["showFramerate"] )
			{
				_uiSpriteBatch.DrawString(
				  Content.Load<SpriteFont>("Fonts/SourceCode"),
				  _fps.FramesPerSecond.ToString("0"),
				  new Vector2(0, 0),
				  Color.White
				);

				// Draw DT
				_uiSpriteBatch.DrawString(
				  Content.Load<SpriteFont>("Fonts/SourceCode"),
				  _dt.ToString(),
				  new Vector2(50, 0),
				  Color.White
				);
			}

			// Draw camera position
			if ( (bool)_debugConsole.Vars["showCameraPos"] )
			{
				_uiSpriteBatch.DrawString(
				  Content.Load<SpriteFont>("Fonts/SourceCode"),
				  Camera.Position.ToString(),
				  new Vector2(550, 0),
				  Color.White
				);
			}

			// Draw amount of collision checks last frame
			if ( (bool)_debugConsole.Vars["collisionChecks"] )
			{
				var collision = _gameObjects.GetSystem<CollisionSystem>() as CollisionSystem;
				_uiSpriteBatch.DrawString(
				  Content.Load<SpriteFont>("Fonts/SourceCode"),
				  "Collision checks: " + collision.NumberOfChecks,
				  new Vector2(200, 0),
				  Color.White
				);
			}

			// Draw the collision grid and boxes
			if ( (bool)_debugConsole.Vars["drawCollision"] )
			{
				_gameObjects.GetSystem<CollisionRenderSystem>().Run();
				var collisionMap = (_gameObjects.GetSystem<CollisionSystem>() as CollisionSystem).CurrentMap;
				if ( collisionMap != null )
				{
					_spriteBatch.DrawGrid(collisionMap.Grid, Camera.Position.ToPoint(), Color.White);
				}
			}
		}

		/// <summary>
		/// Sets up all the initial debug console variables and functions used in the game.
		/// </summary>
		private void SetUpDebugVals()
		{
			// Add all variables
			_debugConsole.AddVar("showFramerate", false);
			_debugConsole.AddVar("drawBorders", false);
			_debugConsole.AddVar("drawGrids", false);
			_debugConsole.AddVar("drawCollision", false);
			_debugConsole.AddVar("collisionChecks", false);
			_debugConsole.AddVar("systemRuntime", false);
			_debugConsole.AddVar("showCameraPos", false);
			_debugConsole.AddVar("showKeys", false);

			// Add all functions
			_debugConsole.AddFunc("ToggleFullscreen", (string[] args) => _graphics.ToggleFullScreen());
			_debugConsole.AddFunc("PopScene", (string[] args) => _scenes.Pop());

			_debugConsole.AddFunc("TestUI", (string[] args) => _scenes.Push(new UITest(_gameObjects, Content)));
		}

		/// <summary>
		/// Registers all EntitySystems used in the engine
		/// </summary>
		public void RegisterSystems()
		{
			_gameObjects.AddSystem<InputSystem>();
			_gameObjects.AddSystem<MovementSystem>();
			_gameObjects.AddSystem<CollisionSystem>();
			_gameObjects.AddSystem<PhysicsSystem>();
			_gameObjects.AddSystem<DepthSystem>();
			_gameObjects.AddSystem<RenderSystem>(_spriteBatch);
			_gameObjects.AddSystem<CollisionRenderSystem>(_spriteBatch);
		}

		/// <summary>
		/// Gets the main graphics device.
		/// </summary>
		/// <value>The graphics device.</value>
		public static GraphicsDevice Graphics
		{
			get { return _graphics.GraphicsDevice; }
		}

		/// <summary>
		/// Gets the main camera.
		/// </summary>
		/// <value>The main camera.</value>
		public static Camera2D Camera
		{
			get { return _camera; }
		}

		/// <summary>
		/// Gets the debug console for reading and writing to. There should only ever be one of these
		/// </summary>
		/// <value>The debug console.</value>
		public static MBConsole Console
		{
			get { return _debugConsole; }
		}

		/// <summary>
		/// Gets time it took to complete the last frame
		/// </summary>
		/// <value>The delta time.</value>
		public static float DeltaTime
		{
			get { return _dt; }
		}

		/// <summary>
		/// Gets the current average frames per second
		/// </summary>
		/// <value>The fps.</value>
		public static float FPS
		{
			get { return _fps.FramesPerSecond; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="T:MB2D.MBGame"/> should quit.
		/// </summary>
		/// <value><c>true</c> if the game should quit; otherwise, <c>false</c>.</value>
		public static bool ForceQuit { get; set; }

		public EntityMap GameObjects => _gameObjects;

		public SpriteBatch SpriteBatch => _spriteBatch;

		public Scene TopScene => _scenes.Top;
	}
}

