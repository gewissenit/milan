#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using Newtonsoft.Json;

namespace Emporer.Math.Distribution
{
  [JsonObject(MemberSerialization.OptIn)]
  public class BernoulliDistributionConfiguration : DistributionConfiguration
  {
    public BernoulliDistributionConfiguration()
    {
      Name = "Bernoulli";
    }

    [JsonProperty]
    public double Probability
    {
      get { return Get<double>(); }
      set { Set(value); }
    }
  }
}