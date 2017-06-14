//
//  UIContext.cs
//  MB2D Engine
//
//  --------------------------------------------------------------
//
//  Created by Jacob Milligan on 27/09/2016.
//  Copyright (c) Jacob Milligan All rights reserved
//

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MB2D.UI
{
  /// <summary>
  /// A single context for all UI elements and layouts.
  /// </summary>
  public class UIView
  {
    /// <summary>
    /// The content and grid representation of the View
    /// </summary>
    private UIContent _grid;

    /// <summary>
    /// Allows fast lookup of all elements in all layouts in the view
    /// </summary>
    private Dictionary<string, UIElement> _elementLookup;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:MB2D.UI.UIView"/> class.
    /// Divides itself into the number of rows and columns evenly based on the size of the
    /// current viewport.
    /// </summary>
    /// <param name="rows">Number of rows in the view</param>
    /// <param name="cols">Number of columns in the view</param>
    public UIView(int rows, int cols)
    {
      _grid = new UIContent(rows, cols, MBGame.Graphics.Viewport.Bounds);
      _elementLookup = new Dictionary<string, UIElement>();
    }

    /// <summary>
    /// Updates and handles input for all elements in the View.
    /// </summary>
    public void Update()
    {
      var rowLen = Content.GetLength(0);
      var colLen = Content.GetLength(1);
      for ( int row = 0; row < rowLen; row++ ) {
        for ( int col = 0; col < colLen; col++ ) {
          var currGrid = _grid.Elements[row, col];
          if ( currGrid != null ) {
            currGrid.Update();
          }
        }
      }
    }

    /// <summary>
    /// Draws the View and its elements to the window
    /// </summary>
    /// <param name="uiSpriteBatch">Sprite batch to draw to.</param>
    public void Draw(SpriteBatch spriteBatch, SpriteBatch uiSpriteBatch)
    {
      spriteBatch.End();

      if ( BackgroundTexture != null ) {
        // Draws the background image if it exists
        uiSpriteBatch.Draw(
          BackgroundTexture,
          position: new Vector2(0, 0)
        );
      }

      // DEBUG: Draws the Views grid to the window
      if ( (bool)MBGame.Console.Vars["drawGrids"] ) {
        uiSpriteBatch.DrawGrid(_grid.Grid, new Point(0, 0), Color.Yellow);
      }

      var rowLen = Content.GetLength(0);
      var colLen = Content.GetLength(1);

      // Draw all elements to the window
      for ( int row = 0; row < rowLen; row++ ) {
        for ( int col = 0; col < colLen; col++ ) {
          var currGrid = _grid.Elements[row, col];
          if ( currGrid != null ) {
            currGrid.Draw(uiSpriteBatch);
          }
        }
      }

      spriteBatch.Begin(
        transformMatrix: MBGame.Camera.GetViewMatrix()
      );
    }

    /// <summary>
    /// Adds a new UIElement to the View
    /// </summary>
    /// <param name="element">Element to add.</param>
    /// <param name="atRow">Row position in the View.</param>
    /// <param name="atCol">Column position in the View.</param>
    /// <param name="rowSpan">Number of rows the element takes up.</param>
    /// <param name="colSpan">Number of columns the element takes up.</param>
    public void Add(UIElement element, int atRow, int atCol, int rowSpan, int colSpan)
    {
      element.SetRelativeSize(_grid, atRow, atCol, rowSpan, colSpan);
      _grid.Elements[atRow, atCol] = element;
      AddToLookup(element);
    }

    /// <summary>
    /// Adds an element to the lookup table used by the view
    /// </summary>
    /// <param name="element">Element to add.</param>
    public void AddToLookup(UIElement element)
    {
      var tag = element.Tag;
      if ( tag == string.Empty ) {
        tag = element.GetType().Name + _elementLookup.Count;
      }
      // Only add if they're not already in the table
      if ( !_elementLookup.ContainsKey(tag) ) {
        _elementLookup.Add(tag, element);
        // Search all elements if this is a layout and not a regular element
        if ( element.GetType() == typeof(Layout) ) {
          foreach ( var e in element.Content.Elements ) {
            if ( e != null ) {
              AddToLookup(element);
            }
          }
        }
      }
    }

    /// <summary>
    /// Gets the <see cref="T:MB2D.UI.UIElement"/> with the specified key.
    /// </summary>
    /// <param name="key">Tag of the element.</param>
    public UIElement this[string key]
    {
      get { return _elementLookup[key]; }
    }

    /// <summary>
    /// Gets the elements this View contains in a 2D array
    /// </summary>
    /// <value>All UIElements.</value>
    public UIElement[,] Content
    {
      get { return _grid.Elements; }
    }

    /// <summary>
    /// Gets or sets the background texture of the view.
    /// </summary>
    /// <value>The background texture.</value>
    public Texture2D BackgroundTexture { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the background image shoudl stretch to fit
    /// the window.
    /// </summary>
    /// <value><c>true</c> if background should be stretched; otherwise, <c>false</c>.</value>
    public bool StretchBackground { get; set; }
  }
}
