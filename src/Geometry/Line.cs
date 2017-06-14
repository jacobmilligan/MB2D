using Microsoft.Xna.Framework;

namespace MB2D.Geometry
{
  /// <summary>
  /// A line structure, can be drawn via SpriteBatch
  /// </summary>
  public class Line
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:MB2D.Geometry.Line"/> class.
    /// </summary>
    /// <param name="start">Start point</param>
    /// <param name="end">End point</param>
    public Line(Vector2 start, Vector2 end)
    {
      Start = start;
      End = end;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:MB2D.Geometry.Line"/> class.
    /// </summary>
    /// <param name="start">Start point</param>
    /// <param name="end">End point</param>
    public Line(Point start, Point end) : this(start.ToVector2(), end.ToVector2()) { }

    /// <summary>
    /// Gets the distance from this line a given point is
    /// </summary>
    /// <returns>The distance.</returns>
    /// <param name="point">Point to calculate.</param>
    public float PointDistance(Vector2 point)
    {
      float result = 0;
      Vector2 d = End - Start;
      float length = d.Length();
      d.Normalize();
      result = Vector2.Dot((point - End), d);
      if ( result < 0 ) {
        result = (point - Start).Length();
      } else if ( result > length ) {
        result = (point - End).Length();
      } else {
        result = (point - (Start + d * result)).Length();
      }
      return result;
    }

    /// <summary>
    /// Gets the distance from this line a given point is
    /// </summary>
    /// <returns>The distance.</returns>
    /// <param name="point">Point to calculate.</param>
    public float PointDistance(Point point)
    {
      return PointDistance(new Vector2(point.X, point.Y));
    }

    /// <summary>
    /// Gets or sets the start point.
    /// </summary>
    /// <value>The start point</value>
    public Vector2 Start { get; set; }
    /// <summary>
    /// Gets or sets the end point.
    /// </summary>
    /// <value>The end point.</value>
    public Vector2 End { get; set; }
  }
}

