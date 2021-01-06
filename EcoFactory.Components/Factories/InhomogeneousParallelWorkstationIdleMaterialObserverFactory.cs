using System.ComponentModel.Composition;
using Milan.Simulation;
using Milan.Simulation.Factories;
using Milan.Simulation.MaterialAccounting;
using Milan.Simulation.MaterialAccounting.Factories;
using Milan.Simulation.Observers;

namespace EcoFactory.Components.Factories
{
  [Export(typeof (IMaterialObserverFactory))][Export(typeof(ISimulationObserverFactory))]
  internal class InhomogeneousParallelWorkstationIdleMaterialObserverFactory : IMaterialObserverFactory
  {
    public string Name
    {
      get { return "Idle"; }
    }

    public bool CanHandle(IEntity entity)
    {
      return entity.GetType() == typeof (InhomogeneousParallelWorkstation);
    }

    public IMaterialObserver Create()
    {
      return new MaterialObserverExtensions.InhomogeneousParallelWorkstation.Idle();
    }

    public IMaterialObserver Duplicate(IMaterialObserver materialObserver)
    {
      return Clone(materialObserver);
    }

    public bool CanHandle(ISimulationObserver simulationObserver)
    {
      return simulationObserver.GetType() == typeof (MaterialObserverExtensions.InhomogeneousParallelWorkstation.Idle);
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
      var master = (MaterialObserverExtensions.InhomogeneousParallelWorkstation.Idle) costObserver;
      var clone = new MaterialObserverExtensions.InhomogeneousParallelWorkstation.Idle();
      Milan.Simulation.Utils.CloneEntityObserver(clone, master);
      Milan.Simulation.MaterialAccounting.Utils.CloneMaterialObserver(clone, master);
      Milan.Simulation.Utils.CloneTimeReferencedObserver(clone, master);
      return clone;
    }
  }
}