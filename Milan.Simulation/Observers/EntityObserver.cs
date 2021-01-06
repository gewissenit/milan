#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;

using Milan.Simulation.Events;
using Newtonsoft.Json;

namespace Milan.Simulation.Observers
{
  [JsonObject(MemberSerialization.OptIn)]
  public abstract class EntityObserver<TEntity> : EntityTypeObserver<TEntity>, IEntityObserver<TEntity>
    where TEntity : class, IEntity
  {
    [JsonProperty]
    public TEntity Entity
    {
      get
      {
        return Get<TEntity>();
      }
      set
      {
        Set(value);
      }
    }

    IEntity IEntityObserver.Entity
    {
      get { return Entity; }
      set
      {
        if (value is TEntity)
        {
          Entity = (TEntity) value;
        }
        else
        {
          throw new ArgumentException("Incompatile entity type.");
        }
      }
    }

    public override string ToString()
    {
      return string.Format("Observer ({0}, {1})", Id, typeof (TEntity).Name);
    }

    protected abstract void OnEntityEventOccurred(ISimulationEvent e);

    protected override void OnEntityTypeEventOccurred(ISimulationEvent e)
    {
      if (e.Sender == Entity)
      {
        OnEntityEventOccurred(e);
      }
    }
  }
}