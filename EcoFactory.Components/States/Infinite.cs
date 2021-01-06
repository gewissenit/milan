#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using Milan.Simulation;
using Milan.Simulation.Events;
using Milan.Simulation.Scheduling;

namespace EcoFactory.Components.States
{
  public class Infinite: IState
  {
    private readonly IWorkstationBase _entity;
    private readonly Func<ISimulationEvent, IRelatedEvent> _getEndEvent;
    private readonly Func<ISimulationEvent> _getStartEvent;
    private readonly string _name;
    private ISimulationEvent _startEvent;

    public Infinite(string name, IWorkstationBase entity, Func<ISimulationEvent> getStartEvent, Func<ISimulationEvent, IRelatedEvent> getEndEvent)
    {
      _getEndEvent = getEndEvent;
      _getStartEvent = getStartEvent;
      _name = name;
      _entity = entity;
      OnEnter += () =>
                 {
                 };
    }

    public Action OnEnter { private get; set; }
    public bool Active { get; private set; }
    
    public void Enter()
    {
      if (Active)
      {
        throw new InvalidOperationException("This should not occur!");
      }

      Active = true;

      _entity.CurrentExperiment.Scheduler.SchedulableHandled += SetEndEventWhenSimulationEndEventOccurs;

      _startEvent = _getStartEvent();
      _startEvent.Schedule(0);
      OnEnter();
    }

    private void SetEndEventWhenSimulationEndEventOccurs(object sender, SchedulerEventArgs e)
    {
      if (e.Schedulable is SimulationEndEvent)
      {
        Exit();
      }
    }

    public void Exit()
    {
      if (!Active)
      {
        throw new InvalidOperationException("This should not occur!");
      }

      Active = false;

      _entity.CurrentExperiment.Scheduler.SchedulableHandled -= SetEndEventWhenSimulationEndEventOccurs;

      var endEvent = _getEndEvent(_startEvent);
      endEvent.Schedule(0);
    }
  }
}