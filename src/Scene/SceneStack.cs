//
// SceneStack.cs
// MB2D Engine
//
// ---------------------------------------------------
//
// Create by Jacob Milligan on 12/09/2016.
// Copyright (c) Jacob Milligan 2016. All rights reserved.
//

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MB2D.Scenes
{
  /// <summary>
  /// Holds the games scenes in a stack structure running the top scene every frame. Handles
  /// switching state for scenes and popping/pushing new scenes on top of one another. Allows
  /// the current scene to access other scenes.
  /// </summary>
  public class SceneStack
  {
    /// <summary>
    /// The scene stack. Uses a list instead of a stack to allow accessing scenes at specific
    /// indexes easily.
    /// </summary>
    private List<Scene> _scenes;

    /// <summary>
    /// The next scene the stack should send to the top. Used during transitions only.
    /// </summary>
    private Scene _nextScene;

    private Type _lastType;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:MB2D.Scenes.SceneStack"/> class.
    /// </summary>
    public SceneStack()
    {
      _scenes = new List<Scene>();
    }

    /// <summary>
    /// Pushes a new scene to the top of the stack. Calls the new scenes Initialize method and the
    /// previous scenes Pause method
    /// </summary>
    /// <param name="scene">Scene to push.</param>
    public void Push(Scene scene)
    {
      scene.SceneController = this;
      // Pause the previous scene if it exists
      if ( _scenes.Count > 0 ) {
        Top.TransitionState = TransitionState.Pausing;
      }

      _nextScene = scene;
      if ( _nextScene.TransitionState == TransitionState.Null ) {
        _nextScene.TransitionState = TransitionState.Initializing;
      }
    }

    public void Draw(SpriteBatch spriteBatch, SpriteBatch uiSpriteBatch)
    {
      if ( Top.TransitionState != Top.PreviousTransitionState ) {
        return;
      }

      Top.Draw(spriteBatch, uiSpriteBatch);
    }

    /// <summary>
    /// Updates the scene at the top of the stack and handles any
    /// state transitions if they've been called. For any transitions or scene logic to function
    /// correctly this must be called once per frame.
    /// </summary>
    public void Update()
    {

      if ( _scenes.Count > 0 ) {
        HandleState(Top);
        // Handle transitions if there's a new scene to add
        if ( _nextScene != null ) {
          HandleState(_nextScene);
        }
      } else {
        // Handle transitions if there's a new scene to add
        if ( _nextScene != null ) {
          HandleState(_nextScene);
        } else {
          MBGame.ForceQuit = true; // quit game when no scenes on the stack
        }
      }

      if ( Top != null ) {
        Top.UpdateTransition();
        Top.HandleInput();
        Top.Update();
      }
    }

    /// <summary>
    /// Pops the top scene off the stack, calling its Exit method and calls the Resume
    /// method of the next scene on the stack if it exists.
    /// </summary>
    public void Pop()
    {
      _lastType = Top.GetType();
      if ( _scenes.Count > 0 ) {
        if ( Top.TransitionState != TransitionState.Null ) {
          Top.TransitionState = TransitionState.Exiting;
        }
        // Resume next available scene
        if ( _scenes.Count > 1 ) {
          _nextScene = _scenes[_scenes.Count - 2];
          _nextScene.TransitionState = TransitionState.Resuming;
        }
      } else {
        MBGame.Console.Debug("MB2D Engine: Cannot pop scene from empty stack");
      }
    }

    /// <summary>
    /// Resets the scene stack to the specified scene, clearing all other scenes
    /// from the stack. Use this in most scenarios instead of Push to save memory
    /// by not keeping scenes allocated if unnecessary.
    /// </summary>
    /// <param name="scene">Scene to reset to.</param>
    public void ResetTo(Scene scene)
    {
      if ( _scenes.Count > 0 ) {
        _nextScene = scene;
        var currentScene = Top;
        // Pop all the scenes off the stack and clean them up
        while ( _scenes.Count > 0 ) {
          Pop();
          Update();
        }
      }
      Push(scene);
    }

    /// <summary>
    /// Gets the scene located at the specific index in the stack
    /// </summary>
    /// <returns>The <see cref="T:MB2D.Scenes.Scene"/>.</returns>
    /// <param name="index">Index to get.</param>
    public Scene SceneAt(int index)
    {
      Scene result = null;
      if ( index >= 0 && index < _scenes.Count ) {
        result = _scenes[index];
      }
      return result;
    }

    /// <summary>
    /// Handles all state transitions between scenes. This must be called once per frame for 
    /// it to function correctly but is handles automatically in the stacks Update method.
    /// </summary>
    /// <param name="scene">Scene to handle the state changes for.</param>
    private void HandleState(Scene scene)
    {
      var last = scene.PreviousTransitionState;
      var current = scene.TransitionState;

      // Handle the transition only if the top scene has changed state
      if ( last != current ) {

        // Handles:
        // * Null -> Initialize (new scene)
        // * Null -> None (intialize to normal)
        if ( last == TransitionState.Null ) {
          switch ( current ) {
            case TransitionState.Initializing:
              scene.Initialize();
              break;
            case TransitionState.None:
              _scenes.Add(_nextScene);
              _nextScene = null;
              break;
            default:
              Console.WriteLine("Transition from '{0}' to '{1}' is invalid", last, current);
              break;
          }
        }

        // Handles:
        // * None -> Null (exiting)
        // * None -> Pausing (enter pause)
        // * None -> Resuming (enter resume)
        // * None -> Exiting (enter exit)
        if ( last == TransitionState.None ) {
          switch ( current ) {
            case TransitionState.Null:
              if ( _scenes.Count > 0 ) {
                _scenes.RemoveAt(_scenes.Count - 1);
              }
              break;
            case TransitionState.Pausing:
              scene.Pause();
              break;
            case TransitionState.Resuming:
              scene.Resume();
              break;
            case TransitionState.Exiting:
              scene.Exit();
              break;
            default:
              Console.WriteLine("Transition from '{0}' to '{1}' is invalid", last, current);
              break;
          }
        }
        // Handles:
        // * Pause -> None
        if ( last == TransitionState.Pausing ) {
          if ( current == TransitionState.None ) {
            return;
          }
        }

        // Handles:
        // * Resuming -> None
        if ( last == TransitionState.Resuming ) {
          if ( current == TransitionState.None && _scenes.Count > 1 ) {
            _scenes.RemoveAt(_scenes.Count - 1);
          }
        }

        // Handles:
        // * Initializing -> None
        if ( last == TransitionState.Initializing ) {
          if ( current == TransitionState.None ) {
            _scenes.Add(_nextScene);
            _nextScene = null;
          }
        }

        // Handles:
        // * Exiting -> Null (destroy scene)
        if ( last == TransitionState.Exiting ) {
          if ( current == TransitionState.Null ) {
            _scenes.RemoveAt(_scenes.Count - 1);
          }
        }
      } else {
        // No state change has happened so call the last known states
        // appropriate method
        switch ( current ) {
          case TransitionState.Null:
            if ( _scenes.Count > 0 ) {
              _scenes.RemoveAt(_scenes.Count - 1);
            }
            break;
          case TransitionState.None:
            break;
          case TransitionState.Pausing:
            scene.Pause();
            break;
          case TransitionState.Resuming:
            scene.Resume();
            break;
          case TransitionState.Exiting:
            scene.Exit();
            break;
          case TransitionState.Initializing:
            scene.Initialize();
            break;
          default:
            Console.WriteLine("Transition from '{0}' to '{1}' is invalid", last, current);
            break;
        }
      }
    }

    /// <summary>
    /// Gets the scene at the top of the stack.
    /// </summary>
    /// <value>The scene at the top of the stack.</value>
    public Scene Top
    {
      get
      {
        Scene result = null;
        if ( _scenes.Count > 0 ) {
          result = _scenes[_scenes.Count - 1];
        }
        return result;
      }
    }

    /// <summary>
    /// Gets the scene at the bottom of the stack.
    /// </summary>
    /// <value>The bottom scene.</value>
    public Scene Bottom
    {
      get { return _scenes[0]; }
    }

    /// <summary>
    /// Gets the current size of the stack
    /// </summary>
    /// <value>The size.</value>
    public int Size
    {
      get { return _scenes.Count; }
    }

    /// <summary>
    /// Gets the upper bounds of the indexes of the stack
    /// </summary>
    /// <value>The last index.</value>
    public int LastIndex
    {
      get { return _scenes.Count - 1; }
    }

    public Scene Next
    {
      get { return _nextScene; }
    }

    public Type LastSceneType
    {
      get { return _lastType; }
    }
  }
}

