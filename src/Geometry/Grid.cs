//
// 	Grid.cs
// 	MB2D Engine
//
// 	--------------------------------------------------------------
//
// 	Created by Jacob Milligan on 27/09/2016.
// 	Copyright (c) Jacob Milligan All rights reserved
//

using System;
using Microsoft.Xna.Framework;

namespace MB2D.Geometry
{
  /// <summary>
  /// Represents a grid structure. Can be drawn via a SpriteBatch
  /// </summary>
  public class Grid
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:MB2D.Geometry.Grid"/> class.
    /// </summary>
    /// <param name="rows">Number of rows</param>
    /// <param name="cols">Number of columns</param>
    /// <param name="rowSize">Size of each row</param>
    /// <param name="colSize">Size of each column</param>
    public Grid(int rows, int cols, int rowSize, int colSize)
    {
      RowCount = rows;
      ColCount = cols;
      RowSize = rowSize;
      ColSize = colSize;
    }

    /// <summary>
    /// Gets or sets the number of rows.
    /// </summary>
    /// <value>The row count.</value>
    public int RowCount { get; set; }
    /// <summary>
    /// Gets or sets the number of columns.
    /// </summary>
    /// <value>The colomn count.</value>
    public int ColCount { get; set; }
    /// <summary>
    /// Gets or sets the size of each row.
    /// </summary>
    /// <value>The size of each row.</value>
    public int RowSize { get; set; }
    /// <summary>
    /// Gets or sets the size of each column.
    /// </summary>
    /// <value>The size of each column.</value>
    public int ColSize { get; set; }

    /// <summary>
    /// Gets the size of each cell.
    /// </summary>
    /// <value>The size of each cell.</value>
    public Vector2 CellSize
    {
      get { return new Vector2(ColSize, RowSize); }
    }
  }
}
