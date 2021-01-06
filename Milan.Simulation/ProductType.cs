#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using Newtonsoft.Json;

namespace Milan.Simulation
{
  [JsonObject(MemberSerialization.OptIn)]
  public class ProductType : Entity, IProductType
  {
    [JsonProperty]
    public string IconId
    {
      get { return Get<string>(); }
      set { Set(value); }
    }
  }
}