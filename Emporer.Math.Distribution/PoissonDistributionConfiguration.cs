#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using Newtonsoft.Json;

namespace Emporer.Math.Distribution
{
  [JsonObject(MemberSerialization.OptIn)]
  public class PoissonDistributionConfiguration : DistributionConfiguration
  {
    public PoissonDistributionConfiguration()
    {
      Name = "Poisson";
    }

    [JsonProperty]
    public double Mean
    {
      get { return Get<double>(); }
      set { Set(value); }
    }

    [JsonProperty]
    public double ExpMean
    {
      get { return Get<double>(); }
      set { Set(value); }
    }
  }
}