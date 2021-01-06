#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using Milan.JsonStore;
using Newtonsoft.Json;

namespace Emporer.Material
{
  [JsonObject(MemberSerialization.OptIn)]
  public class MaterialProperty : DomainEntity, IMaterialProperty
  {
    [JsonProperty]
    public IPropertyType PropertyType
    {
      get { return Get<IPropertyType>(); }
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