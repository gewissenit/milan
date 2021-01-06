using System.ComponentModel.Composition;
using EcoFactory.Components.Observers;
using Milan.Simulation;
using Milan.Simulation.Factories;
using Milan.Simulation.Observers;

namespace EcoFactory.Components.Factories
{
  [Export(typeof (IStatisticalObserverFactory))]
  internal class StorageObserverFactory : IStatisticalObserverFactory
  {
    public string Name
    {
      get { return "Storage processes"; }
    }

    public ISimulationObserver Create(IExperiment experiment)
    {
      var observer = new StorageObserver();
      observer.Configure(experiment);
      return observer;
    }
  }
}