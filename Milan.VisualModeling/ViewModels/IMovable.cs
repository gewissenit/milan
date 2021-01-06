#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.Windows;

namespace Milan.VisualModeling.ViewModels
{
  public interface IMovable : IVisual
  {
    void MoveBy(Vector delta);
    void MoveBy(double deltaX, double deltaY);
    void MoveHorizontallyBy(double deltaX);
    void MoveHorizontallyTo(double x);
    void MoveTo(Point location);
    void MoveTo(double x, double y);
    void MoveVerticallyBy(double deltaY);
    void MoveVerticallyTo(double y);
    bool CanMoveTo(Point location);
    bool CanMoveBy(Vector delta);
  }
}