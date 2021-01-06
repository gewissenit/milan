#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;

namespace Milan.Simulation
{
  public class BatchEventArgs : EventArgs
  {
    public BatchEventArgs(IBatch batch)
    {
      Batch = batch;
    }

    public IBatch Batch { get; private set; }
  }
}