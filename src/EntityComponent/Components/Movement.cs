//
// 	Movement.cs
// 	MB2D Engine
//
// 	--------------------------------------------------------------
//
// 	Created by Jacob Milligan on 4/10/2016.
// 	Copyright (c) Jacob Milligan All rights reserved
//
using System;
using Microsoft.Xna.Framework;

namespace MB2D.EntityComponent
{
  /// <summary>
  /// Defines position, rotation and speed related data for moving an entity.
  /// </summary>
  public class Movement : IComponent
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:MB2D.EntityComponent.Movement"/> class.
    /// </summary>
    /// <param name="speed">Initial speed value.</param>
    /// <param name="rotationSpeed">Initial rotation speed value.</param>
    public Movement(float speed = 0.0f, float rotationSpeed = 0.0f)
    {
      Speed = speed;
      RotationSpeed = rotationSpeed;
    }

    /// <summary>
    /// Gets or sets the world position.
    /// </summary>
    /// <value>The position.</value>
    public Vector2 Position { get; set; }

    /// <summary>
    /// Gets or sets the rotation speed.
    /// </summary>
    /// <value>The rotation speed.</value>
    public float RotationSpeed { get; set; }

    /// <summary>
    /// Gets or sets the movement speed.
    /// </summary>
    /// <value>The speed.</value>
    public float Speed { get; set; }

    /// <summary>
    /// Gets or sets the current heading.
    /// </summary>
    /// <value>The heading.</value>
    public Vector2 Heading { get; set; }

    /// <summary>
    /// Gets or sets the angle.
    /// </summary>
    /// <value>The angle in radians.</value>
    public float Angle { get; set; }

    /// <summary>
    /// Gets or sets the last known position.
    /// </summary>
    /// <value>The last position.</value>
    public Vector2 LastPosition { get; set; }
  }
}
