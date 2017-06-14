//
// 	SpriteBatchExtensions.cs
// 	MB2D Engine
//
// 	--------------------------------------------------------------
//
// 	Created by Jacob Milligan on 28/09/2016.
// 	Copyright (c) Jacob Milligan All rights reserved
//

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MB2D.Geometry;
using MonoGame.Extended;
using MonoGame.Extended.Shapes;

namespace MB2D
{
  /// <summary>
  /// Extends SpriteBatch with MidnightBlue data structures.
  /// </summary>
  public static class SpriteBatchExtensions
  {
    /// <summary>
    /// Draws a grid to the screen
    /// </summary>
    /// <param name="spriteBatch">Sprite batch to draw with.</param>
    /// <param name="grid">Grid to draw.</param>
    /// <param name="position">Position to draw the grid at.</param>
    /// <param name="color">Color to draw the grid lines with.</param>
    public static void DrawGrid(this SpriteBatch spriteBatch, Grid grid, Point position, Color color)
    {
      var rowCount = grid.RowCount;
      var colCount = grid.ColCount;
      for ( int row = 0; row < rowCount; row++ ) {
        for ( int col = 0; col < colCount; col++ ) {
          // Draw a rectangle for every cell
          var rect = new RectangleF(
            position.X + (col * grid.ColSize),
            position.Y + (row * grid.RowSize),
            grid.ColSize,
            grid.RowSize
          );
          if ( MBGame.Graphics.Viewport.Bounds.Intersects((Rectangle)rect) ) {
            spriteBatch.DrawRectangle(rect, color, 1);
          }
        }
      }
    }
  }
}
