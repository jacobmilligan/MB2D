//
// 	UIControlElement.cs
// 	MB2D Engine
//
// 	--------------------------------------------------------------
//
// 	Created by Jacob Milligan on 29/09/2016.
// 	Copyright (c) Jacob Milligan All rights reserved
//
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MB2D.UI
{
  /// <summary>
  /// Represents the current state of a controllable UIElement
  /// </summary>
  public enum UIState
  {
    /// <summary>
    /// Unselected, unpressed state
    /// </summary>
    Normal,
    /// <summary>
    /// Hovered or highlighted state
    /// </summary>
    Selected,
    /// <summary>
    /// Clicked or pressed state
    /// </summary>
    Pressed
  }

  /// <summary>
  /// An interactive and controllable UIElement
  /// </summary>
  public class UIControlElement : UIElement
  {
    /// <summary>
    /// The current UIState of the element
    /// </summary>
    protected UIState _currentState;
    /// <summary>
    /// The last state of the element
    /// </summary>
    protected UIState _previousState;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:MB2D.UI.UIControlElement"/> class.
    /// </summary>
    /// <param name="normal">Normal state texture</param>
    /// <param name="selected">Selected state texture</param>
    /// <param name="pressed">Pressed state texture</param>
    public UIControlElement(Texture2D normal, Texture2D selected, Texture2D pressed) : base(1, 1)
    {
      NormalTexture = normal;
      HighlightedTexture = selected;
      PressedTexture = pressed;

      NormalTextColor = HighlightedTextColor = TextColor;
      _currentState = UIState.Normal;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:MB2D.UI.UIControlElement"/> class
    /// with no textures associated
    /// </summary>
    public UIControlElement() : this(null, null, null) { }

    /// <summary>
    /// Update the UIState of the control element
    /// based on mouse position
    /// </summary>
    public override void Update()
    {
      var mousePos = Mouse.GetState().Position;
      _previousState = _currentState;

      var size = Font.MeasureString(TextContent);
      var scale = Font.MeasureString(TextContent).FitInto(
        Content.Grid.CellSize, Fill
      );

      var rect = new Rectangle(
        Content.Rect.Location, (size * scale).ToPoint()
      );
      if ( NormalTexture != null ) {
        rect = NormalTexture.Bounds;
      }

      // Selected state
      if ( rect.Contains(mousePos) ) {
        _currentState = UIState.Selected;
        TextColor = HighlightedTextColor;

        if ( HighlightedSound != null && _previousState == UIState.Normal ) {
          HighlightedSound.Play();
        }
      } else {
        _currentState = UIState.Normal;
        TextColor = NormalTextColor;
      }

    }

    /// <summary>
    /// Draws the texture associated with the elements current UIState
    /// and then its TextContent on top of the texture
    /// </summary>
    /// <param name="spriteBatch">Sprite batch to draw to</param>
    public override void Draw(SpriteBatch spriteBatch)
    {
      base.Draw(spriteBatch);

      var drawnTexture = NormalTexture;
      // Get the correct texture state to draw
      switch ( _currentState ) {
        case UIState.Selected:
          drawnTexture = HighlightedTexture;
          break;
        case UIState.Pressed:
          drawnTexture = PressedTexture;
          break;
      }

      // This elements position
      var pos = new Vector2(Content.Rect.X, Content.Rect.Y);

      if ( drawnTexture != null ) {

        var scale = drawnTexture.Bounds.Size.ToVector2().FitInto(
          Content.Grid.CellSize, Fill
        );

        // Draw the current states texture
        spriteBatch.Draw(
          drawnTexture,
          scale: scale,
          position: pos
        );
      }

      if ( TextContent.Length > 0 ) {
        var scale = Font.MeasureString(TextContent).FitInto(
          Content.Grid.CellSize, Fill
        );

        // Draw the TextContent to fit inside the element
        spriteBatch.DrawString(
          spriteFont: Font,
          text: TextContent,
          position: pos,
          color: TextColor,
          scale: scale,
          rotation: 0,
          origin: new Vector2(0, 0),
          effects: SpriteEffects.None,
          layerDepth: 0
        );
      }

      if ( BorderDisplayed ) {
        DrawBorder(spriteBatch);
      }
    }

    /// <summary>
    /// Gets or sets the TextContent color associated with the Normal UIState of the element.
    /// </summary>
    /// <value>The TextContents normal color</value>
    public Color NormalTextColor { get; set; }
    /// <summary>
    /// Gets or sets the TextContent color associated with the Selected UIState of the element.
    /// </summary>
    /// <value>The TextContents selected color</value>
    public Color HighlightedTextColor { get; set; }

    /// <summary>
    /// Gets or sets the normal UIState texture.
    /// </summary>
    /// <value>The normal texture.</value>
    public Texture2D NormalTexture { get; set; }
    /// <summary>
    /// Gets or sets the selected UIState texture.
    /// </summary>
    /// <value>The selected texture.</value>
    public Texture2D HighlightedTexture { get; set; }
    /// <summary>
    /// Gets or sets the pressed UIState texture.
    /// </summary>
    /// <value>The pressed texture.</value>
    public Texture2D PressedTexture { get; set; }

    /// <summary>
    /// Gets or sets the sound played when an element switches to the selected state.
    /// </summary>
    /// <value>The highlighted state sound effect.</value>
    public SoundEffect HighlightedSound { get; set; }
  }
}
