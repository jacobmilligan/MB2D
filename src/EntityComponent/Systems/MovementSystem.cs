//
// 	MovementSystem.cs
// 	MB2D Engine
//
// 	--------------------------------------------------------------
//
// 	Created by Jacob Milligan on 4/10/2016.
// 	Copyright (c) Jacob Milligan All rights reserved
//
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Shapes;

namespace MB2D.EntityComponent
{
  /// <summary>
  /// Processes the change in position, rotation, and sprite transform for an entity
  /// </summary>
  public class MovementSystem : EntitySystem
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:MB2D.EntityComponent.MovementSystem"/> class.
    /// </summary>
    public MovementSystem() : base(typeof(Movement), typeof(SpriteTransform)) { }

    /// <summary>
    /// Processes the movement for the specific entity
    /// </summary>
    /// <param name="entity">Entity to operate on.</param>
    protected override void Process(Entity entity)
    {
      if ( entity.HasComponent<Movement>() && entity.HasComponent<SpriteTransform>() ) {

        var movement = entity.GetComponent<Movement>();
        var sprite = entity.GetComponent<SpriteTransform>();
        // Update position
        movement.LastPosition = movement.Position;
        sprite.Target.Position = movement.Position;

        // Update bounds and rotation
        sprite.Rotation = movement.Angle;
        sprite.Bounds = sprite.Target.BoundingRectangle;

        // Update heading
        movement.Heading = sprite.Direction;
      }
    }

  }
}
