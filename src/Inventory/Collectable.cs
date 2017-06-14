//
// 	ICollectable.cs
// 	MB2D Engine
//
// 	--------------------------------------------------------------
//
// 	Created by Jacob Milligan on 6/10/2016.
// 	Copyright (c) Jacob Milligan All rights reserved
//
using System;
using MB2D.EntityComponent;

namespace MB2D
{
  /// <summary>
  /// Defines an object that can be contained within components and systems
  /// that operate on collectible items, such as Inventory.
  /// </summary>
  public abstract class Collectable
  {
    /// <summary>
    /// The name of the item
    /// </summary>
    private string _name,
    /// <summary>
    /// The short name used for lookups and displaying in summaries
    /// </summary>
    _tag;

    /// <summary>
    /// The number of instances of this item available.
    /// </summary>
    private int _count;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:MB2D.Collectable"/> class.
    /// </summary>
    /// <param name="name">Name to give to the item.</param>
    /// <param name="tag">Short tag to give to the item.</param>
    /// <param name="initialCount">Initial count to add to the container.</param>
    public Collectable(string name, string tag, int initialCount)
    {
      _name = name;
      _tag = tag;
      _count = initialCount;
    }

    /// <summary>
    /// Consumes a number of instances of the item
    /// </summary>
    /// <param name="amount">Amount to consume.</param>
    public void Consume(int amount = 1)
    {
      if ( amount > 0 ) {
        _count -= amount;
      }
      if ( _count < 0 ) {
        _count = 0;
      }
    }

    /// <summary>
    /// Adds a number of instances of this item to the container
    /// </summary>
    /// <param name="amount">Amount to add.</param>
    public void Add(int amount = 1)
    {
      if ( amount > 0 ) {
        _count += amount;
      }
    }

    /// <summary>
    /// The action to enact when the item is consumed or used
    /// </summary>
    /// <param name="entity">Entity to operate on.</param>
    public abstract void Effect(Entity entity);

    /// <summary>
    /// Gets the name of the item.
    /// </summary>
    /// <value>The name.</value>
    public string Name
    {
      get { return _name; }
    }

    /// <summary>
    /// Gets the items tag.
    /// </summary>
    /// <value>The tag.</value>
    public string Tag
    {
      get { return _tag; }
    }

    /// <summary>
    /// Gets the count of available instances of the item.
    /// </summary>
    /// <value>The count.</value>
    public int Count
    {
      get { return _count; }
    }
  }
}
