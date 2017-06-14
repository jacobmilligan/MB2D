//
// 	PhysicsSystem.cs
// 	MB2D Engine
//
// 	--------------------------------------------------------------
//
// 	Created by Jacob Milligan on 4/10/2016.
// 	Copyright (c) Jacob Milligan All rights reserved
//
using System;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Shapes;

namespace MB2D.EntityComponent
{
  /// <summary>
  /// Defines a new environment to feed into the physics 
  /// system to alter the impact it has on an entity
  /// </summary>
  public class PhysicsEnvironment
  {
    /// <summary>
    /// Gets or sets the inertia of the environment.
    /// </summary>
    /// <value>The inertia.</value>
    public float Inertia { get; set; }
    /// <summary>
    /// Gets or sets the rotation inertia of the environment.
    /// </summary>
    /// <value>The rotation inertia.</value>
    public float RotationInertia { get; set; }
  }

  /// <summary>
  /// Processes physics changes for a given entity
  /// </summary>
  public class PhysicsSystem : EntitySystem
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:MB2D.EntityComponent.PhysicsSystem"/> class.
    /// </summary>
    public PhysicsSystem()
      : base(typeof(PhysicsComponent), typeof(Movement))
    {
      // Space
      Environment = new PhysicsEnvironment {
        Inertia = 0.999f,
        RotationInertia = 0.98f
      };
    }

    /// <summary>
    /// Updates the entities movement and velocity values based on the current
    /// physics environment
    /// </summary>
    /// <param name="entity">Entity to process.</param>
    protected override void Process(Entity entity)
    {
      var physics = entity.GetComponent<PhysicsComponent>();
      var movement = entity.GetComponent<Movement>();

      // Update rotation
      physics.RotationAcceleration *= Environment.RotationInertia;
      movement.Angle += physics.RotationAcceleration;

      // Newtons law except without mass - just some arbitrary 'power' value
      // applied by adding thrust to the ship
      var force = movement.Heading * physics.Power;
      physics.Acceleration = force;
      physics.Velocity += physics.Acceleration * MBGame.DeltaTime;
      movement.Position += physics.Velocity * MBGame.DeltaTime;

      physics.Velocity *= Environment.Inertia;

      // Reset every frame as power should be only on or off
      // not decaying
      physics.Power = 0;
    }

    /// <summary>
    /// Gets or sets the current physics environment.
    /// </summary>
    /// <value>The environment.</value>
    public PhysicsEnvironment Environment { get; set; }
  }
}
