#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using Newtonsoft.Json;

namespace Emporer.Math.Distribution
{
  [JsonObject(MemberSerialization.OptIn)]
  public class WeibullDistributionConfiguration : DistributionConfiguration
  {
    public WeibullDistributionConfiguration()
    {
      Name = "Weibull";
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
      if (Scale == 0 &&
          Shape == 0)
      {
        return string.Format("Weibull");
      }
      return string.Format("shape {0} scale {1} (weibull.)", Shape, Scale);
    }
  }
}