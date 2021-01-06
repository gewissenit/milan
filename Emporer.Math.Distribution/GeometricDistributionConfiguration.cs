#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using Newtonsoft.Json;

namespace Emporer.Math.Distribution
{
  [JsonObject(MemberSerialization.OptIn)]
  public class GeometricDistributionConfiguration : DistributionConfiguration
  {
    public GeometricDistributionConfiguration()
    {
      Name = "Geometric";
    }

    [JsonProperty]
    public double Probability
    {
      get { return Get<double>(); }
      set { Set(value); }
    }
  }
}