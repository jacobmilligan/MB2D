//
// 	MBMath.cs
// 	MB2D Engine
//
// 	--------------------------------------------------------------
//
// 	Created by Jacob Milligan on 11/10/2016.
// 	Copyright (c) Jacob Milligan All rights reserved
//
using System;
using Microsoft.Xna.Framework;

namespace MB2D
{
  /// <summary>
  /// Math helper class
  /// </summary>
  public static class MBMath
  {
    /// <summary>
    /// Wraps a value around a 2D grid coordinate
    /// </summary>
    /// <returns>The wrapped point.</returns>
    /// <param name="x">The x coordinate to wrap.</param>
    /// <param name="y">The y coordinate to wrap.</param>
    /// <param name="width">Width of the 2D grid.</param>
    /// <param name="height">Height of the 2D grid.</param>
    public static Point WrapGrid(int x, int y, int width, int height)
    {
      var newX = (x % width + width) % width;
      var newY = (y % height + height) % height;
      return new Point((int)newX, (int)newY);
    }

    /// <summary>
    /// Normalizes a sample of a data set to be between two arbitrary values.
    /// </summary>
    /// <param name="value">Sample to normalize.</param>
    /// <param name="low">Lower bound of the normalized result.</param>
    /// <param name="high">Upper bound of the normalized result.</param>
    /// <param name="dataMin">Data minimum.</param>
    /// <param name="dataMax">Data max.</param>
    public static float Normalize(float value, float low, float high, float dataMin, float dataMax)
    {
      return (high - low) * ((value - dataMin) / (dataMax - dataMin)) + low;
    }

    /// <summary>
    /// Normalizes a sample of a data set to be between two arbitrary values.
    /// </summary>
    /// <param name="value">Sample to normalize.</param>
    /// <param name="low">Lower bound of the normalized result.</param>
    /// <param name="high">Upper bound of the normalized result.</param>
    /// <param name="dataMin">Data minimum.</param>
    /// <param name="dataMax">Data max.</param>
    public static double Normalize(double value, double low, double high, double dataMin, double dataMax)
    {
      return (high - low) * ((value - dataMin) / (dataMax - dataMin)) + low;
    }
  }
}
