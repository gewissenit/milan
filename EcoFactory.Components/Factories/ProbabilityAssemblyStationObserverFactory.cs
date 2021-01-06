using System.ComponentModel.Composition;
using EcoFactory.Components.Observers;
using Milan.Simulation;
using Milan.Simulation.Factories;
using Milan.Simulation.Observers;

namespace EcoFactory.Components.Factories
{
  [Export(typeof (IStatisticalObserverFactory))]
  internal class ProbabilityAssemblyStationObserverFactory : IStatisticalObserverFactory
  {
    public string Name
    {
      get { return "Probability Assembly station processes"; }
    }

    public ISimulationObserver Create(IExperiment experiment)
    {
      var observer = new ProbabilityAssemblyStationObserver();
      observer.Configure(experiment);
      return observer;
    }
  }
}