//
// InputSystem.cs
// MB2D Engine
//
// ---------------------------------------------------
//
// Create by Jacob Milligan on 12/09/2016.
// Copyright (c) Jacob Milligan 2016. All rights reserved.
//
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using MB2D.IO;

namespace MB2D.EntityComponent
{
  /// <summary>
  /// Processes input for PlayerController and UtilityController entities.
  /// Can operate on an entity with either or both components
  /// </summary>
  public class InputSystem : EntitySystem
  {
    public InputSystem() : base(
      typeof(PlayerController), typeof(UtilityController)
    )
    {
      _associationType = EntityAssociation.Loose;
    }

    /// <summary>
    /// Processes the controllers inputs
    /// </summary>
    /// <param name="entity">Entity to process.</param>
    protected override void Process(Entity entity)
    {
      var keys = Keyboard.GetState().GetPressedKeys();

      var controller = entity.GetComponent<PlayerController>();
      var utilityController = entity.GetComponent<UtilityController>();

      // Handle input commands for both controllers for all pressed keys
      for ( int k = 0; k < keys.Length; k++ ) {
        // Player controller
        if ( controller != null ) {
          HandleInput(controller.InputMap[keys[k]], entity);
        }

        // Utility controller
        if ( utilityController != null ) {
          HandleInput(utilityController.InputMap[keys[k]], entity);
        }
      }

      // Update the camera position to look at the player
      var playerMovement = entity.GetComponent<Movement>();
      if ( playerMovement != null ) {
        MBGame.Camera.LookAt(
          playerMovement.Position
        );
      }
    }

    /// <summary>
    /// Handles calling a list of commands on a specific entity
    /// </summary>
    /// <param name="commands">Commands to process.</param>
    /// <param name="entity">Entity to operate on.</param>
    private void HandleInput(List<Command> commands, Entity entity)
    {
      if ( commands != null ) {
        for ( int c = 0; c < commands.Count; c++ ) {
          var cmd = commands[c];
          if ( cmd != null ) {
            cmd.Execute(entity);
          }
        }
      }
    }

  }
}

