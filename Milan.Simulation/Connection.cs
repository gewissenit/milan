#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Collections.Generic;
using Milan.JsonStore;
using Newtonsoft.Json;

namespace Milan.Simulation
{
  [JsonObject(MemberSerialization.OptIn)]
  public class Connection : DomainEntity, IConnection
  {
    [JsonProperty]
    public IStationaryElement Destination
    {
      get { return Get<IStationaryElement>(); }
      set { Set(value); }
    }

    [JsonProperty]
    public int Priority
    {
      get { return Get<int>(); }
      set { Set(value); }
    }

    [JsonProperty]
    public bool IsRoutingPerProductType
    {
      get { return Get<bool>(); }
      set { Set(value); }
    }

    [JsonProperty]
    private readonly List<IProductType> _ProductTypes = new List<IProductType>();

    public virtual IEnumerable<IProductType> ProductTypes
    {
      get { return _ProductTypes; }
    }


    public virtual void Add(IProductType productType)
    {
      if (_ProductTypes.Contains(productType))
      {
        throw new InvalidOperationException(string.Format("The connection already contains the given product type: {0}", productType));
      }
      _ProductTypes.Add(productType);
    }


    public virtual void Remove(IProductType productType)
    {
      if (!_ProductTypes.Contains(productType))
      {
        throw new InvalidOperationException(string.Format("The connection does not contain the given product type: {0}", productType));
      }
      _ProductTypes.Remove(productType);
    }
  }
}