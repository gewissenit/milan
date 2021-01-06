#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using Milan.Simulation.Factories;

namespace Milan.Simulation
{
  public interface ISimulationService
  {
    IBatch CreateBatch(IModel model, int numberOfRuns, IStatisticalObserverFactory[] statisticalObserverFactories, TimeSpan settlingTime);
    void DeleteBatch(IBatch batch);
    void Start(IBatch batch);
    event EventHandler<BatchEventArgs> BatchCreated;
    event EventHandler<BatchEventArgs> BatchDeleted;
    event EventHandler<AggregateException> BatchError;
  }
}