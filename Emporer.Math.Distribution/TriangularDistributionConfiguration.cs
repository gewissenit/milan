#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using Newtonsoft.Json;

namespace Emporer.Math.Distribution
{
  [JsonObject(MemberSerialization.OptIn)]
  public class TriangularDistributionConfiguration : DistributionConfiguration
  {
    public TriangularDistributionConfiguration()
    {
      Name = "Triangular";
    }

    [JsonProperty]
    public double UpperBorder
    {
      get { return Get<double>(); }
      set { Set(value); }
    }

    [JsonProperty]
    public double LowerBorder
    {
      get { return Get<double>(); }
      set { Set(value); }
    }

    [JsonProperty]
    public double Mean
    {
      get { return Get<double>(); }
      set { Set(value); }
    }
  }
}