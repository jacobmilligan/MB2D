//
// MoveCommands.cs
// MB2D Engine
//
// ---------------------------------------------------
//
// Create by Jacob Milligan on 13/09/2016.
// Copyright (c) Jacob Milligan 2016. All rights reserved.
//

using MB2D.Testing;
using MB2D.EntityComponent;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace MB2D.IO
{
  /// <summary>
  /// Moves a player controller up 
  /// </summary>
  public class MoveUp : Command
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:MB2D.IO.MoveUp"/> class.
    /// </summary>
    /// <param name="key">Key to assign to.</param>
    /// <param name="type">Trigger type.</param>
    public MoveUp(Keys key, CommandType type) : base(key, type) { }

    /// <summary>
    /// Move an entity up
    /// </summary>
    /// <param name="e">Entity to move.</param>
    protected override void OnKeyPress(Entity e = null)
    {
      var physics = e.GetComponent<PhysicsComponent>();
      var movement = e.GetComponent<Movement>();
      movement.Angle = MathHelper.ToRadians(270); // radians
      physics.Velocity += new Vector2(0, -1 * movement.Speed) * MBGame.DeltaTime;
      physics.Power = movement.Speed;
    }
  }

  /// <summary>
  /// Moves an entity right
  /// </summary>
  public class MoveRight : Command
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:MB2D.IO.MoveRight"/> class.
    /// </summary>
    /// <param name="key">Key to assign to.</param>
    /// <param name="type">Trigger type.</param>
    public MoveRight(Keys key, CommandType type) : base(key, type) { }

    /// <summary>
    /// Move an entity right
    /// </summary>
    /// <param name="e">Entity to move.</param>
    protected override void OnKeyPress(Entity e = null)
    {
      var physics = e.GetComponent<PhysicsComponent>();
      var movement = e.GetComponent<Movement>();
      movement.Angle = MathHelper.ToRadians(0); ; // radians
      physics.Velocity += new Vector2(1 * movement.Speed, 0) * MBGame.DeltaTime;
      physics.Power = movement.Speed;
    }
  }

  /// <summary>
  /// Moves an entity down
  /// </summary>
  public class MoveDown : Command
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:MB2D.IO.MoveDown"/> class.
    /// </summary>
    /// <param name="key">Key to assign to.</param>
    /// <param name="type">Trigger type.</param>
    public MoveDown(Keys key, CommandType type) : base(key, type) { }

    /// <summary>
    /// Move an entity down
    /// </summary>
    /// <param name="e">Entity to move.</param>
    protected override void OnKeyPress(Entity e = null)
    {
      var physics = e.GetComponent<PhysicsComponent>();
      var movement = e.GetComponent<Movement>();
      movement.Angle = MathHelper.ToRadians(90); ; // radians
      physics.Velocity += new Vector2(0, 1 * movement.Speed) * MBGame.DeltaTime;
      physics.Power = movement.Speed;
    }
  }

  /// <summary>
  /// Moves an entity left.
  /// </summary>
  public class MoveLeft : Command
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:MB2D.IO.MoveLeft"/> class.
    /// </summary>
    /// <param name="key">Key to assign to.</param>
    /// <param name="type">Trigger type.</param>
    public MoveLeft(Keys key, CommandType type) : base(key, type) { }

    /// <summary>
    /// Move an entity up
    /// </summary>
    /// <param name="e">Entity to move.</param>
    protected override void OnKeyPress(Entity e = null)
    {
      var physics = e.GetComponent<PhysicsComponent>();
      var movement = e.GetComponent<Movement>();
      movement.Angle = MathHelper.ToRadians(180);
      physics.Velocity += new Vector2(-1 * movement.Speed, 0) * MBGame.DeltaTime;
      physics.Power = movement.Speed;
    }
  }

  /// <summary>
  /// Moves an entity forward. Only runs on entities with a physics component
  /// </summary>
  public class MoveForward : Command
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:MB2D.IO.MoveForward"/> class.
    /// </summary>
    /// <param name="key">Key to assign to.</param>
    /// <param name="type">Trigger type.</param>
    public MoveForward(Keys key, CommandType type) : base(key, type) { }

    /// <summary>
    /// Move an entity forward based on their velocity
    /// </summary>
    /// <param name="e">Entity to move.</param>
    protected override void OnKeyPress(Entity e = null)
    {
      var physics = e.GetComponent<PhysicsComponent>();
      var movement = e.GetComponent<Movement>();
      physics.Velocity += (movement.Heading * movement.Speed) * MBGame.DeltaTime;
      physics.Power = movement.Speed;
    }
  }

  /// <summary>
  /// Moves an entity backward. Only runs on entities with a physics component
  /// </summary>
  public class MoveBackward : Command
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:MB2D.IO.MoveBackward"/> class.
    /// </summary>
    /// <param name="key">Key to assign to.</param>
    /// <param name="type">Trigger type.</param>
    public MoveBackward(Keys key, CommandType type) : base(key, type) { }

    /// <summary>
    /// Move an entity forward based on their velocity
    /// </summary>
    /// <param name="e">Entity to move.</param>
    protected override void OnKeyPress(Entity e = null)
    {
      var movement = e.GetComponent<Movement>();
      var physics = e.GetComponent<PhysicsComponent>();
      physics.Velocity -= (movement.Heading * movement.Speed) * MBGame.DeltaTime;
      physics.Power = -movement.Speed;
    }
  }

  /// <summary>
  /// Rotates an entity right
  /// </summary>
  public class RotateRight : Command
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:MB2D.IO.RotateRight"/> class.
    /// </summary>
    /// <param name="key">Key to assign to.</param>
    /// <param name="type">Trigger type.</param>
    public RotateRight(Keys key, CommandType type) : base(key, type) { }

    /// <summary>
    /// Rotates an entity right
    /// </summary>
    /// <param name="e">Entity to rotate.</param>
    protected override void OnKeyPress(Entity e = null)
    {
      var movement = e.GetComponent<Movement>();
      var physics = e.GetComponent<PhysicsComponent>();
      if ( movement != null ) {
        if ( physics != null ) {
          physics.RotationAcceleration += movement.RotationSpeed * MBGame.DeltaTime;
        } else {
          movement.Angle += movement.RotationSpeed * MBGame.DeltaTime;
        }
      }
    }
  }

  /// <summary>
  /// Rotates an entity left
  /// </summary>
  public class RotateLeft : Command
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:MB2D.IO.RotateLeft"/> class.
    /// </summary>
    /// <param name="key">Key to assign to.</param>
    /// <param name="type">Trigger type.</param>
    public RotateLeft(Keys key, CommandType type) : base(key, type) { }

    /// <summary>
    /// Rotates an entity left
    /// </summary>
    /// <param name="e">Entity to rotate.</param>
    protected override void OnKeyPress(Entity e = null)
    {
      var movement = e.GetComponent<Movement>();
      var physics = e.GetComponent<PhysicsComponent>();
      if ( movement != null ) {
        if ( physics != null ) {
          physics.RotationAcceleration -= movement.RotationSpeed * MBGame.DeltaTime;
        } else {
          movement.Angle -= movement.RotationSpeed * MBGame.DeltaTime;
        }
      }
    }
  }
}

