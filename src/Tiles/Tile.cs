//
// 	Tile.cs
// 	MB2D Engine
//
// 	--------------------------------------------------------------
//
// 	Created by Jacob Milligan on 13/10/2016.
// 	Copyright (c) Jacob Milligan All rights reserved
//
using System;
using Microsoft.Xna.Framework;

namespace MB2D
{
  /// <summary>
  /// Flags a tile as passable or impassable, used in collision checking.
  /// </summary>
  public enum TileFlag
  {
    /// <summary>
    /// Flags a tile as walkable
    /// </summary>
    Passable,
    /// <summary>
    /// Flags a tile as collidable and unable to be walked on.
    /// </summary>
    Impassable
  }

  /// <summary>
  /// Represents a single tile in a tile map.
  /// </summary>
  public class Tile
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:MidnightBlue.Tile"/> class
    /// </summary>
    /// <param name="textureID">ID of the texture region in the tile map to use for this tile, i.e. Grass or water.</param>
    /// <param name="color">Color tint to apply to the tile.</param>
    public Tile(int textureID, Color color)
    {
      ID = textureID;
      TintColor = color;
      Flag = TileFlag.Passable;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:MidnightBlue.Tile"/> class.
    /// </summary>
    public Tile() : this(1, Color.Transparent) { }

    /// <summary>
    /// Gets or sets the tile map region ID to use for this tile.
    /// </summary>
    /// <value>The tile map region ID.</value>
    public int ID { get; protected set; }

    /// <summary>
    /// Gets or sets the color of the tint.
    /// </summary>
    /// <value>The color of the tint.</value>
    public Color TintColor { get; protected set; }

    /// <summary>
    /// Gets or sets the tile flag for collision detection.
    /// </summary>
    /// <value>The flag.</value>
    public TileFlag Flag { get; set; }
  }
}
