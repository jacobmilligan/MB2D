//
// Command.cs
// MB2D Engine
//
// ---------------------------------------------------
//
// Create by Jacob Milligan on 12/09/2016.
// Copyright (c) Jacob Milligan 2016. All rights reserved.
//

using Microsoft.Xna.Framework.Input;
using MB2D.EntityComponent;

namespace MB2D.IO
{

  /// <summary>
  /// Represents either a trigger or hold command type
  /// </summary>
  public enum CommandType
  {
    /// <summary>
    /// Execute command every frame its associated input
    /// key/button is detected
    /// </summary>
    Hold,

    /// <summary>
    /// Execute command only on the first frame its associated
    /// input key/button is detected and don't execute again until
    /// it's released and pressed again
    /// </summary>
    Trigger
  }

  /// <summary>
  /// Executes an action associated with a specific key
  /// </summary>
  public abstract class Command
  {
    /// <summary>
    /// The key code associated with the command
    /// </summary>
    private Keys _key;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:MidnightBlue.Command"/> class.
    /// </summary>
    /// <param name="key">Key to associate with the command</param>
    /// <param name="commandType">Trigger or hold command</param>
    protected Command(Keys key, CommandType commandType)
    {
      Type = commandType;
      _key = key;
      Disabled = false;
    }

    /// <summary>
    /// Executes the specific command on the entity parameter
    /// </summary>
    /// <param name="e">Entity to operate on. Optional</param>
    public bool Execute(Entity e = null)
    {
      if ( Disabled )
        return false;

      if ( Type == CommandType.Hold && Keyboard.GetState().IsKeyDown(_key) ) {
        OnKeyPress(e);
        return true;
      }

      if ( Type == CommandType.Trigger && IOUtil.KeyTyped(_key) ) {
        OnKeyPress(e);
        return true;
      }

      return false;
    }

    /// <summary>
    /// Defines the logic to perform when operating on a given entity
    /// </summary>
    /// <param name="e">Entity to operate on</param>
    protected abstract void OnKeyPress(Entity e = null);

    /// <summary>
    /// Gets or sets the trigger type of the command.
    /// </summary>
    /// <value>The command type</value>
    public CommandType Type { get; set; }

    /// <summary>
    /// Gets the keycode associated with the command.
    /// </summary>
    /// <value>The key code</value>
    public Keys Key
    {
      get { return _key; }
    }

    public bool Disabled { get; set; }
  }
}

