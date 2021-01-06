#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using Milan.JsonStore;
using Newtonsoft.Json;

namespace Emporer.Material
{
  [JsonObject(MemberSerialization.OptIn)]
  public class ContainedMaterial : DomainEntity, IContainedMaterial
  {
    [JsonProperty]
    public double Amount
    {
      get { return Get<double>(); }
      set { Set(value); }
    }

    [JsonProperty]
    public IMaterial Material
    {
      get { return Get<IMaterial>(); }
      set { Set(value); }
    }
  }
}