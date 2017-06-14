//
// 	Vector2Extensions.cs
// 	MB2D Engine
//
// 	--------------------------------------------------------------
//
// 	Created by Jacob Milligan on 9/10/2016.
// 	Copyright (c) Jacob Milligan All rights reserved
//
using System;
using Microsoft.Xna.Framework;

namespace MB2D
{
  public static class Vector2Extensions
  {
    /// <summary>
    /// Gets a scale vector that fits inside a parent vector
    /// </summary>
    /// <returns>The size fitting vector</returns>
    /// <param name="child">Child size vector.</param>
    /// <param name="parent">Parent size vector.</param>
    public static Vector2 FitInto(this Vector2 child, Vector2 parent)
    {
      var scale = new Vector2(1, 1);
      if ( child.X > parent.X ) {
        scale.X = parent.X / child.X;
      }
      if ( child.Y > parent.Y ) {
        scale.Y = parent.Y / child.Y;
      }
      return scale;
    }

    /// <summary>
    /// Gets a scale vector that fits exactly inside a parent vector
    /// </summary>
    /// <returns>The size fitting vector</returns>
    /// <param name="child">Child size vector.</param>
    /// <param name="parent">Parent size vector.</param>
    /// <param name="fill">If true, the scale will stretch to fit the parent exactly.</param>
    public static Vector2 FitInto(this Vector2 child, Vector2 parent, bool fill)
    {
      var scale = new Vector2(1, 1);
      if ( child.X > parent.X || fill ) {
        scale.X = parent.X / child.X;
      }
      if ( child.Y > parent.Y || fill ) {
        scale.Y = parent.Y / child.Y;
      }
      return scale;
    }
  }
}
