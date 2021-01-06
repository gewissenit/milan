#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Collections.Generic;
using EcoFactory.Components.Events;
using Emporer.Math.Distribution;
using Milan.Simulation;
using Milan.Simulation.Events;

namespace EcoFactory.Components.States
{
  public class ArrivalProcess
  {
    private readonly double _batchSize;
    private readonly IRealDistribution _distribution;
    private readonly Action<ISimulationEvent, ArrivalProcess> _endAction;
    private readonly IStationaryElement _entity;
    private readonly IProductType _productType;
    private ISimulationEvent _currentEndEvent;
    private double? _remainingDuration;

    public ArrivalProcess(IStationaryElement entity,
                          double batchSize,
                          IProductType productType,
                          IRealDistribution distribution,
                          Action<ISimulationEvent, ArrivalProcess> endAction)
    {
      _batchSize = batchSize;
      _productType = productType;
      _distribution = distribution;
      _entity = entity;
      _endAction = endAction;
    }

    public void Start()
    {
      if (_currentEndEvent != null)
      {
        throw new InvalidOperationException("This should not occur!");
      }

      var timeDifference = _distribution.GetSample();
      //TODO: list of values can contain 0 as value. this should be checked only for probability distributions in entry editor.
      //if (timeDifference == 0)
      //{
      //  throw new ModelConfigurationException(_entity.Model, _entity, "An arrival occurence should be greater than 0!");
      //}
      var products = new List<Product>();
      for (var i = 0; i < _batchSize; i++)
      {
        products.Add(new Product(_entity.Model, _productType, _entity.CurrentExperiment.CurrentTime));
      }

      var arrival = new ProductsArrivedEvent(_entity, products);
      arrival.OnOccur = _ =>
                        {
                          if (_currentEndEvent == null)
                          {
                            throw new InvalidOperationException("This should not occur!");
                          }
                          _currentEndEvent = null;
                          _endAction(arrival, this);
                        };
      arrival.Schedule(timeDifference);
      _currentEndEvent = arrival;
    }

    public void Suspend()
    {
      if (_remainingDuration.HasValue)
      {
        throw new InvalidOperationException("This should not occur!");
      }
      _entity.CurrentExperiment.Scheduler.RemoveSchedulable(_currentEndEvent);
      _remainingDuration = _currentEndEvent.ScheduledTime - _entity.CurrentExperiment.CurrentTime;
    }

    public void Resume()
    {
      if (!_remainingDuration.HasValue)
      {
        throw new InvalidOperationException("This should not occur!");
      }
      _currentEndEvent.Schedule(_remainingDuration.Value);
      _remainingDuration = null;
    }
  }
}