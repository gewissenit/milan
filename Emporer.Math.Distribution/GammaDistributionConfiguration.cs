#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using Newtonsoft.Json;

namespace Emporer.Math.Distribution
{
  [JsonObject(MemberSerialization.OptIn)]
  public class GammaDistributionConfiguration : DistributionConfiguration
  {
    public GammaDistributionConfiguration()
    {
      Name = "Gamma";
    }

    [JsonProperty]
    public double Minimum
    {
      get { return Get<double>(); }
      set { Set(value); }
    }

    [JsonProperty]
    public double Shape
    {
      get { return Get<double>(); }
      set { Set(value); }
    }

    [JsonProperty]
    public double Scale
    {
      get { return Get<double>(); }
      set { Set(value); }
    }

    public override string ToString()
    {
      return string.Format("min {0} shape {1} scale {2} (gamma.)", Minimum, Shape, Scale);
    }
  }
}