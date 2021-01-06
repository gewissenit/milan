#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

namespace Milan.Simulation.Observers
{
  public interface ITimeReferenced
  {
    TimeReference TimeReference { get; set; }
  }
}