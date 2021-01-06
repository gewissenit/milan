#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using Milan.Simulation.Observers;

namespace Milan.Simulation.Factories
{
  public interface ISimulationObserverFactory
  {
    bool CanHandle(ISimulationObserver simulationObserver);
    ISimulationObserver CreateSimulationObserver(ISimulationObserver simulationObserver, IExperiment experiment);
    void Prepare(ISimulationObserver simulationObserver);
  }
}