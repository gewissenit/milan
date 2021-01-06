#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.Windows;
using ReactiveUI;

namespace Milan.VisualModeling.ViewModels
{
  /// <summary>
  ///   A point on a plane (2D space).
  /// </summary>
  public class Coordinate : ReactiveObject, ICoordinate
  {
    private double _x;
    private double _y;

    /// <summary>
    ///   Creates a coordinate at [0|0].
    /// </summary>
    public Coordinate()
    {
      _x = 0;
      _y = 0;
    }

    /// <summary>
    ///   Creates a coordinate at [x|y].
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public Coordinate(double x, double y)
    {
      _x = x;
      _y = y;
    }

    /// <summary>
    ///   Creates a coordinate at the given point
    /// </summary>
    /// <param name="location"></param>
    public Coordinate(Point location)
      : this(location.X, location.Y)
    {
    }

    public virtual double X
    {
      get { return _x; }
      set { this.RaiseAndSetIfChanged(ref _x, value); }
    }

    public virtual double Y
    {
      get { return _y; }
      set { this.RaiseAndSetIfChanged(ref _y, value); }
    }

    public override string ToString()
    {
      return string.Format("[{0:0.00}|{1:0.00}]", X, Y);
    }
  }
}