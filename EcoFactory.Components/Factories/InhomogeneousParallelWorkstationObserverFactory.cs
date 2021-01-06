#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.ComponentModel.Composition;
using EcoFactory.Components.Observers;
using Milan.Simulation;
using Milan.Simulation.Factories;
using Milan.Simulation.Observers;

namespace EcoFactory.Components.Factories
{
  [Export(typeof (IStatisticalObserverFactory))]
  internal class InhomogeneousParallelWorkstationObserverFactory : IStatisticalObserverFactory
  {
    public string Name
    {
      get { return "Inhomogeneous parallel workstation processes"; }
    }

    public ISimulationObserver Create(IExperiment experiment)
    {
      var observer = new InhomogeneousParallelWorkstationObserver();
      observer.Configure(experiment);
      return observer;
    }
  }
}