using System.ComponentModel.Composition;
using Milan.Simulation.Observers;

namespace Milan.Simulation.Factories
{
  [Export(typeof (IStatisticalObserverFactory))]
  internal class ProductLifeCycleLogWriterFactory : IStatisticalObserverFactory
  {
    public string Name
    {
      get
      {
        return "Product Lifecycle Log Writer";
      }
    }

    public ISimulationObserver Create(IExperiment experiment)
    {
      var observer = new ProductLifeCycleLogWriter();
      observer.Configure(experiment);
      return observer;
    }
  }
}