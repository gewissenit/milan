using System.ComponentModel.Composition;
using Milan.Simulation.Observers;

namespace Milan.Simulation.Factories
{
  [Export(typeof (IStatisticalObserverFactory))]
  internal class ExperimentObserverFactory : IStatisticalObserverFactory
  {
    public string Name
    {
      get { return "Events"; }
    }

    public ISimulationObserver Create(IExperiment experiment)
    {
      var observer = new ExperimentObserver();
      observer.Configure(experiment);
      return observer;
    }
  }
}