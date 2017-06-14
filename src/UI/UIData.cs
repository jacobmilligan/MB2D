//
// 	UIGrid.cs
// 	MB2D Engine
//
// 	--------------------------------------------------------------
//
// 	Created by Jacob Milligan on 27/09/2016.
// 	Copyright (c) Jacob Milligan All rights reserved
//

using Microsoft.Xna.Framework;
using MB2D.Geometry;

namespace MB2D.UI
{
  /// <summary>
  /// Holds content in a grid structure for a UIContext or Layout
  /// </summary>
  public class UIContent
  {
    /// <summary>
    /// All elements stored in this content
    /// </summary>
    private UIElement[,] _grid;
    /// <summary>
    /// Height of each cell
    /// </summary>
    private int _rowSpan;
    /// <summary>
    /// Width of each cell
    /// </summary>
    private int _colSpan;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:MB2D.UI.UIContent"/> class.
    /// 
    /// </summary>
    /// <param name="rows">Rows.</param>
    /// <param name="cols">Cols.</param>
    /// <param name="parent">Parent.</param>
    public UIContent(int rows, int cols, Rectangle parent)
    {
      _grid = new UIElement[rows, cols];
      _rowSpan = parent.Height / rows;
      _colSpan = parent.Width / cols;
      Rect = parent;
    }

    /// <summary>
    /// Gets the elements of the content.
    /// </summary>
    /// <value>The UI elements.</value>
    public UIElement[,] Elements
    {
      get { return _grid; }
    }

    /// <summary>
    /// Gets a grid geometry representation of the content
    /// </summary>
    /// <value>The grid.</value>
    public Grid Grid
    {
      get
      {
        return new Grid(_grid.GetLength(0) + 1, _grid.GetLength(1) + 1, _rowSpan, _colSpan);
      }
    }

    /// <summary>
    /// Gets or sets the rectangle encompassing the content.
    /// </summary>
    /// <value>The rectangle.</value>
    public Rectangle Rect { get; set; }
  }
}
