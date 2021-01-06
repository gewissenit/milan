#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using Milan.Simulation.Events;
using Milan.Simulation.Observers;
using Newtonsoft.Json;

namespace Milan.Simulation.MaterialAccounting
{
  [JsonObject(MemberSerialization.OptIn)]
  public abstract class ProcessMaterialObserver<TEntity, TStartEvent, TEndEvent> : EventMaterialObserver<TEntity, TEndEvent>, IProcessMaterialObserver<TEntity, TStartEvent, TEndEvent>
    where TEntity : class, IEntity
    where TStartEvent : class, ISimulationEvent
    where TEndEvent : class, IRelatedEvent
  {
    protected ProcessMaterialObserver(string name)
      : base(name)
    {
    }
    
    [JsonProperty]
    public TimeReference TimeReference
    {
      get { return Get<TimeReference>(); }
      set { Set(value); }
    }

    public override string ToString()
    {
      return $"{Name}";
    }

    protected double ApplyTimeReference(double value, double duration)
    {
      switch (TimeReference)
      {
        case TimeReference.Once:
          return value;
        case TimeReference.PerMillisecond:
          return value * duration;
        case TimeReference.PerSecond:
          return value * (duration / 1000);
        case TimeReference.PerMinute:
          return value * (duration / 60000);
        case TimeReference.PerHour:
          return value * (duration / 3600000);
        case TimeReference.PerDay:
          return value * (duration / 86400000);
        default:
          throw new InvalidOperationException();
      }
    }

    protected override void OnEntityEventOccurred(TEndEvent endEvent)
    {
      var duration = endEvent.ScheduledTime - endEvent.RelatedEvent.ScheduledTime;
      var value = Unit.ToBaseUnit(Amount);
      value = ApplyTimeReference(value, duration);
      
      CreateBalancePosition(endEvent, Entity, Material, CurrentExperiment, value, LossRatio, Name, Category);
    }
  }
}