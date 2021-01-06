#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using GeWISSEN.Utils;
using Milan.VisualModeling.ViewModels;

namespace Milan.VisualModeling.Extensions
{
  public static class Geometry
  {
    public static double GetDistanceTo(this ICoordinate self, ICoordinate other)
    {
      return GetDistanceTo(self.ToPoint(), other.ToPoint());
    }

    public static double GetDistanceTo(this Point self, Point other)
    {
      var distX = self.X - other.X;
      var distY = self.Y - other.Y;

      return Math.Sqrt(distX * distX + distY * distY);
    }

    public static Point GetNearest(this Point self, IEnumerable<Point> others)
    {
      var candidates = others.ToArray();
      if (!candidates.Any())
      {
        throw new ArgumentException("Collection of other coordinates is empty. Please supply at least one element.");
      }

      return candidates.MinBy(x => self.GetDistanceTo(x));
    }

    public static ICoordinate GetNearest(this ICoordinate self, IEnumerable<ICoordinate> others)
    {
      var candidates = others.ToArray();
      if (!candidates.Any())
      {
        throw new ArgumentException("Collection of other coordinates is empty. Please supply at least one element.");
      }

      ICoordinate nearest = null;
      var shortestDistance = double.MaxValue;
      foreach (var candidate in candidates)
      {
        var distance = self.GetDistanceTo(candidate);
        if (!(distance < shortestDistance))
        {
          continue;
        }
        shortestDistance = distance;
        nearest = candidate;
      }
      return nearest;
    }

    public static Point GetCenter(this Rect rectangle)
    {
      return new Point(rectangle.X+rectangle.Width/2, rectangle.Y+rectangle.Height/2);
    }


    public static bool IsEqualTo(this ICoordinate coordinate, Point point)
    {
      return Math.Abs(coordinate.X - point.X) < double.Epsilon && (coordinate.Y - point.Y) < double.Epsilon;
    }

    public static Point ToPoint(this Vector vector)
    {
      return new Point(vector.X, vector.Y);
    }

    public static Point ToPoint(this ICoordinate coordinate)
    {
      return new Point(coordinate.X, coordinate.Y);
    }

    public static ICoordinate ToCoordinate(this Point point)
    {
      return new Coordinate(point.X, point.Y);
    }

    public static Vector ToVector(this Point point)
    {
      return new Vector(point.X, point.Y);
    }

    public static void UpdateTo(this ICoordinate coordinate, Point point)
    {
      coordinate.X = point.X;
      coordinate.Y = point.Y;
    }
  }
}