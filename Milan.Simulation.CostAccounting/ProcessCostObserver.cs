#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using Milan.Simulation.Events;
using Milan.Simulation.Observers;
using Newtonsoft.Json;

namespace Milan.Simulation.CostAccounting
{
  [JsonObject(MemberSerialization.OptIn)]
  public class ProcessCostObserver<TEntity, TStartEvent, TEndEvent> : EventCostObserver<TEntity, TEndEvent>, IProcessCostObserver<TEntity, TStartEvent, TEndEvent>
    where TStartEvent : class, ISimulationEvent
    where TEndEvent : class, IRelatedEvent
    where TEntity : class, IEntity
  {
    protected ProcessCostObserver(string name)
      : base(name)
    {
    }

    [JsonProperty]
    public TimeReference TimeReference
    {
      get { return Get<TimeReference>(); }
      set { Set(value); }
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
      var startEvent = endEvent.RelatedEvent as TStartEvent;
      if (startEvent == null)
      {
        return;
      }

      var duration = endEvent.ScheduledTime - endEvent.RelatedEvent.ScheduledTime;
      var value = ApplyTimeReference(Amount, duration);

      CreateBalancePosition(endEvent, Entity, Currency, CurrentExperiment, value, LossRatio, Name, Category);
    }
  }
}