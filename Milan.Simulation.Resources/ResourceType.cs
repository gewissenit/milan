#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.Collections.Generic;
using Emporer.Math.Distribution;
using Newtonsoft.Json;

namespace Milan.Simulation.Resources
{
  [JsonObject(MemberSerialization.OptIn)]
  public class ResourceType : Entity, IResourceType
  {
    private readonly List<IResourceTypeInfluence> _influences;


    public ResourceType()
    {
      _influences = new List<IResourceTypeInfluence>();
    }

    [JsonProperty]
    public IEnumerable<IResourceTypeInfluence> Influences
    {
      get
      {
        return _influences;
      }
    }

    [JsonProperty]
    public IDistributionConfiguration MaintenanceDuration
    {
      get { return Get<IDistributionConfiguration>(); }
      set { Set(value); }
    }

    [JsonProperty]
    public IDistributionConfiguration UsageAmount
    {
      get { return Get<IDistributionConfiguration>(); }
      set { Set(value); }
    }

    public void Add(IResourceTypeInfluence influence)
    {
      _influences.Add(influence);
    }

    public void Remove(IResourceTypeInfluence influnence)
    {
      _influences.Remove(influnence);
    }

    public override string ToString()
    {
      return Name;
    }
  }
}