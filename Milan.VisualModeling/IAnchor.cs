#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Windows;
using System.Xml.Serialization;

namespace Milan.VisualModeling
{
  public interface IAnchor : IXmlSerializable
  {
    Point Point { get; }

    int X { get; set; }

    int Y { get; set; }


    void MoveBy(int horizontalOffset, int verticalOffset);


    void MoveBy(Size offset);


    void MoveTo(int x, int y);


    void MoveTo(Point coordinates);


    event EventHandler LocationChanged;
  }
}