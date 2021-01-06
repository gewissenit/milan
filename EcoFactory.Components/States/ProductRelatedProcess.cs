#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using Emporer.Math.Distribution;
using Milan.Simulation;
using Milan.Simulation.Events;

namespace EcoFactory.Components.States
{
  public class ProductRelatedProcess
  {
    private readonly IWorkstationBase _entity;
    private readonly Func<ISimulationEvent> _getCancelEvent;
    private readonly Func<ISimulationEvent, IRelatedEvent> _getEndEvent;
    private readonly Func<IProductsRelatedEvent> _getStartEvent;
    private readonly string _name;
    private IRelatedEvent _endEvent;
    private IProductsRelatedEvent _startEvent;

    public ProductRelatedProcess(string name,
                                 IWorkstationBase entity,
                                 Func<IProductsRelatedEvent> getStartEvent,
                                 Func<ISimulationEvent, IRelatedEvent> getEndEvent,
                                 Func<ISimulationEvent> getCancelEvent)
    {
      _getCancelEvent = getCancelEvent;
      _getEndEvent = getEndEvent;
      _getStartEvent = getStartEvent;
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
    public IRealDistribution DurationDistribution { private get; set; }
    public IDictionary<IProductType, IRealDistribution> ProductTypeDistributions { private get; set; }

    public void Start()
    {
      if (Active)
      {
        throw new InvalidOperationException("This should not occur!");
      }
      Active = true;
      // marker event start
      _startEvent = _getStartEvent();
      _startEvent.OnOccur += _ => OnStarted();
      _startEvent.Schedule(0);
      OnStart();
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

    private void OnStarted()
    {
      var products = _startEvent.Products.ToArray();

      var dist = DurationDistribution;
      if (ProductTypeDistributions != null)
      {
        var productType = products.First()
                                  .ProductType;
        if (ProductTypeDistributions.ContainsKey(productType))
        {
          dist = ProductTypeDistributions[productType];
        }
      }

      if (dist == null)
      {
        throw new ModelConfigurationException(_entity.Model, _entity, string.Format("{0} has no duration distribution configured.", _name));
      }

      // end event
      var endEvent = _getEndEvent(_startEvent);
      endEvent.OnOccur = _ =>
                         {
                           if (!Active)
                           {
                             throw new InvalidOperationException("This should not occur!");
                           }
                           Active = false;
                           OnFinish();
                         };
      var duration = dist.GetSample();
      if (duration == 0)
      {
        throw new ModelConfigurationException(_entity.Model, _entity, "A duration should be greater than 0!");
      }
      endEvent.Schedule(duration);
      _endEvent = endEvent;
    }
  }
}