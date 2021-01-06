#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using Milan.JsonStore;
using Newtonsoft.Json;

namespace Milan.Simulation
{
  [JsonObject(MemberSerialization.OptIn)]
  public class ProductTypeAmount : DomainEntity, IProductTypeAmount
  {
    public ProductTypeAmount()
    {
      Amount = 1;
    }

    public ProductTypeAmount(IProductType productType, int amount)
      : this()
    {
      ProductType = productType;
      Amount = amount;
    }

    [JsonProperty]
    public IProductType ProductType
    {
      get { return Get<IProductType>(); }
      set { Set(value); }
    }

    [JsonProperty]
    public int Amount
    {
      get { return Get<int>(); }
      set { Set(value); }
    }
    
    public override string ToString()
    {
      var type = string.Empty;
      if (ProductType != null)
      {
        type = ProductType.Name;
      }

      return string.Format("{0} of {1}", Amount, type);
    }
  }
}