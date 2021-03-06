﻿using Milan.JsonStore;
using Newtonsoft.Json;

namespace Milan.Simulation.Resources
{
  [JsonObject(MemberSerialization.OptIn)]
  public class ProductTypeSpecificResource : DomainEntity, IProductTypeSpecificResource
  {
    [JsonProperty]
    public IResourceType ResourceType
    {
      get { return Get<IResourceType>(); }
      set { Set(value); }
    }

    [JsonProperty]
    public IProductType ProductType
    {
      get { return Get<IProductType>(); }
      set { Set(value); }
    }
    
    [JsonProperty]
    public IResourcePool ResourcePool
    {
      get { return Get<IResourcePool>(); }
      set { Set(value); }
    }

    [JsonProperty]
    public int Amount
    {
      get { return Get<int>(); }
      set { Set(value); }
    }
  }
}