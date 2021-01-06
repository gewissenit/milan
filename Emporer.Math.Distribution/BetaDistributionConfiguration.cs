#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using Newtonsoft.Json;

namespace Emporer.Math.Distribution
{
  [JsonObject(MemberSerialization.OptIn)]
  public class BetaDistributionConfiguration : DistributionConfiguration
  {
    public BetaDistributionConfiguration()
    {
      Name = "Beta";
    }

    [JsonProperty]
    public double Minimum
    {
      get { return Get<double>(); }
      set { Set(value); }
    }

    [JsonProperty]
    public double Maximum
    {
      get { return Get<double>(); }
      set { Set(value); }
    }

    [JsonProperty]
    public double FirstShape
    {
      get { return Get<double>(); }
      set { Set(value); }
    }

    [JsonProperty]
    public double SecondShape
    {
      get { return Get<double>(); }
      set { Set(value); }
    }
  }
}