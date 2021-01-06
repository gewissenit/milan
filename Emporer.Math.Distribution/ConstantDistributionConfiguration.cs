#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using Newtonsoft.Json;

namespace Emporer.Math.Distribution
{
  [JsonObject(MemberSerialization.OptIn)]
  public class ConstantDistributionConfiguration : DistributionConfiguration
  {
    public ConstantDistributionConfiguration()
    {
      Name = "Constant";
    }

    [JsonProperty]
    public double ConstantValue
    {
      get { return Get<double>(); }
      set { Set(value); }
    }

    public override string ToString()
    {
      return string.Format("{0} (const.)", ConstantValue);
    }
  }
}