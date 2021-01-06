#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;

namespace Milan.Simulation.Statistics
{
  public class BatchFillLevelStatistic
  {
    public BatchFillLevelStatistic(string entity, string productType, IEnumerable<HistoricalValueStore<int>> fillLevels, int capacity)
    {
      if (entity == null ||
          productType == null ||
          fillLevels == null)
      {
        throw new ArgumentNullException();
      }

      Entity = entity;
      ProductType = productType;
      FillLevels = fillLevels;
      Capacity = capacity;

      CalculateStatistics();
    }

    public string Entity { get; private set; }
    public string ProductType { get; private set; }
    public IEnumerable<HistoricalValueStore<int>> FillLevels { get; set; }

    public double Minimum { get; private set; }
    public double Mean { get; private set; }
    public double Maximum { get; private set; }
    public double Capacity { get; private set; }

    public TimeSpan TimeEmpty { get; private set; }
    public TimeSpan TimeInUse { get; private set; }
    public TimeSpan TimeFull { get; private set; }

    public IEnumerable<KeyValuePair<string, double>> DataPointsAccumulated { get; private set; }
    public Queue<TimedValue<int>> DataPointsTime { get; private set; }

    private void CalculateStatistics()
    {
      Minimum = FillLevels.Average(fl => fl.Minimum);
      Maximum = FillLevels.Average(fl => fl.Maximum);
      Mean = FillLevels.Average(fl => fl.Mean);

      DataPointsAccumulated = new[]
                              {
                                new KeyValuePair<string, double>("Min", Minimum), new KeyValuePair<string, double>("Avg", Mean),
                                new KeyValuePair<string, double>("Max", Maximum), new KeyValuePair<string, double>("Cap", Capacity)
                              };

      //DataPointsTime = new[] {new KeyValuePair<string, double>("Empty", TimeEmpty.Ticks), new KeyValuePair<string, double>("Ready", TimeInUse.Ticks), new KeyValuePair<string, double>("Full", TimeFull.Ticks)};

      if (FillLevels.Any())
      {
        DataPointsTime = FillLevels.First()
                                   .ValuesOverTime;
      }
    }
  }
}