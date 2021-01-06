#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using Milan.JsonStore;
using Newtonsoft.Json;

namespace Emporer.Material
{
  [JsonObject(MemberSerialization.OptIn)]
  public class PropertyType : DomainEntity, IPropertyType
  {
    [JsonProperty]
    public string Name
    {
      get { return Get<string>(); }
      set { Set(value); }
    }

    [JsonProperty]
    public string Location
    {
      get { return Get<string>(); }
      set { Set(value); }
    }

    [JsonProperty]
    public string EcoCat
    {
      get { return Get<string>(); }
      set { Set(value); }
    }

    [JsonProperty]
    public string EcoSubCat
    {
      get { return Get<string>(); }
      set { Set(value); }
    }

    [JsonProperty]
    public string Unit
    {
      get { return Get<string>(); }
      set { Set(value); }
    }

    [JsonProperty]
    public string DataSourceId
    {
      get { return Get<string>(); }
      set { Set(value); }
    }
  }
}