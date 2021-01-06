#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using Emporer.Math.Distribution;
using Milan.JsonStore;
using Newtonsoft.Json;

namespace Milan.Simulation
{
  [JsonObject(MemberSerialization.OptIn)]
  public class ProductTypeDistribution : DomainEntity, IProductTypeDistribution
  {
    [JsonProperty]
    public IProductType ProductType
    {
      get { return Get<IProductType>(); }
      set { Set(value); }
    }

    [JsonProperty]
    public IDistributionConfiguration DistributionConfiguration
    {
      get { return Get<IDistributionConfiguration>(); }
      set { Set(value); }
    }

    public override string ToString()
    {
      var type = string.Empty;
      if (ProductType != null)
      {
        type = ProductType.Name;
      }

      var distCfg = string.Empty;
      if (DistributionConfiguration != null)
      {
        distCfg = DistributionConfiguration.ToString();
      }

      return string.Format("{0} : {1}", type, distCfg);
    }
  }
}