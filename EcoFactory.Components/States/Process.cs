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
  public class Process
  {
    private readonly IWorkstationBase _entity;
    private readonly Func<ISimulationEvent> _getCancelEvent;
    private readonly Func<ISimulationEvent, IRelatedEvent> _getEndEvent;
    private readonly Func<ISimulationEvent> _getStartEvent;
    private readonly string _name;
    private IRelatedEvent _endEvent;
    private ISimulationEvent _startEvent;

    public Process(string name,
                    IWorkstationBase entity,
                    Func<ISimulationEvent> getStartEvent,
                    Func<ISimulationEvent, IRelatedEvent> getEndEvent,
                    Func<ISimulationEvent> getCancelEvent)
    {
      _getCancelEvent = getCancelEvent;
      _getEndEvent = getEndEvent;
      _getStartEvent = getStartEvent;
      _name = name;
      _entity = entity;
      OnFinish += () =>
                {
                };
    }

    public Action OnFinish { get; set; }
    public bool Active { get; private set; }
    public IRealDistribution DurationDistribution { get; set; }
    
    public void Start()
    {
      if (Active)
      {
        throw new InvalidOperationException("This should not occur!");
      }
      Active = true;
      // marker event start
      _startEvent = _getStartEvent();
      _startEvent.OnOccur += OnStarted;
      _startEvent.Schedule(0);
    }

    public void Cancel()
    {
      if (!Active)
      {
        return;
      }

      Active = false;
      if (_entity.CurrentExperiment.Scheduler.IsScheduled(_startEvent))
      {
        _entity.CurrentExperiment.Scheduler.RemoveSchedulable(_startEvent);
        return;
      }
      var specificCancel = _getCancelEvent();
      specificCancel.Schedule(0);
      _entity.CurrentExperiment.Scheduler.RemoveSchedulable(_endEvent);
    }

    private void OnStarted(ISimulationEvent startEvent)
    {
      if (DurationDistribution == null)
      {
        throw new ModelConfigurationException(_entity.Model, _entity, $"{_name} has no duration distribution configured.");
      }

      // end event
      var endEvent = _getEndEvent(startEvent);
      endEvent.OnOccur = _ =>
                         {
                           if (!Active)
                           {
                             throw new InvalidOperationException("This should not occur!");
                           }
                           Active = false;
                           OnFinish();
                         };
      var duration = DurationDistribution.GetSample();
      if (duration == 0)
      {
        throw new ModelConfigurationException(_entity.Model, _entity, "A duration should be greater than 0!");
      }
      endEvent.Schedule(duration);
      _endEvent = endEvent;
    }
  }
}