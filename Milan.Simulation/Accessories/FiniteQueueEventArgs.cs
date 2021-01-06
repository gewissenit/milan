#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;

namespace Milan.Simulation.Accessories
{
  public class FiniteQueueEventArgs : EventArgs
  {
    public FiniteQueueEventArgs(object category)
    {
      Category = category;
    }

    public object Category { get; set; }
  }
}