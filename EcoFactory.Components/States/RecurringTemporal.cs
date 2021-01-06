#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using Emporer.Math.Distribution;
using Milan.Simulation;
using Milan.Simulation.Events;

namespace EcoFactory.Components.States
{
  public class RecurringTemporal
  {
    private readonly IRealDistribution _durationDist;
    private readonly IWorkstationBase _entity;
    private readonly Func<ISimulationEvent, IRelatedEvent> _getEndEvent;
    private readonly Func<ISimulationEvent> _getStartEvent;
    private readonly string _name;
    private readonly IRealDistribution _occurrenceDist;
    private ISimulationEvent _endEvent;
    private ISimulationEvent _startEvent;

    public RecurringTemporal(string name,
                             IWorkstationBase entity,
                             IRealDistribution occurrenceDist,
                             IRealDistribution durationDist,
                             Func<ISimulationEvent> getStartEvent,
                             Func<ISimulationEvent, IRelatedEvent> getEndEvent)
    {
      _occurrenceDist = occurrenceDist;
      _durationDist = durationDist;
      _getEndEvent = getEndEvent;
      _getStartEvent = getStartEvent;
      CreateNextOccurrence();
      _name = name;
      _entity = entity;
      OnStart += () =>
                 {
                 };
      OnFinish += () =>
                {
                };
    }

    public Action OnStart { private get; set; }
    public Action OnFinish { private get; set; }
    public bool Active { get; private set; }

    private void Enter()
    {
      if (Active)
      {
        throw new InvalidOperationException("This should not occur!");
      }
      Active = true;
      _endEvent = _getEndEvent(_startEvent);
      _endEvent.OnOccur += _ =>
                           {
                             if (!Active)
                             {
                               throw new InvalidOperationException("This should not occur!");
                             }
                             Active = false;
                             OnFinish();
                           };
      var duration = _durationDist.GetSample();
      if (duration == 0)
      {
        throw new ModelConfigurationException(_entity.Model, _entity, "A duration should be greater than 0!");
      }
      _endEvent.Schedule(duration);
      var occurenceTime = CreateNextOccurrence();
      if (occurenceTime < duration)
      {
        throw new ModelConfigurationException(_entity.Model,
                                              _entity,
                                              "This process is already active. Shorter occurences then its durations are not supported atm.");
      }
      OnStart();
    }

    private double CreateNextOccurrence()
    {
      _startEvent = _getStartEvent();
      _startEvent.OnOccur += _ => Enter();
      var duration = _occurrenceDist.GetSample();
      if (duration == 0)
      {
        throw new ModelConfigurationException(_entity.Model, _entity, "A failure occurence should be greater than 0!");
      }
      _startEvent.Schedule(duration);
      return duration;
    }
  }
}