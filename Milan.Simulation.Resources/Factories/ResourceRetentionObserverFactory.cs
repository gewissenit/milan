using System.ComponentModel.Composition;
using Milan.Simulation.Factories;
using Milan.Simulation.Observers;
using Milan.Simulation.Resources.Observers;

namespace Milan.Simulation.Resources.Factories
{
  [Export(typeof (IStatisticalObserverFactory))]
  internal class ResourceRetentionObserverFactory: IStatisticalObserverFactory
  {
    public string Name
    {
      get
      {
        return "Resource Retention Times";
      }
    }

    public ISimulationObserver Create(IExperiment experiment)
    {
      var observer = new ResourceRetentionObserver();
      observer.Configure(experiment);
      return observer;
    }
  }
}