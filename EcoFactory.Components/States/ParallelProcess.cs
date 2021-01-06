#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using EcoFactory.Components.Events;
using Emporer.Math.Distribution;
using Milan.Simulation;
using Milan.Simulation.Events;

namespace EcoFactory.Components.States
{
  public class ParallelProcess
  {
    private readonly IStationaryElement _entity;
    private readonly string _name;
    private IRelatedEvent _currentEndEvent;
    private double? _remainingDuration;

    public ParallelProcess(string name, IStationaryElement entity, IEnumerable<Product> products)
    {
      Products = new Queue<Product>(products);
      _name = name;
      _entity = entity;
      OnFinish += (pp) =>
                  {
                  };
    }

    public Action<ParallelProcess> OnFinish { get; set; }
    public Queue<Product> Products { get; private set; }
    public IRealDistribution DurationDistribution { get; set; }
    public IDictionary<IProductType, IRealDistribution> ProductTypeDistributions { get; set; }
    public bool Active { get; set; }

    public void Start()
    {
      if (Active)
      {
        throw new InvalidOperationException("This should not occur.");
      }

      var dist = DurationDistribution;

      Active = true;

      var startEvent = new ProcessingStartEvent(_entity, Products);
      startEvent.Schedule(0);

      if (ProductTypeDistributions != null)
      {
        var productType = Products.First()
                                  .ProductType;

        if (ProductTypeDistributions.ContainsKey(productType))
        {
          dist = ProductTypeDistributions[productType];
        }
      }
      if (dist == null)
      {
        throw new ModelConfigurationException(_entity.Model, _entity, string.Format("({0}) has no duration distribution configured.", _name));
      }

      // end event
      var duration = dist.GetSample();
      if (duration == 0)
      {
        throw new ModelConfigurationException(_entity.Model, _entity, "A duration should be greater than 0!");
      }
      var endEvent = new ParallelProcessingEndEvent(_entity, Products.ToArray(), startEvent, duration)
                     {
                       OnOccur = _ =>
                                 {
                                   if (!Active)
                                   {
                                     throw new InvalidOperationException("This should not occur.");
                                   }

                                   Active = false;
                                   _currentEndEvent = null;
                                   OnFinish(this);
                                 }
                     };

      endEvent.Schedule(duration);
      _currentEndEvent = endEvent;
    }

    public void Suspend()
    {
      if (!Active ||
          _remainingDuration.HasValue)
      {
        throw new InvalidOperationException("This should not occur!");
      }

      Active = false;

      var psuspend = new ProcessingSuspendEvent(_entity, ((ProcessingEndEvent) _currentEndEvent).Products.ToArray());
      psuspend.Schedule(0);
      _entity.CurrentExperiment.Scheduler.RemoveSchedulable(_currentEndEvent);
      _remainingDuration = _currentEndEvent.ScheduledTime - _entity.CurrentExperiment.CurrentTime;
    }

    public void Resume()
    {
      if (Active || !_remainingDuration.HasValue)
      {
        throw new InvalidOperationException("This should not occur!");
      }

      Active = true;

      var presume = new ProcessingResumeEvent(_entity, ((ProcessingEndEvent) _currentEndEvent).Products.ToArray());
      presume.Schedule(0);
      _currentEndEvent.Schedule(_remainingDuration.Value);
      _remainingDuration = null;
    }

    public void Cancel()
    {
      _remainingDuration = null;
      Active = false;
      if (_currentEndEvent == null)
      {
        return;
      }

      var specificCancel = new ProductsDestroyedEvent(_entity, Products.ToArray());
      specificCancel.Schedule(0);

      _entity.CurrentExperiment.Scheduler.RemoveSchedulable(_currentEndEvent);
      _currentEndEvent = null;
    }
  }
}