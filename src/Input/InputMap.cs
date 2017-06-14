//
// InputMap.cs
// MB2D Engine
//
// ---------------------------------------------------
//
// Create by Jacob Milligan on 12/09/2016.
// Copyright (c) Jacob Milligan 2016. All rights reserved.
//

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Input;

namespace MB2D.IO
{
  /// <summary>
  /// Maps commands to keys and trigger types
  /// </summary>
  public class InputMap
  {
    /// <summary>
    /// All commands mapped to specific keys
    /// </summary>
    private Dictionary<Keys, List<Command>> _inputMap;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:MB2D.IO.InputMap"/> class.
    /// </summary>
    public InputMap()
    {
      _inputMap = new Dictionary<Keys, List<Command>>();
    }

    /// <summary>
    /// Assign a Command to the specified key and CommandType .
    /// </summary>
    /// <param name="key">Key to assign the command to</param>
    /// <param name="type">Type of trigger</param>
    /// <typeparam name="T">The command to assign</typeparam>
    public Command Assign<T>(Keys key, CommandType type, params object[] args) where T : Command
    {
      var newCommand = NewCommand<T>(key, type, args);

      if ( _inputMap.ContainsKey(key) ) {
        _inputMap[key].Add(newCommand);
      } else {
        _inputMap.Add(key, new List<Command>());
        _inputMap[key].Add(newCommand);
      }

      return newCommand;
    }

    /// <summary>
    /// Searches the map for a specific command
    /// </summary>
    /// <typeparam name="T">The 1st type parameter.</typeparam>
    public T Search<T>() where T : Command
    {
      var bucket = _inputMap.Select(
        (key) => key.Value.Find(
          command => command.GetType() == typeof(T)
        )
      );
      var result = bucket.FirstOrDefault(
        command => command != null && command.GetType() == typeof(T)
      );

      if ( result == null )
        return null;

      return (T)result;
    }

    /// <summary>
    /// Creates a new instance of a Command
    /// </summary>
    /// <returns>The command.</returns>
    /// <param name="key">Key to assign the command to</param>
    /// <param name="type">Type of trigger</param>
    /// <typeparam name="T">Type of Command to new</typeparam>
    private Command NewCommand<T>(Keys key, CommandType type, params object[] arguments) where T : Command
    {
      var args = new List<object> { key, type };
      foreach ( var a in arguments ) {
        args.Add(a);
      }

      if ( arguments.Length > 0 )
        return (T)Activator.CreateInstance(typeof(T), args.ToArray());

      return (T)Activator.CreateInstance(typeof(T), key, type);
    }

    /// <summary>
    /// Gets the <see cref="T:MB2D.IO.Commnad"/> mapped to the specified key.
    /// </summary>
    /// <param name="key">Key to query</param>
    public List<Command> this[Keys key]
    {
      get
      {
        if ( _inputMap.ContainsKey(key) ) {
          return _inputMap[key];
        }
        return null;
      }
    }

    /// <summary>
    /// Gets a key->Command dictionary
    /// </summary>
    /// <value>The collection of commands.</value>
    public Dictionary<Keys, List<Command>> Collection
    {
      get { return _inputMap; }
    }
  }
}

