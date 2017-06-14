//
// 	ListView.cs
// 	MB2D Engine
//
// 	--------------------------------------------------------------
//
// 	Created by Jacob Milligan on 9/10/2016.
// 	Copyright (c) Jacob Milligan All rights reserved
//
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MB2D.IO;
using MonoGame.Extended;
using MonoGame.Extended.Shapes;

namespace MB2D.UI
{
  /// <summary>
  /// A scrollable list box. Items can be added and interactied with.
  /// </summary>
  public class ListControl : UIControlElement
  {
    /// <summary>
    /// Font to render in the list box
    /// </summary>
    private SpriteFont _font;

    /// <summary>
    /// The index in the List of the currently selected item
    /// </summary>
    private int _selectedItem;

    /// <summary>
    /// The up arrow box
    /// </summary>
    private Rectangle _upArrow,
    /// <summary>
    /// The down arrow box
    /// </summary>
    _downArrow;

    /// <summary>
    /// The rectangle encompassing the entire list. Culled by the outer, visible bounding box
    /// of the list.
    /// </summary>
    private Rectangle _listRect;

    /// <summary>
    /// All elements currently in the list.
    /// </summary>
    private List<string> _elements;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:MB2D.UI.ListControl"/> class
    /// using the specified font to draw elements.
    /// </summary>
    /// <param name="font">Font to use.</param>
    public ListControl(SpriteFont font)
      : this(font, null, null, null)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:MB2D.UI.ListControl"/> class
    /// with a font to use when drawing elements alongside state textures.
    /// </summary>
    /// <param name="font">Font to use.</param>
    /// <param name="normal">Normal state texture.</param>
    /// <param name="selected">Selected stae texture.</param>
    /// <param name="pressed">Pressed state texture.</param>
    public ListControl(SpriteFont font, Texture2D normal, Texture2D selected, Texture2D pressed)
      : base(normal, selected, pressed)
    {
      _elements = new List<string>();

      // Setup default styles
      SeperaterColor = Color.LightGray;
      ControlSize = 10;
      ItemSpan = 1;
      _selectedItem = -1;
      _font = font;

      _listRect = new Rectangle(
        BoundingBox.Location.X,
        BoundingBox.Location.Y,
        BoundingBox.Size.X - ControlSize,
        BoundingBox.Size.Y
      );
    }

    /// <summary>
    /// Updates the element to handle scrolling and clicking. Must be called
    /// once per frame.
    /// </summary>
    public override void Update()
    {
      // If none of the visuals have been preset
      // setup the default arrow and list box graphics
      if ( _upArrow == Rectangle.Empty || _downArrow == Rectangle.Empty ) {
        _upArrow = new Rectangle(
          BoundingBox.Right - ControlSize - BorderWidth,
          BoundingBox.Top + BorderWidth,
          ControlSize,
          ControlSize
        );

        _downArrow = new Rectangle(
          BoundingBox.Right - ControlSize - BorderWidth,
          BoundingBox.Bottom - ControlSize - BorderWidth,
          ControlSize,
          ControlSize
        );

        _listRect = new Rectangle(
          BoundingBox.Location.X,
          BoundingBox.Location.Y,
          BoundingBox.Size.X - ControlSize,
          BoundingBox.Size.Y
        );
      }

      HandleScroll();
      HandleClick();
    }

    /// <summary>
    /// Draws the list control to the window
    /// </summary>
    /// <param name="spriteBatch">Sprite batch to draw to.</param>
    public override void Draw(SpriteBatch spriteBatch)
    {
      base.Draw(spriteBatch);

      spriteBatch.End();

      // Creates a rasterize state for clipping elements outside the outer
      // box of the control
      var rasterizeState = new RasterizerState() { ScissorTestEnable = true };

      spriteBatch.Begin(rasterizerState: rasterizeState);
      spriteBatch.GraphicsDevice.ScissorRectangle = BoundingBox; // the outer box of the list control

      // Draw arrows
      spriteBatch.DrawRectangle(_upArrow, SeperaterColor, 2);
      spriteBatch.DrawRectangle(_downArrow, SeperaterColor, 2);

      // Draw the selected element with a special color
      if ( _selectedItem >= 0 ) {
        spriteBatch.FillRectangle(
          BoundingBox.X,
          _listRect.Top + (_selectedItem * ItemSpan),
          BoundingBox.Width,
          ItemSpan,
          BackgroundColor
        );
      }

      var nextEntry = 0;
      var itemY = 0;

      // Draws all the other list elements to the box - clips them if they aren't
      // in the visible area
      for ( int item = 0; item < Count && nextEntry < BoundingBox.Bottom; item++ ) {

        nextEntry = _listRect.Location.Y + (ItemSpan * itemY);
        var listEntry = new Vector2(BoundingBox.X, nextEntry); // clip!

        spriteBatch.DrawString(
          _font,
          Elements[item],
          listEntry,
          SeperaterColor
        );

        itemY++;
      }


      spriteBatch.End();
      spriteBatch.Begin();
      spriteBatch.GraphicsDevice.ScissorRectangle = MBGame.Graphics.Viewport.Bounds;
    }

    /// <summary>
    /// Handles click events on list control elements.
    /// </summary>
    private void HandleClick()
    {
      var mouse = Mouse.GetState();

      // Check all elements to see if they've been clicked
      for ( int element = 0; element < Count; element++ ) {
        // Create a temporary rectangle for checking click event
        var listItemRect = new Rectangle(
          BoundingBox.X,
          _listRect.Y + (ItemSpan * element),
          _listRect.Width,
          ItemSpan
        );

        if ( IOUtil.LeftMouseClicked() && listItemRect.Contains(mouse.Position) ) {
          _selectedItem = element;
        }

      }
    }

    /// <summary>
    /// Handles scrolling the list control up and down.
    /// </summary>
    private void HandleScroll()
    {
      var mouse = Mouse.GetState();
      var listPos = _listRect.Location;

      // Scroll up
      if ( _upArrow.Contains(mouse.Position) && mouse.LeftButton == ButtonState.Pressed ) {
        // Only if the list hasn't reached the bottom
        if ( listPos.Y <= BoundingBox.Top ) {
          _listRect.Location = new Point(listPos.X, listPos.Y + 1);
        }
      }
      // Scroll down
      if ( _downArrow.Contains(mouse.Position) && mouse.LeftButton == ButtonState.Pressed ) {
        // Only if the list hasn't reached the top
        if ( listPos.Y + _listRect.Height > BoundingBox.Bottom ) {
          _listRect.Location = new Point(listPos.X, listPos.Y - 1);
        }
      }

    }

    /// <summary>
    /// Gets or sets a list of all of the list control elements
    /// </summary>
    /// <value>The elements.</value>
    public List<string> Elements
    {
      get
      {
        return _elements;
      }
      set
      {
        _elements = value;
        _listRect.Height = Count * ItemSpan;
      }
    }

    /// <summary>
    /// Gets the count of list control elements.
    /// </summary>
    /// <value>The count.</value>
    public int Count
    {
      get { return _elements.Count; }
    }

    /// <summary>
    /// Gets or sets the color of the seperater between list elements.
    /// </summary>
    /// <value>The color of the seperater.</value>
    public Color SeperaterColor { get; set; }

    /// <summary>
    /// Gets or sets the size of the up and down arrow controls.
    /// </summary>
    /// <value>The size of the controls.</value>
    public int ControlSize { get; set; }

    /// <summary>
    /// Gets or sets the span of each item vertically in px.
    /// </summary>
    /// <value>The item span.</value>
    public int ItemSpan { get; set; }
  }
}
