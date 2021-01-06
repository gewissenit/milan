#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.Windows;

namespace Milan.VisualModeling.ViewModels
{
  public interface IVisual
  {
    ICoordinate Location { get; }
    double Width { get; set; }
    double Height { get; set; }
    Rect Bounds { get; }
    object Model { get; }
  }
}