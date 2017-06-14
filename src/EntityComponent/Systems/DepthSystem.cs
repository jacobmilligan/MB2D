//
// 	DepthSystem.cs
// 	MB2D Engine
//
// 	--------------------------------------------------------------
//
// 	Created by Jacob Milligan on 5/10/2016.
// 	Copyright (c) Jacob Milligan All rights reserved
//
using System;
namespace MB2D.EntityComponent
{
  /// <summary>
  /// Changes an entities z-index based on the y coordinate of the top of their sprite
  /// </summary>
  public class DepthSystem : EntitySystem
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:MB2D.EntityComponent.DepthSystem"/> class.
    /// </summary>
    public DepthSystem() : base(typeof(Depth), typeof(SpriteTransform)) { }

    /// <summary>
    /// Changes the z index based on the y coordinate of the entities bounds top
    /// </summary>
    /// <param name="entity">Entity to process.</param>
    protected override void Process(Entity entity)
    {
      if ( entity.HasComponent<Depth>() && entity.HasComponent<SpriteTransform>() ) {
        var sprite = entity.GetComponent<SpriteTransform>();
        sprite.Z = sprite.Bounds.Top;
      }
    }
  }
}
