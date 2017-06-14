//
// 	CollisionRenderSystem.cs
// 	MB2D Engine
//
// 	--------------------------------------------------------------
//
// 	Created by Jacob Milligan on 5/10/2016.
// 	Copyright (c) Jacob Milligan All rights reserved
//
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MB2D.EntityComponent;
using MonoGame.Extended;
using MonoGame.Extended.Shapes;

namespace MB2D.Testing
{
  public class CollisionRenderSystem : EntitySystem
  {
    private SpriteBatch _spriteBatch;
    public CollisionRenderSystem(SpriteBatch spriteBatch)
      : base(typeof(CollisionComponent), typeof(SpriteTransform))
    {
      _spriteBatch = spriteBatch;
    }

    protected override void Process(Entity entity)
    {
      var collision = entity.GetComponent<CollisionComponent>();
      var sprite = entity.GetComponent<SpriteTransform>();

      if ( sprite != null && MBGame.Camera.BoundingRectangle.Intersects(sprite.Target.BoundingRectangle) ) {
        _spriteBatch.DrawRectangle(sprite.Target.BoundingRectangle, Color.Yellow);
        if ( collision != null ) {
          foreach ( var b in collision.Boxes ) {
            _spriteBatch.DrawRectangle(b, Color.Red);
            if ( collision.Event ) {
              _spriteBatch.DrawRectangle(b, Color.Green);
            }
          }
        }
      }

    }
  }
}
