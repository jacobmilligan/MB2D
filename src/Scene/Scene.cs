//
// Scene.cs
// MB2D Engine
//
// ---------------------------------------------------
//
// Create by Jacob Milligan on 12/09/2016.
// Copyright (c) Jacob Milligan 2016. All rights reserved.
//


using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MB2D.EntityComponent;

namespace MB2D.Scenes
{
  /// <summary>
  /// Defines a valid transition state to move into. Once set, the scene stack will automatically
  /// move the current scene into that state the next frame.
  /// </summary>
  public enum TransitionState
  {
    /// <summary>
    /// The scene hasn't intitialized yet
    /// </summary>
    Null,
    /// <summary>
    /// The normal state
    /// </summary>
    None,
    /// <summary>
    /// The scene is currently pausing. Set state to None to end transition.
    /// </summary>
    Pausing,
    /// <summary>
    /// The scene is resuming from the paused state. Set state to None to end transition.
    /// </summary>
    Resuming,
    /// <summary>
    /// The scene is exiting to be destroyed. Set state to Null to end transition.
    /// </summary>
    Exiting,
    /// <summary>
    /// The scene is initializing from the an unconstructed state. Set state to None to end.
    /// </summary>
    Initializing
  }

  /// <summary>
  /// Holds all logic and data for a single game screen
  /// </summary>
  public abstract class Scene
  {
    private TransitionState _lastState;
    /// <summary>
    /// The scenes EntityMap
    /// </summary>
    private EntityMap _gameObjects;

    private ContentManager _content;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:MB2D.Scenes.Scene"/> class
    /// </summary>
    private Scene(ContentManager content)
    {
      _gameObjects = new EntityMap();
      WindowBackgroundColor = Color.MidnightBlue;
      _lastState = TransitionState.Null;
      TransitionState = TransitionState.Initializing;
      _content = content;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:MB2D.Scenes.Scene"/> class
    /// with a pre-existing EntityMap
    /// </summary>
    /// <param name="gameObjects">EntityMap to assign to the scene.</param>
    public Scene(EntityMap gameObjects, ContentManager content)
    {
      _content = content;
      if ( gameObjects == null ) {
        _gameObjects = new EntityMap();
        return;
      }

      _gameObjects = new EntityMap(gameObjects);
      _gameObjects.Clear();
    }

    public void UpdateTransition()
    {
      _lastState = TransitionState;
    }

    /// <summary>
    /// Initialize this scene and loads all resources. Runs logic to execute 
    /// during the Initializing state. Set state to None to end.
    /// </summary>
    public abstract void Initialize();

    /// <summary>
    /// Handles all input for the scene
    /// </summary>
    public abstract void HandleInput();

    /// <summary>
    /// Updates game logic and changes scene state.
    /// </summary>
    public abstract void Update();

    /// <summary>
    /// Runs logic to execute while the scene is in the Pausing state. Set state to None to end.
    /// </summary>
    public abstract void Pause();

    /// <summary>
    /// Runs logic to execute while the scene is in the Resuming state. Set state to None to end.
    /// </summary>
    public abstract void Resume();

    /// <summary>
    /// Draws entities and UI elements to the specfied SpriteBatches
    /// </summary>
    /// <param name="spriteBatch">World-coordinate based sprite batch.</param>
    /// <param name="uiSpriteBatch">Camera-based User Interface sprite batch.</param>
    public abstract void Draw(SpriteBatch spriteBatch, SpriteBatch uiSpriteBatch);

    /// <summary>
    /// Runs logic to execute while the scene is in the Exiting state. Set state to Null to end.
    /// </summary>
    public abstract void Exit();

    /// <summary>
    /// Cleans up the scene and unloads content.
    /// </summary>
    public void Cleanup()
    {
      _gameObjects.Clear();
      Content.Unload();
    }

    /// <summary>
    /// Gets all entities allocated to the scene
    /// </summary>
    /// <value>The game objects.</value>
    public EntityMap GameObjects
    {
      get { return _gameObjects; }
    }

    /// <summary>
    /// Gets or sets the current transition state of the scene. This causes
    /// the scene stack to change the scenes state on the next frame.
    /// </summary>
    /// <value>The transition state.</value>
    public TransitionState TransitionState { get; set; }

    /// <summary>
    /// Gets the state the scene was in during the last frame.
    /// </summary>
    /// <value>The state of the previous transition.</value>
    public TransitionState PreviousTransitionState
    {
      get { return _lastState; }
    }

    /// <summary>
    /// Gets or sets the color of the window background for this scene.
    /// </summary>
    /// <value>The color of the window background.</value>
    public Color WindowBackgroundColor { get; set; }

    /// <summary>
    /// Gets or sets the scene controller.
    /// </summary>
    /// <value>The scene controller.</value>
    public SceneStack SceneController { get; set; }

    /// <summary>
    /// Gets the content manager for loading and unloading resources.
    /// </summary>
    /// <value>The content manager.</value>
    protected ContentManager Content
    {
      get { return _content; }
    }

    /// <summary>
    /// Gets or sets the delta time value.
    /// </summary>
    /// <value>The delta time.</value>
    public float DeltaTime { get; set; }
  }
}

