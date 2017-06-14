//
// 	Collision.cs
// 	MB2D Engine
//
// 	--------------------------------------------------------------
//
// 	Created by Jacob Milligan on 5/10/2016.
// 	Copyright (c) Jacob Milligan All rights reserved
//
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MB2D.Collision;
using MonoGame.Extended;
using MonoGame.Extended.Shapes;

namespace MB2D.EntityComponent
{
  /// <summary>
  /// Used for running collision detection on an Entity
  /// </summary>
  public class CollisionComponent : IComponent
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:MB2D.EntityComponent.CollisionComponent"/> class
    /// with an array of its associated AABB's
    /// </summary>
    /// <param name="boxes">The bounding boxes used for detecting collisions.</param>
    public CollisionComponent(params RectangleF[] boxes)
    {
      if ( boxes.Length > 0 ) {
        Boxes = new List<RectangleF>(boxes);
      } else {
        Boxes = new List<RectangleF>();
      }
    }
    /// <summary>
    /// Gets or sets the list of bounding boxes used for collision detection.
    /// </summary>
    /// <value>The boxes.</value>
    public List<RectangleF> Boxes { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this
    /// <see cref="T:MB2D.EntityComponent.CollisionComponent"/> has had a
    /// collision event this frame.
    /// </summary>
    /// <value><c>true</c> if an event ocurred; otherwise, <c>false</c>.</value>
    public bool Event { get; set; }

    /// <summary>
    /// Gets or sets the collider entity associated with the collision event.
    /// </summary>
    /// <value>The collider.</value>
    public Entity Collider { get; set; }
  }
}
