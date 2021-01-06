#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;

namespace Milan.Simulation.Statistics
{
  public class BatchProcessStatistic
  {
    public BatchProcessStatistic(string batchId)
    {
      BatchId = batchId;
    }

    public string BatchId { get; private set; }

    public string Entity { get; set; }
    public string ProcessType { get; set; }
    public string ProductType { get; set; }

    public double MinCount { get; set; }
    public double AvgCount { get; set; }
    public double MaxCount { get; set; }

    public TimeSpan SumDuration { get; set; }
    public TimeSpan MinDuration { get; set; }
    public TimeSpan AvgDuration { get; set; }
    public TimeSpan MaxDuration { get; set; }

    public double MinProcessedProductsPerProcess { get; set; }
    public double AvgProcessedProductsPerProcess { get; set; }
    public double MaxProcessedProductsPerProcess { get; set; }

    public double MinTotalProcessedProducts { get; set; }
    public double AvgTotalProcessedProducts { get; set; }
    public double MaxTotalProcessedProducts { get; set; }

    public override string ToString()
    {
      return $"[{Entity}|{ProcessType}|{ProductType}] #:{AvgCount} d:{AvgDuration}";
    }
  }
}