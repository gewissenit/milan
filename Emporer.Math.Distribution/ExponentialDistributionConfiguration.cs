#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using Newtonsoft.Json;

namespace Emporer.Math.Distribution
{
  [JsonObject(MemberSerialization.OptIn)]
  public class ExponentialDistributionConfiguration : DistributionConfiguration
  {
    public ExponentialDistributionConfiguration()
    {
      Name = "Exponential";
    }

    [JsonProperty]
    public double Mean
    {
      get { return Get<double>(); }
      set { Set(value); }
    }

    [JsonProperty]
    public double Minimum
    {
      get { return Get<double>(); }
      set { Set(value); }
    }

    public override string ToString()
    {
      if (Mean == 0 &&
          Minimum == 0)
      {
        return string.Format("Exponential");
      }
      return string.Format("Mean {0}, Minimum {1} (expo.)", Mean, Minimum);
    }
  }
}