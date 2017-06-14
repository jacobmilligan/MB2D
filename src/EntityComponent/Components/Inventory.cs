//
// 	Inventory.cs
// 	MB2D Engine
//
// 	--------------------------------------------------------------
//
// 	Created by Jacob Milligan on 6/10/2016.
// 	Copyright (c) Jacob Milligan All rights reserved
//
using System;
using System.Collections.Generic;

namespace MB2D.EntityComponent
{
  /// <summary>
  /// Defines a dictionary of Collectable types used for entities
  /// </summary>
  public class Inventory : IComponent
  {
    public Inventory()
    {
      Items = new Dictionary<Type, Collectable>();
    }

    /// <summary>
    /// The items currently in the inventory
    /// </summary>
    /// <value>The items.</value>
    public Dictionary<Type, Collectable> Items { get; set; }
  }
}
