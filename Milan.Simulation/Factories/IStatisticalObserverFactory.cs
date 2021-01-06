#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using Milan.Simulation.Observers;

namespace Milan.Simulation.Factories
{
  public interface IStatisticalObserverFactory
  {
    string Name { get; }
    ISimulationObserver Create(IExperiment experiment);
  }
}