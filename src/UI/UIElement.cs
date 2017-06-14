//
// 	UIElement.cs
// 	MB2D Engine
//
// 	--------------------------------------------------------------
//
// 	Created by Jacob Milligan on 27/09/2016.
// 	Copyright (c) Jacob Milligan All rights reserved
//

using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MB2D.Geometry;
using MonoGame.Extended;
using MonoGame.Extended.Shapes;

namespace MB2D.UI
{
  /// <summary>
  /// Defines a UI object that can be contained within Views and Layouts, drawn, updated, and
  /// moved about
  /// </summary>
  public abstract class UIElement
  {
    /// <summary>
    /// The content within this element
    /// </summary>
    private UIContent _grid;
    /// <summary>
    /// The rectangle encompassing the elements border
    /// </summary>
    private Rectangle _borderRect;
    /// <summary>
    /// The base color of the border, overriden by individual border sides' colors.
    /// </summary>
    private Color _baseBorderColor;
    /// <summary>
    /// The borders individual sides
    /// </summary>
    private Line _borderTop, _borderRight, _borderBottom, _borderLeft;
    /// <summary>
    /// The number of columns and rows this element takes up in its parent
    /// </summary>
    private int _numRows, _numCols;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:MB2D.UI.UIElement"/> class.
    /// Sets default property values
    /// </summary>
    /// <param name="rows">
    /// Number of rows this element should span - used only be used for container elements
    /// </param>
    /// <param name="cols">
    /// Number of columns this element should span - used only be used for container elements.
    /// </param>
    protected UIElement(int rows, int cols)
    {
      BorderDisplayed = false;
      BorderColor = Color.Transparent;
      BorderTopColor = BorderRightColor = BorderBottomColor = BorderLeftColor = BorderColor;
      BorderWidth = 0;

      TextColor = Color.White;
      TextContent = string.Empty;

      Fill = false;

      _numRows = rows;
      _numCols = cols;

      _grid = new UIContent(1, 1, Rectangle.Empty);

      Tag = string.Empty;
    }

    /// <summary>
    /// Sets the size of the element relative to its parent
    /// </summary>
    /// <param name="parent">Parent content to align to</param>
    /// <param name="at">Position element should be set to</param>
    /// <param name="span">Number of columns/rows the element should span.</param>
    public void SetRelativeSize(UIContent parent, Point at, Point span)
    {
      var x = parent.Rect.X + (parent.Grid.ColSize * at.X);
      var y = parent.Rect.Y + (parent.Grid.RowSize * at.Y);
      var width = parent.Grid.ColSize * span.X;
      var height = parent.Grid.RowSize * span.Y;

      var rect = new Rectangle(x, y, width, height);

      if ( _grid != null ) {
        _grid = new UIContent(_numRows, _numCols, rect);
      }

      _borderRect = rect;
      _borderRect.Width += parent.Grid.ColSize;
      _borderRect.Height += parent.Grid.RowSize;
      ResetBorders();
    }

    private void ResetBorders()
    {
      _borderTop = new Line(
        new Vector2(_borderRect.X, _borderRect.Y),
        new Vector2(_borderRect.X + _borderRect.Width, _borderRect.Y)
      );
      _borderRight = new Line(
        new Vector2(_borderRect.X + _borderRect.Width, _borderRect.Y),
        new Vector2(_borderRect.X + _borderRect.Width, _borderRect.Y + _borderRect.Height)
      );
      _borderBottom = new Line(
        new Vector2(_borderRect.X + _borderRect.Width, _borderRect.Y + _borderRect.Height),
        new Vector2(_borderRect.X, _borderRect.Y + _borderRect.Height)
      );
      _borderLeft = new Line(
        new Vector2(_borderRect.X, _borderRect.Y + _borderRect.Height),
        new Vector2(_borderRect.X, _borderRect.Y)
      );
    }

    /// <summary>
    /// Sets the size of the element relative to its parent
    /// </summary>
    /// <param name="parent">Parent content to align to</param>
    /// <param name="atRow">Row position the element should align to.</param>
    /// <param name="atCol">Column position the element should align to.</param>
    /// <param name="rowSpan">Number of rows in the parent the element should span.</param>
    /// <param name="colSpan">Number of columns in the parent the element should span.</param>
    public void SetRelativeSize(UIContent parent, int atRow, int atCol, int rowSpan, int colSpan)
    {
      SetRelativeSize(
        parent, new Point(atCol, atRow), new Point(colSpan, rowSpan)
      );
    }

    /// <summary>
    /// Draws the element to the window. Overriden in derived classes
    /// </summary>
    /// <param name="spriteBatch">Sprite batch to draw to.</param>
    public virtual void Draw(SpriteBatch spriteBatch)
    {
      if ( BackgroundColor != Color.Transparent ) {
        spriteBatch.FillRectangle(_borderRect, BackgroundColor);
      }
    }

    /// <summary>
    /// Update the elements state and handles input. Overriden in derived classes
    /// </summary>
    public abstract void Update();

    /// <summary>
    /// Draws the elements border to the window. Skips sides that have color set to <see cref="Color.Transparent"/>
    /// </summary>
    /// <param name="spriteBatch">Sprite batch to draw the border to.</param>
    protected void DrawBorder(SpriteBatch spriteBatch)
    {
      var colors = new Color[] { BorderTopColor, BorderRightColor, BorderBottomColor, BorderLeftColor };
      if ( colors.All(clr => clr == BorderTopColor) ) {
        spriteBatch.DrawRectangle(_borderRect, BorderTopColor, BorderWidth);
      } else {
        if ( BorderTopColor != Color.Transparent ) {
          spriteBatch.DrawLine(_borderTop.Start, _borderTop.End, BorderTopColor, BorderWidth);
        }
        if ( BorderRightColor != Color.Transparent ) {
          spriteBatch.DrawLine(_borderRight.Start, _borderRight.End, BorderRightColor, BorderWidth);
        }
        if ( BorderBottomColor != Color.Transparent ) {
          spriteBatch.DrawLine(_borderBottom.Start, _borderBottom.End, BorderBottomColor, BorderWidth);
        }
        if ( BorderLeftColor != Color.Transparent ) {
          spriteBatch.DrawLine(_borderLeft.Start, _borderLeft.End, BorderLeftColor, BorderWidth);
        }
      }
    }

    /// <summary>
    /// Gets the UIContent of the element, only available in container elements
    /// </summary>
    /// <value>The content.</value>
    public UIContent Content
    {
      get { return _grid; }
    }

    /// <summary>
    /// Gets the column and row count of the element
    /// </summary>
    /// <value>The elements grid size.</value>
    public Vector2 Size
    {
      get { return new Vector2(_numCols, _numRows); }
    }

    /// <summary>
    /// Gets the bounding box of this element
    /// </summary>
    /// <value>The bounding box.</value>
    public Rectangle BoundingBox
    {
      get { return _borderRect; }
    }

    /// <summary>
    /// Gets or sets a value that indicates the element should be stretched or shrunk to fill its
    /// parents bounds exactly
    /// </summary>
    /// <value><c>true</c> if set to fill parent; otherwise, <c>false</c>.</value>
    public bool Fill { get; set; }

    /// <summary>
    /// Gets or sets the color of the elements background.
    /// </summary>
    /// <value>The color of the background.</value>
    public Color BackgroundColor { get; set; }

    /// <summary>
    /// Gets or sets the string rendered by the element.
    /// </summary>
    /// <value>The content of the text.</value>
    public string TextContent { get; set; }
    /// <summary>
    /// Gets or sets the current color of the text.
    /// </summary>
    /// <value>The text contents current color value.</value>
    public Color TextColor { get; set; }
    /// <summary>
    /// Gets or sets the font used in rendering the elements TextContent
    /// </summary>
    /// <value>The font.</value>
    public SpriteFont Font { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this 
    /// <see cref="T:MB2D.UI.UIElement"/> has itsborder displayed.
    /// </summary>
    /// <value><c>true</c> if border should be displayed; otherwise, <c>false</c>.</value>
    public bool BorderDisplayed { get; set; }
    /// <summary>
    /// Gets or sets the width of the border.
    /// </summary>
    /// <value>The width of the border.</value>
    public int BorderWidth { get; set; }
    /// <summary>
    /// Gets or sets the color of the border.
    /// </summary>
    /// <value>The color of the border. Resets all border sides' colors to this color.</value>
    public Color BorderColor
    {
      get
      {
        return _baseBorderColor;
      }
      set
      {
        BorderTopColor = BorderRightColor = BorderBottomColor = BorderLeftColor = _baseBorderColor = value;
      }
    }
    /// <summary>
    /// Gets or sets the color of the border top.
    /// </summary>
    /// <value>The color of the border top.</value>
    public Color BorderTopColor { get; set; }
    /// <summary>
    /// Gets or sets the color of the border right.
    /// </summary>
    /// <value>The color of the border right.</value>
    public Color BorderRightColor { get; set; }
    /// <summary>
    /// Gets or sets the color of the border bottom.
    /// </summary>
    /// <value>The color of the border bottom.</value>
    public Color BorderBottomColor { get; set; }
    /// <summary>
    /// Gets or sets the color of the border left.
    /// </summary>
    /// <value>The color of the border left.</value>
    public Color BorderLeftColor { get; set; }

    /// <summary>
    /// Gets or sets the tag used to quickly access this element and uniquely identify it.
    /// </summary>
    /// <value>The tag.</value>
    public string Tag { get; set; }
  }
}
