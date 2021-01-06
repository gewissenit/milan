using System.ComponentModel.Composition;
using Milan.Simulation.Observers;

namespace Milan.Simulation.Factories
{
  [Export(typeof(IStatisticalObserverFactory))]
  internal class ThroughputObserverFactory : IStatisticalObserverFactory
  {
    public string Name
    {
      get { return "Accounting Product exited status"; }
    }

    public ISimulationObserver Create(IExperiment experiment)
    {
      var observer = new ThroughputObserver();
      observer.Configure(experiment);
      return observer;
    }
  }
}