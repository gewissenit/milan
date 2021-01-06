using System.ComponentModel.Composition;
using EcoFactory.Components.Observers;
using Milan.Simulation;
using Milan.Simulation.Factories;
using Milan.Simulation.Observers;

namespace EcoFactory.Components.Factories
{
  [Export(typeof (IStatisticalObserverFactory))]
  internal class AssemblyStationObserverFactory : IStatisticalObserverFactory
  {
    public string Name
    {
      get { return "Assembly station processes"; }
    }

    public ISimulationObserver Create(IExperiment experiment)
    {
      var observer = new AssemblyStationObserver();
      observer.Configure(experiment);
      return observer;
    }
  }
}