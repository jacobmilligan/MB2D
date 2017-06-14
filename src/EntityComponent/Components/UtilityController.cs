//
// PlayerController.cs
// MB2D Engine
//
// ---------------------------------------------------
//
// Create by Jacob Milligan on 12/09/2016.
// Copyright (c) Jacob Milligan 2016. All rights reserved.
//

using Microsoft.Xna.Framework.Input;
using MB2D.IO;

namespace MB2D.EntityComponent
{
  /// <summary>
  /// Declares the attached entity as able to control utility 
  /// commands such as opening the debug console
  /// </summary>
  public class UtilityController : IComponent
  {
    /// <summary>
    /// The input map for the controller. 
    /// </summary>
    private InputMap _inputMap;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:MidnightBlue.UtilityController"/> component
    /// with default input assignment
    /// </summary>
    public UtilityController()
    {
      _inputMap = new InputMap();
      _inputMap.Assign<ConsoleCommand>(Keys.OemTilde, CommandType.Trigger);
    }

    /// <summary>
    /// Gets the input map.
    /// </summary>
    /// <value>The input map.</value>
    public InputMap InputMap
    {
      get { return _inputMap; }
    }

  }
}

