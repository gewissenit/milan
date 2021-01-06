#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.Collections.Generic;
using Milan.JsonStore;
using Newtonsoft.Json;

namespace Milan.Simulation
{
  public abstract class Entity : DomainEntity, IEntity
  {
    protected bool _initialized;

    protected Entity()
    {
      ExtendingPropertyValues = new Dictionary<string, object>();
      Name = string.Empty;
    }

    public IDictionary<string, object> ExtendingPropertyValues { get; private set; }

    [JsonProperty]
    public string Name
    {
      get { return Get<string>(); }
      set { Set(value); }
    }

    [JsonProperty]
    public string Description
    {
      get { return Get<string>(); }
      set { Set(value); }
    }

    [JsonProperty]
    public IModel Model
    {
      get { return Get<IModel>(); }
      set { Set(value); }
    }

    public virtual void Initialize()
    {
      _initialized = true;
    }

    public virtual void Reset()
    {
      ExtendingPropertyValues.Clear();
      _initialized = false;
    }

    public IExperiment CurrentExperiment { get; set; }

    public override string ToString()
    {
      return Name;
    }
  }
}