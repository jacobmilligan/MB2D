//
// 	Layout.cs
// 	MB2D Engine
//
// 	--------------------------------------------------------------
//
// 	Created by Jacob Milligan on 27/09/2016.
// 	Copyright (c) Jacob Milligan All rights reserved
//

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MB2D.UI
{
  /// <summary>
  /// A container for UIElements used within a UIView. Used to
  /// divide the View into smaller segments and to move around
  /// a group of elements easily
  /// </summary>
  public class Layout : UIElement
  {
    private UIView _parent;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:MB2D.UI.Layout"/> class.
    /// Divides the layouts rows and columns into an even cell size based of its parents size.
    /// </summary>
    /// <param name="rows">Number of rows the layout has.</param>
    /// <param name="cols">Number of columns the layout has.</param>
    public Layout(UIView parentView, int rows, int cols) : base(rows, cols)
    {
      _parent = parentView;
    }

    /// <summary>
    /// Update all elements contained within the layout.
    /// </summary>
    public override void Update()
    {
      var rowLen = Content.Elements.GetLength(0);
      var colLen = Content.Elements.GetLength(1);

      for ( int row = 0; row < rowLen; row++ ) {
        for ( int col = 0; col < colLen; col++ ) {
          var currGrid = Content.Elements[row, col];
          if ( currGrid != null ) {
            currGrid.Update();
          }
        }
      }
    }

    /// <summary>
    /// Draws all elements contained within the layout and the layouts border to the window.
    /// </summary>
    /// <param name="spriteBatch">Sprite batch to draw to.</param>
    public override void Draw(SpriteBatch spriteBatch)
    {
      base.Draw(spriteBatch);

      if ( BaseTexture != null ) {
        // Draw the current states texture
        spriteBatch.Draw(
          BaseTexture,
          destinationRectangle: BoundingBox
        );
      }

      if ( (bool)MBGame.Console.Vars["drawBorders"] || BorderDisplayed ) {
        DrawBorder(spriteBatch);
      }
      // DEBUG: Draws the layouts grid to the window
      if ( (bool)MBGame.Console.Vars["drawGrids"] ) {
        spriteBatch.DrawGrid(Content.Grid, new Point(Content.Rect.X, Content.Rect.Y), Color.Red);
      }

      var rowLen = Content.Elements.GetLength(0);
      var colLen = Content.Elements.GetLength(1);

      // Draw all elements
      for ( int row = 0; row < rowLen; row++ ) {
        for ( int col = 0; col < colLen; col++ ) {
          var currGrid = Content.Elements[row, col];
          if ( currGrid != null ) {
            currGrid.Draw(spriteBatch);
          }
        }
      }
    }

    /// <summary>
    /// Adds a new UIElement to the layout, setting its size relative to this layouts cell size
    /// </summary>
    /// <param name="element">Element to add.</param>
    /// <param name="atRow">Row position to add the element at.</param>
    /// <param name="atCol">Column position to add the element at.</param>
    /// <param name="rowSpan">Number of rows the element should span.</param>
    /// <param name="colSpan">Number of columns the element should span.</param>
    public void Add(UIElement element, int atRow, int atCol, int rowSpan, int colSpan)
    {
      atRow--;
      atCol--;

      if ( atRow < 0 )
        atRow = 0;
      if ( atCol < 0 )
        atCol = 0;

      element.SetRelativeSize(Content, atRow, atCol, rowSpan, colSpan);
      Content.Elements[atRow, atCol] = element;
      _parent.AddToLookup(element);
    }

    public Texture2D BaseTexture { get; set; }
  }
}
