using System.ComponentModel.Composition;
using Milan.Simulation;
using Milan.Simulation.Factories;
using Milan.Simulation.MaterialAccounting;
using Milan.Simulation.MaterialAccounting.Factories;
using Milan.Simulation.Observers;

namespace EcoFactory.Components.Factories
{
  [Export(typeof (IMaterialObserverFactory))][Export(typeof(ISimulationObserverFactory))]
  internal class ProbabilityAssemblyStationFailureMaterialObserverFactory : IMaterialObserverFactory
  {
    public string Name
    {
      get { return "Failure"; }
    }

    public bool CanHandle(IEntity entity)
    {
      return entity.GetType() == typeof (ProbabilityAssemblyStation);
    }

    public IMaterialObserver Create()
    {
      return new MaterialObserverExtensions.ProbabilityAssemblyStation.Failure();
    }

    public IMaterialObserver Duplicate(IMaterialObserver materialObserver)
    {
      return Clone(materialObserver);
    }

    public bool CanHandle(ISimulationObserver simulationObserver)
    {
      return simulationObserver.GetType() == typeof (MaterialObserverExtensions.ProbabilityAssemblyStation.Failure);
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
      Milan.Simulation.Utils.PrepareEntityObserver((IEntityObserver) simulationObserver);
    }

    private IMaterialObserver Clone(ISimulationObserver costObserver)
    {
      var master = (MaterialObserverExtensions.ProbabilityAssemblyStation.Failure) costObserver;
      var clone = new MaterialObserverExtensions.ProbabilityAssemblyStation.Failure();
      Milan.Simulation.Utils.CloneEntityObserver(clone, master);
      Milan.Simulation.MaterialAccounting.Utils.CloneMaterialObserver(clone, master);
      Milan.Simulation.Utils.CloneTimeReferencedObserver(clone, master);
      return clone;
    }
  }
}