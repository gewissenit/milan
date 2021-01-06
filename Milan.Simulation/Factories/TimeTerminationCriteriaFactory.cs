#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.ComponentModel.Composition;
using Milan.Simulation.Observers;

namespace Milan.Simulation.Factories
{
  [Export(typeof (ITerminationCriteriaFactory))]
  [Export(typeof (ISimulationObserverFactory))]
  internal class TimeTerminationCriteriaFactory : ITerminationCriteriaFactory
  {
    public ITerminationCriteria Create()
    {
      var newInstance = new TimeTerminationCriteria();
      return newInstance;
    }

    public ITerminationCriteria Duplicate(ITerminationCriteria terminationCriteria)
    {
      return Clone(terminationCriteria);
    }

    public bool CanHandle(ISimulationObserver simulationObserver)
    {
      return simulationObserver.GetType() == typeof (TimeTerminationCriteria);
    }

    public ISimulationObserver CreateSimulationObserver(ISimulationObserver simulationObserver, IExperiment experiment)
    {
      var clone = Clone(simulationObserver);
      clone.Id = simulationObserver.Id;
      clone.Configure(experiment);
      return clone;
    }

    public void Prepare(ISimulationObserver simulationObserver)
    {
    }

    private ITerminationCriteria Clone(ISimulationObserver simulationObserver)
    {
      var master = (TimeTerminationCriteria) simulationObserver;
      var clone = new TimeTerminationCriteria
                  {
                    Duration = master.Duration
                  };
      return clone;
    }
  }
}