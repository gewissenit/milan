using System.ComponentModel.Composition;
using Milan.Simulation.Observers;

namespace Milan.Simulation.Factories
{
  [Export(typeof (IStatisticalObserverFactory))]
  internal class EventLogWriterFactory : IStatisticalObserverFactory
  {
    public string Name
    {
      get { return "Event Log Writer"; }
    }

    public ISimulationObserver Create(IExperiment experiment)
    {
      var observer = new EventLogWriter();
      observer.Configure(experiment);
      return observer;
    }
  }
}