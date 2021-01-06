#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.Collections.Generic;
using System.Linq;

namespace Milan.Simulation.Resources.Statistics
{
  public class ResourceInfluenceStatistic
  {
    public ResourceInfluenceStatistic(Resource resource,
                                      IInfluence influence,
                                      IEnumerable<int> counts,
                                      IEnumerable<double> min,
                                      IEnumerable<double> avg,
                                      IEnumerable<double> max,
                                      IEnumerable<double> sum)
    {
      Resource = resource;
      Influence = influence;
      counts = counts.ToArray();
      MinCount = counts.Min();
      AvgCount = counts.Average();
      MaxCount = counts.Max();

      SumCounts = counts.Sum();
      Min = GetAverage(min);
      Avg = GetAverage(avg);
      Max = GetAverage(max);
      Sum = GetAverage(sum);
    }

    public int SumCounts { get; private set; }

    public Resource Resource { get; private set; }
    public IInfluence Influence { get; private set; }

    public int MinCount { get; private set; }
    public double AvgCount { get; private set; }
    public int MaxCount { get; private set; }

    public double Min { get; private set; }
    public double Avg { get; private set; }
    public double Max { get; private set; }
    public double Sum { get; private set; }


    private double GetAverage(IEnumerable<double> values)
    {
      return values.Average();
    }
  }

  public class DetailedInfluenceDatum
  {
    public int Experiment { get; set; }
    public string ResourcePool { get; set; }
    public string ResourceType { get;  set; }
    public string Resource { get;  set; }
    public string Influence { get;  set; }
    public ValueAtPointInTime[] ValuesOverTime { get;  set; }    
  }

  public class ValueAtPointInTime
  {
    public double PointInTime { get; set; }
    public double Value { get; set; }
  }
}