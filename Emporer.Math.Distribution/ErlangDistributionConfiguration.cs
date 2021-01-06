#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using Newtonsoft.Json;

namespace Emporer.Math.Distribution
{
  [JsonObject(MemberSerialization.OptIn)]
  public class ErlangDistributionConfiguration : DistributionConfiguration
  {
    public ErlangDistributionConfiguration()
    {
      Name = "Erlang";
    }

    [JsonProperty]
    public double Minimum
    {
      get { return Get<double>(); }
      set { Set(value); }
    }

    [JsonProperty]
    public int Order
    {
      get { return Get<int>(); }
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