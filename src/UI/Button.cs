//
// 	Button.cs
// 	MB2D Engine
//
// 	--------------------------------------------------------------
//
// 	Created by Jacob Milligan on 3/10/2016.
// 	Copyright (c) Jacob Milligan All rights reserved
//
using System;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MB2D.IO;
using MB2D.UI;

namespace MB2D.UI
{
  /// <summary>
  /// A pressable ui element with a single OnPress event
  /// </summary>
  public class Button : UIControlElement
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:MB2D.UI.Button"/> class.
    /// </summary>
    /// <param name="normal">Normal state texture</param>
    /// <param name="selected">Selected state texture.</param>
    /// <param name="pressed">Pressed state texture.</param>
    public Button(Texture2D normal, Texture2D selected, Texture2D pressed
    ) : base(normal, selected, pressed)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:MB2D.UI.Button"/> class
    /// with no associated textures
    /// </summary>
    public Button() : base(null, null, null) { }

    /// <summary>
    /// Updates the button state.
    /// </summary>
    public override void Update()
    {
      base.Update();

      if ( _currentState == UIState.Selected && IOUtil.LeftMouseClicked() ) {
        if ( OnPress != null ) {
          if ( PressedSound != null && _previousState != UIState.Pressed ) {
            PressedSound.Play();
          }
          OnPress(this, EventArgs.Empty);
        }
      }
    }

    /// <summary>
    /// Occurs when the button has been clicked or pressed.
    /// </summary>
    public event EventHandler OnPress;

    /// <summary>
    /// Gets or sets the sound fired when transitioning to the pressed state.
    /// </summary>
    /// <value>The pressed state sound.</value>
    public SoundEffectInstance PressedSound { get; set; }
  }
}
