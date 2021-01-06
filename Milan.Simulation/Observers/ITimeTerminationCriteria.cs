#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;

namespace Milan.Simulation.Observers
{
  public interface ITimeTerminationCriteria : ITerminationCriteria
  {
    DateTime StopDate { get; set; }
    TimeSpan Duration { get; set; }
  }
}