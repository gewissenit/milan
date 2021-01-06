#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;

namespace Milan.Simulation.Statistics
{
  public class UnfinishedProcessStatistic
  {
    public TimeSpan ElapsedDuration { get; set; }
    public string Entity { get; set; }
    public string ExperimentId { get; set; }
    public string ProcessType { get; set; }
    public string ProductType { get; set; }
    public TimeSpan RemainingDuration { get; set; }
  }
}