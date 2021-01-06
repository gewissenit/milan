using System.ComponentModel.Composition;
using Milan.Simulation;
using Milan.Simulation.Factories;
using Milan.Simulation.MaterialAccounting;
using Milan.Simulation.MaterialAccounting.Factories;
using Milan.Simulation.Observers;

namespace EcoFactory.Components.Factories
{
  [Export(typeof (IMaterialObserverFactory))][Export(typeof(ISimulationObserverFactory))]
  internal class WorkstationProcessingMaterialObserverFactory : IMaterialObserverFactory
  {
    public string Name
    {
      get { return "Processing"; }
    }

    public bool CanHandle(IEntity entity)
    {
      return entity.GetType() == typeof (Workstation);
    }

    public IMaterialObserver Create()
    {
      return new MaterialObserverExtensions.Workstation.Processing();
    }

    public IMaterialObserver Duplicate(IMaterialObserver materialObserver)
    {
      return Clone(materialObserver);
    }

    public bool CanHandle(ISimulationObserver simulationObserver)
    {
      return simulationObserver.GetType() == typeof (MaterialObserverExtensions.Workstation.Processing);
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
      var observer = (MaterialObserverExtensions.Workstation.Processing) simulationObserver;
      Milan.Simulation.Utils.PrepareEntityObserver(observer);
      Milan.Simulation.Utils.PrepareProductRelatedObserver(observer);
    }

    private IMaterialObserver Clone(ISimulationObserver costObserver)
    {
      var master = (MaterialObserverExtensions.Workstation.Processing) costObserver;
      var clone = new MaterialObserverExtensions.Workstation.Processing();
      Milan.Simulation.Utils.CloneEntityObserver(clone, master);
      Milan.Simulation.MaterialAccounting.Utils.CloneMaterialObserver(clone, master);
      Milan.Simulation.Utils.CloneProductRelatedObserver(clone, master);
      Milan.Simulation.Utils.CloneTimeReferencedObserver(clone, master);
      return clone;
    }
  }
}