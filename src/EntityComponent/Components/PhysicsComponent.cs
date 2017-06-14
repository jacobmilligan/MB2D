//
// 	PhysicsComponent.cs
// 	MB2D Engine
//
// 	--------------------------------------------------------------
//
// 	Created by Jacob Milligan on 8/10/2016.
// 	Copyright (c) Jacob Milligan All rights reserved
//
using System;
using Microsoft.Xna.Framework;

namespace MB2D.EntityComponent
{
  /// <summary>
  /// Physics component used to define acceleration and velocity.
  /// </summary>
  public class PhysicsComponent : IComponent
  {
    /// <summary>
    /// Gets or sets the current acceleration of the rotation.
    /// </summary>
    /// <value>The rotation acceleration.</value>
    public float RotationAcceleration { get; set; }

    /// <summary>
    /// Gets or sets the current acceleration.
    /// </summary>
    /// <value>The acceleration.</value>
    public Vector2 Acceleration { get; set; }

    /// <summary>
    /// Gets or sets the current positional velocity.
    /// </summary>
    /// <value>The velocity.</value>
    public Vector2 Velocity { get; set; }

    /// <summary>
    /// Gets or sets the current power applied.
    /// </summary>
    /// <value>The power applied.</value>
    public float Power { get; set; }
  }
}
