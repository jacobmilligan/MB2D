//
// ConsoleCommand.cs
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
  /// Shows or hides the debug console
  /// </summary>
  public class ConsoleCommand : Command
  {

    /// <summary>
    /// Initializes a new instance of the <see cref="T:MB2D.IO.ConsoleCommand"/> class.
    /// </summary>
    /// <param name="key">Key to assign the command to.</param>
    /// <param name="type">Type of command trigger.</param>
    public ConsoleCommand(Keys key, CommandType type) : base(key, type) { }

    /// <summary>
    /// Toggles the debug console open/closed
    /// </summary>
    /// <param name="e">Entity with the controller component. Unused.</param>
    protected override void OnKeyPress(Entity e = null)
    {
      MBGame.Console.Toggle();
    }
  }
}

