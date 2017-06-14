//
// 	CollisionCell.cs
// 	MB2D Engine
//
// 	--------------------------------------------------------------
//
// 	Created by Jacob Milligan on 6/10/2016.
// 	Copyright (c) Jacob Milligan All rights reserved
//
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MB2D.EntityComponent;
using MonoGame.Extended.Shapes;

namespace MB2D.Collision
{
  /// <summary>
  /// A cell used in a collision map to hold a linked list of
  /// all its contained entities
  /// </summary>
  public class CollisionCell
  {
    /// <summary>
    /// The entities inside this cell
    /// </summary>
    private LinkedList<Entity> _list;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:MB2D.Collision.CollisionCell"/> class.
    /// </summary>
    public CollisionCell()
    {
      _list = new LinkedList<Entity>();
    }

    /// <summary>
    /// Adds an entity to the cell
    /// </summary>
    /// <param name="entity">Entity to add.</param>
    public void Add(Entity entity)
    {
      _list.AddFirst(entity);
    }

    /// <summary>
    /// Removes a specific entity from the cell
    /// </summary>
    /// <param name="entity">Entity to remove.</param>
    public void Remove(Entity entity)
    {
      _list.Remove(entity);
    }

    /// <summary>
    /// Checks if the entity is inside the cell already
    /// </summary>
    /// <param name="entity">Entity.</param>
    public bool Contains(Entity entity)
    {
      return _list.Contains(entity);
    }

    /// <summary>
    /// Clear the cell of all entities.
    /// </summary>
    public void Clear()
    {
      _list.Clear();
    }

    /// <summary>
    /// Gets the list of this cells entities
    /// </summary>
    /// <value>The entities.</value>
    public LinkedList<Entity> Items
    {
      get { return _list; }
    }
  }
}
