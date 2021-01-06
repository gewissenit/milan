using System.ComponentModel.Composition;
using EcoFactory.Components.Observers;
using Milan.Simulation;
using Milan.Simulation.Factories;
using Milan.Simulation.Observers;

namespace EcoFactory.Components.Factories
{
  [Export(typeof (IStatisticalObserverFactory))]
  internal class ParallelWorkstationObserverFactory : IStatisticalObserverFactory
  {
    public string Name
    {
      get { return "Parallel workstation processes"; }
    }

    public ISimulationObserver Create(IExperiment experiment)
    {
      var observer = new ParallelWorkstationObserver();
      observer.Configure(experiment);
      return observer;
    }
  }
}