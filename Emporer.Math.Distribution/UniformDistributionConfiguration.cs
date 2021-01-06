#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using Newtonsoft.Json;

namespace Emporer.Math.Distribution
{
  [JsonObject(MemberSerialization.OptIn)]
  public class UniformDistributionConfiguration : DistributionConfiguration
  {
    public UniformDistributionConfiguration()
    {
      Name = "Uniform";
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

    public override string ToString()
    {
      if (LowerBorder == 0 &&
          UpperBorder == 0)
      {
        return string.Format("Uniform");
      }
      return string.Format("{0}-{1} (uniform)", LowerBorder, UpperBorder);
    }
  }
}