#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.Collections.Generic;

namespace Milan.Simulation.Factories
{
  public interface IExperimentFactory
  {
    IExperiment Create(IModel model, int seed, IEnumerable<IStatisticalObserverFactory> statisticalObserverFactories, double settlingTime);
  }
}