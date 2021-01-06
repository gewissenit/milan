#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;

namespace Milan.VisualModeling.Shapes
{
  /// <summary>
  ///   Indicates which end of the line has an arrow.
  /// </summary>
  [Flags]
  public enum ArrowEnds
  {
    None = 0,
    Start = 1,
    End = 2,
    Both = 3
  }
}