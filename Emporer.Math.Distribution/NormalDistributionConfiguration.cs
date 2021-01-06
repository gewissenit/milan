#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using Newtonsoft.Json;

namespace Emporer.Math.Distribution
{
  [JsonObject(MemberSerialization.OptIn)]
  public class NormalDistributionConfiguration : DistributionConfiguration
  {
    public NormalDistributionConfiguration()
    {
      Name = "Normal";
    }

    [JsonProperty]
    public double Mean
    {
      get { return Get<double>(); }
      set { Set(value); }
    }

    [JsonProperty]
    public double StandardDeviation
    {
      get { return Get<double>(); }
      set { Set(value); }
    }

    public override string ToString()
    {
      return string.Format("{0} +/-{1} (norm.)", Mean, StandardDeviation);
    }
  }
}