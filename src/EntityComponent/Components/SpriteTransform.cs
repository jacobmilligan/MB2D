//
// 	Sprite.cs
// 	MB2D Engine
//
// 	--------------------------------------------------------------
//
// 	Created by Jacob Milligan on 4/10/2016.
// 	Copyright (c) Jacob Milligan All rights reserved
//
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Shapes;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;

namespace MB2D.EntityComponent
{
  /// <summary>
  /// Defines a sprite component with control over its size, rotation, and scale
  /// </summary>
  public class SpriteTransform : IComponent
  {
    /// <summary>
    /// Sets the default values of a new sprite.
    /// </summary>
    /// <param name="position">Default position.</param>
    /// <param name="scale">Default scale.</param>
    private void SetDefaults(Vector2 position, Vector2 scale)
    {
      Target.Position = position;
      Target.Scale = scale;
      Rotation = 0.0f;
      Z = 0;
      Bounds = Target.BoundingRectangle;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:MB2D.EntityComponent.SpriteTransform"/> class.
    /// </summary>
    /// <param name="texture">Texture to assign to the sprite.</param>
    /// <param name="position">Initial position of the sprite. Should be the entities position for best practice.</param>
    /// <param name="scale">Initial scale of the sprite.</param>
    public SpriteTransform(Texture2D texture, Vector2 position, Vector2 scale)
    {
      Target = new Sprite(texture);
      SetDefaults(position, scale);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:MB2D.EntityComponent.SpriteTransform"/> class.
    /// </summary>
    /// <param name="texture">Texture region in a texture atlas to assign to the sprite.</param>
    /// <param name="position">Initial position of the sprite. Should be the entities position for best practice.</param>
    /// <param name="scale">Initial scale of the sprite.</param>
    public SpriteTransform(TextureRegion2D texture, Vector2 position, Vector2 scale)
    {
      Target = new Sprite(texture);
      SetDefaults(position, scale);
    }

    /// <summary>
    /// Gets or sets the sprites target containing all of the applied data and logicd.
    /// </summary>
    /// <value>The sprite target.</value>
    public Sprite Target { get; set; }

    /// <summary>
    /// Gets the sprites direction.
    /// </summary>
    /// <value>The direction.</value>
    // Direction and Rotation implementation derived from Monogame.Extended SpaceGame Demo source code:
    // https://github.com/craftworkgames/MonoGame.Extended/blob/develop/Source/Demos/Demo.SpaceGame/Entities/Spaceship.cs
    public Vector2 Direction
    {
      get
      {
        return Vector2.UnitX.Rotate(Rotation);
      }
    }

    /// <summary>
    /// Gets or sets the rotation in radians.
    /// </summary>
    /// <value>The rotation in radians.</value>
    public float Rotation
    {
      get { return Target.Rotation - MathHelper.ToRadians(90); }
      set { Target.Rotation = value + MathHelper.ToRadians(90); }
    }

    /// <summary>
    /// Gets or sets the sprites bounding box.
    /// </summary>
    /// <value>The bounds.</value>
    public RectangleF Bounds { get; set; }

    /// <summary>
    /// Gets or sets the sprites z index. Used in depth systems.
    /// </summary>
    /// <value>The z index.</value>
    public float Z { get; set; }
  }
}
