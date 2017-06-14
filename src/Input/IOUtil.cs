//
// IOUtil.cs
// MB2D Engine
//
// ---------------------------------------------------
//
// Create by Jacob Milligan on 12/09/2016.
// Copyright (c) Jacob Milligan 2016. All rights reserved.
//


using System;
using Microsoft.Xna.Framework.Input;

namespace MB2D.IO
{
  /// <summary>
  /// Utility methods for working with the keyboard and mouse
  /// </summary>
  public static class IOUtil
  {
    /// <summary>
    /// The last known state of the keyboard
    /// </summary>
    private static KeyboardState _previousState;

    /// <summary>
    /// The last known state of the mouse
    /// </summary>
    private static MouseState _lastMouseState;

    /// <summary>
    /// Checks if the specific key was typed (aka not pressed down in the previous frame).
    /// </summary>
    /// <returns><c>true</c>, if the key was up in the last frame and down in the current one,
    /// <c>false</c> otherwise.</returns>
    /// <param name="key">Key.</param>
    public static bool KeyTyped(Keys key)
    {
      var result = false;
      var nextState = Keyboard.GetState();
      if ( nextState.IsKeyDown(key) && _previousState.IsKeyUp(key) ) {
        result = true;
      }
      return result;
    }

    /// <summary>
    /// Checks if the left mouse was clicked this frame and not held down
    /// </summary>
    /// <returns><c>true</c>, if left mouse was clicked, <c>false</c> otherwise.</returns>
    public static bool LeftMouseClicked()
    {
      var result = false;
      var nextState = Mouse.GetState();
      if ( nextState.LeftButton == ButtonState.Pressed && _lastMouseState.LeftButton == ButtonState.Released ) {
        result = true;
      }
      return result;
    }

    /// <summary>
    /// Updates the known keyboard state. Called once per frame.
    /// </summary>
    public static void UpdateKeyState()
    {
      _previousState = Keyboard.GetState();
    }

    /// <summary>
    /// Updates the known mouse state. Called once per frame.
    /// </summary>
    public static void UpdateMouseState()
    {
      _lastMouseState = Mouse.GetState();
    }

    /// <summary>
    /// Converts a number of strings into a single CSV formatted line
    /// </summary>
    /// <returns>The CSV formatted line.</returns>
    /// <param name="values">Values to take and format into a single line.</param>
    public static string LineToCSV(params string[] values)
    {
      var result = string.Empty;
      for ( int i = 0; i < values.Length; i++ ) {
        if ( values[i].Contains(",") ) {
          Console.WriteLine("CSV parse error: Value '{0}' contains illegal character ','", values[i]);
          result = null;
          break;
        }
        result += values[i];
        if ( i < values.Length - 1 ) {
          result += ",";
        }
      }
      return result;
    }

    /// <summary>
    /// Gets the last key typed.
    /// </summary>
    /// <value>The last key typed.</value>
    public static Keys LastKeyTyped
    {
      get
      {
        var result = Keys.None;
        var keyStates = Keyboard.GetState();
        foreach ( var k in keyStates.GetPressedKeys() ) {
          if ( KeyTyped(k) ) {
            result = k;
            break;
          }
        }
        return result;
      }
    }

    public static bool KeyDown(Keys key)
    {
      return Keyboard.GetState().IsKeyDown(key);
    }

    /// <summary>
    /// Gets the last key held down.
    /// </summary>
    /// <value>The last key held down.</value>
    public static Keys LastKeyDown
    {
      get
      {
        var result = Keys.None;
        var keyStates = Keyboard.GetState();
        foreach ( var k in keyStates.GetPressedKeys() ) {
          if ( keyStates.IsKeyDown(k) ) {
            result = k;
            break;
          }
        }
        return result;
      }
    }

    public static Keys[] PreviousKeyboardState
    {
      get { return _previousState.GetPressedKeys(); }
    }

    public static Keys[] KeyboardState
    {
      get { return Keyboard.GetState().GetPressedKeys(); }
    }
  }
}
