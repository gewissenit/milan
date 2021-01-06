#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using Newtonsoft.Json;

namespace Emporer.Math.Distribution
{
  [JsonObject(MemberSerialization.OptIn)]
  public class LogNormalDistributionConfiguration : NormalDistributionConfiguration
  {
    public LogNormalDistributionConfiguration()
    {
      Name = "Log Normal";
    }

    [JsonProperty]
    public double Minimum
    {
      get { return Get<double>(); }
      set { Set(value); }
    }

    [JsonProperty]
    public bool ParamsNormal
    {
      get { return Get<bool>(); }
      set { Set(value); }
    }

    public override string ToString()
    {
      if (Mean == 0 &&
          StandardDeviation == 0 &&
          Minimum == 0)
      {
        return string.Format("Log Normal");
      }
      return string.Format("{0} +/-{1}, min {2} (lognorm.)", Mean, StandardDeviation, Minimum);
    }
  }
}