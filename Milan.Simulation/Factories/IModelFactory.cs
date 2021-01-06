#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.Collections.Generic;
using Milan.Simulation.Observers;

namespace Milan.Simulation.Factories
{
  public interface IModelFactory
  {
    IModel Create();
    IModel Duplicate(IModel model);
    void Delete(IModel model);
    IModel CreateSimulationModel(IModel model, IEnumerable<IStatisticalObserverFactory> statisticalObserverFactories, IExperiment experiment);
  }
}