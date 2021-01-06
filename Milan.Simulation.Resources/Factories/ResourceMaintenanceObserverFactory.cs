using System.ComponentModel.Composition;
using Milan.Simulation.Factories;
using Milan.Simulation.Observers;
using Milan.Simulation.Resources.Observers;

namespace Milan.Simulation.Resources.Factories
{
  [Export(typeof (IStatisticalObserverFactory))]
  internal class ResourceMaintenanceObserverFactory: IStatisticalObserverFactory
  {
    public string Name
    {
      get
      {
        return "Resource Maintenance Times";
      }
    }

    public ISimulationObserver Create(IExperiment experiment)
    {
      var observer = new ResourceMaintenanceObserver();
      observer.Configure(experiment);
      return observer;
    }
  }
}