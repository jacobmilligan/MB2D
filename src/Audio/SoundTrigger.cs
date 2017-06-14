//
// 	SoundTrigger.cs
// 	MB2D Engine
//
// 	--------------------------------------------------------------
//
// 	Created by Jacob Milligan on 13/10/2016.
// 	Copyright (c) Jacob Milligan All rights reserved
//
using System;
using Microsoft.Xna.Framework.Audio;

namespace MB2D
{
  /// <summary>
  /// Triggers a sound effect
  /// </summary>
  public class SoundTrigger
  {
    /// <summary>
    /// The sound effect to play
    /// </summary>
    private SoundEffect _sound;
    /// <summary>
    /// The sound effects instance used to fade and play/stop
    /// </summary>
    private SoundEffectInstance _instance;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:MB2D.SoundTrigger"/> class
    /// using the specified SoundEffect
    /// </summary>
    /// <param name="sound">Sound to use.</param>
    public SoundTrigger(SoundEffect sound)
    {
      _sound = sound;
      _instance = _sound.CreateInstance();
      FadeSpeed = 0.05f;
      MaxVolume = 0.5f;
    }

    /// <summary>
    /// Plays the sound if it's not already playing
    /// </summary>
    public void Trigger()
    {
      if ( _instance.State == SoundState.Stopped ) {
        _instance.Play();
      }
    }

    /// <summary>
    /// Stops the sound if it's playing
    /// </summary>
    public void Cut()
    {
      _instance.Stop();
    }

    /// <summary>
    /// Increases the sounds volume one step based on the 
    /// specified FadeSpeed
    /// </summary>
    public void FadeUp()
    {
      if ( _instance.State == SoundState.Stopped ) {
        _instance.Play();
        _instance.Volume = 0.0f;
      }
      if ( _instance.Volume < MaxVolume ) {
        _instance.Volume += FadeSpeed;
      }
    }

    /// <summary>
    /// Decreases the volume based on the specified
    /// FadeSpeed. Stops the sound once the volume reaches 0
    /// </summary>
    public void FadeDown()
    {
      if ( _instance.Volume > FadeSpeed ) {
        _instance.Volume -= FadeSpeed;
      } else {
        _instance.Stop();
      }
    }

    /// <summary>
    /// Gets or sets the fade speed.
    /// </summary>
    /// <value>The fade speed.</value>
    public float FadeSpeed { get; set; }

    /// <summary>
    /// Determines the maximum volume to FadeUp
    /// </summary>
    /// <value>The max volume.</value>
    public float MaxVolume { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="T:MB2D.SoundTrigger"/> is looped
    /// or one-shot.
    /// </summary>
    /// <value><c>true</c> if is looped; otherwise, <c>false</c>.</value>
    public bool IsLooped
    {
      get { return _instance.IsLooped; }
      set { _instance.IsLooped = value; }
    }
  }
}
